// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using OpenNos.Core;
using OpenNos.Core.Logging;
using WingsEmu.Communication;
using WingsEmu.Communication.RPC;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs.Character;
using WingsEmu.Master.Datas;
using WingsEmu.Master.Extensions;
using WingsEmu.Master.Managers;

namespace WingsEmu.Master
{
    public class MasterImpl : global::Master.MasterBase
    {
        private readonly WorldServerCommunicationManager _communicationManager;
        private readonly MaintenanceManager _maintenanceManager;
        private readonly SessionManager _sessionManager;
        private readonly WorldServerManager _worldManager;
        private readonly ICharacterDAO _characterDao;

        public MasterImpl(MaintenanceManager maintenanceManager, WorldServerManager worldManager, SessionManager sessionManager, WorldServerCommunicationManager communicationManager,
            ICharacterDAO characterDao)
        {
            _maintenanceManager = maintenanceManager;
            _worldManager = worldManager;
            _sessionManager = sessionManager;
            _communicationManager = communicationManager;
            _characterDao = characterDao;
        }

        public override Task<Bool> IsAccountConnected(Long request, ServerCallContext context) => Task.FromResult(_sessionManager.IsConnectedByAccountId(request.Id).ToBool());

        public override Task<Bool> ConnectAccountOnWorld(ConnectAccountOnWorldRequest request, ServerCallContext context) =>
            Task.FromResult(_sessionManager.ConnectAccountOnWorldId(request.WorldId.ToGuid(), request.AccountId).ToBool());

        public override Task<Void> RegisterAccountLogin(RegisterAccountLoginRequest request, ServerCallContext context)
        {
            _sessionManager.ConnectAccount(request.AccountId, request.SessionId, request.AccountName);
            return Task.FromResult(new Void());
        }
        public override Task<Void> DisconnectAccount(Long request, ServerCallContext context)
        {
            _sessionManager.DisconnectByAccountId(request.Id);
            return Task.FromResult(new Void());
        }

        public override Task<Void> PulseAccount(Long request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.Id);
            if (session == null)
            {
                return Task.FromResult(new Void());
            }

            session.LastPulse = DateTime.Now;
            return Task.FromResult(new Void());
        }

        public override Task<Bool> IsCharacterConnected(IsCharacterConnectedRequest request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByCharacterId(request.WorldGroup, request.CharacterId);
            return Task.FromResult(session == null ? true.ToBool() : (session.ConnectedWorld != null).ToBool());
        }

        public override Task<Bool> ConnectCharacter(ConnectCharacterRequest request, ServerCallContext context)
        {
            WorldServer world = _worldManager.GetWorldById(request.Id.ToGuid());
            if (world == null)
            {
                return Task.FromResult(false.ToBool());
            }

            CharacterDTO character = _characterDao.LoadById(request.CharacterId);
            if (character == null)
            {
                return Task.FromResult(false.ToBool());
            }

            // fetch accountId here
            var characterSession = new CharacterSession(character.Name, character.Level, character.Gender.ToString().ToUpper(), character.Class.ToString().ToUpper());
            return Task.FromResult(_sessionManager.ConnectCharacter(world.Id, request.CharacterId, character.AccountId, characterSession).ToBool());
        }

        public override Task<Void> DisconnectCharacter(DisconnectCharacterRequest request, ServerCallContext context)
        {
            WorldServer world = _worldManager.GetWorldById(request.WorldId.ToGuid());
            if (world == null)
            {
                return Task.FromResult(new Void());
            }

            PlayerSession session = _sessionManager.GetByCharacterId(world.WorldGroup, request.CharacterId);

            _sessionManager.DisconnectByAccountId(session.AccountId);
            return Task.FromResult(new Void());
        }

        public override async Task<Void> SendMessageToCharacter(MessageToCharacter request, ServerCallContext context) => await base.SendMessageToCharacter(request, context);

        public override async Task<Bool> ChangeAuthority(ChangeAuthorityRequest request, ServerCallContext context) => await base.ChangeAuthority(request, context);

        public override Task<Bool> IsLoginPermitted(AccountIdAndSessionIdRequest request, ServerCallContext context)
        {
            return Task.FromResult((_sessionManager.GetByAccountId(request.AccountId) != null).ToBool());
        }

        public override Task<Bool> IsCrossServerLoginPermitted(AccountIdAndSessionIdRequest request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.AccountId);
            if (session == null)
            {
                return Task.FromResult(false.ToBool());
            }

            return Task.FromResult(session.CanSwitchChannel.ToBool());
        }

        public override Task<Void> RegisterCrossServerLogin(RegisterSessionQuery request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.AccountId);
            if (session == null)
            {
                return Task.FromResult(new Void());
            }

            session.CanSwitchChannel = true;
            return Task.FromResult(new Void());
        }

        public override Task<Void> KickSession(AccountIdAndSessionIdRequest request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.AccountId);

            if (session == null)
            {
                return Task.FromResult(new Void());
            }

            ICommunicationClient world = _communicationManager.GetCommunicationClientByWorldId(session.ConnectedWorld.Id);
            world?.KickSession(session.AccountId, session.SessionId);
            return Task.FromResult(new Void());
        }

        public override Task<Bool> IsMasterOnline(Void request, ServerCallContext context) => Task.FromResult(true.ToBool());

        public override Task<Int> RegisterWorldServer(RegisterWorldServerRequest request, ServerCallContext context)
        {
            Guid worldId = request.RegisteredWorldServerInformations.Id.ToGuid();
            SerializableWorldServer serialized = request.RegisteredWorldServerInformations.ToSerializableWorldServer();
            WorldServer newWorld = _worldManager.RegisterWorldServer(serialized);
            _communicationManager.CreateCommunicationClient(worldId, request.GRPCEndPoint.Ip, request.GRPCEndPoint.Port);
            Logger.Log.Info($"[SERVER_REGISTRATION] {serialized.WorldGroup}:{serialized.Id}:{serialized.ChannelId}");
            return Task.FromResult(newWorld.ChannelId.ToInt());
        }

        public override Task<Void> UnregisterWorldServer(UUID request, ServerCallContext context)
        {
            Guid worldId = request.ToGuid();
            _worldManager.UnregisterWorld(worldId);
            _communicationManager.UnregisterCommunicationClient(worldId);
            return Task.FromResult(new Void());
        }

        public override Task<Int> GetChannelIdByWorldId(UUID request, ServerCallContext context)
        {
            WorldServer world = _worldManager.GetWorldById(request.ToGuid());
            return Task.FromResult(world.ChannelId.ToInt());
        }

        public override Task<Void> Shutdown(Name request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.Str);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.Shutdown();
            }

            return Task.FromResult(new Void());
        }

        public override Task<Bool> GetMaintenanceState(Void request, ServerCallContext context) => Task.FromResult(_maintenanceManager.GetMaintenanceMode().ToBool());

        public override Task<Void> SetMaintenanceState(Bool request, ServerCallContext context)
        {
            _maintenanceManager.SetMaintenanceMode(request.Boolean);
            return Task.FromResult(new Void());
        }

        public override Task<RegisteredWorldServer> GetPreviousChannelByAccountId(Long request, ServerCallContext context)
        {
            PlayerSession session = _sessionManager.GetByAccountId(request.Id);
            if (session == null)
            {
                return Task.FromResult(new RegisteredWorldServer());
            }

            return Task.FromResult(session.PreviousChannel.ToSerializableWorldServer().ToRegisteredWorldServer());
        }

        public override Task<RegisteredWorldServer> GetAct4ChannelInfo(Name request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worlds = _worldManager.GetWorldsByWorldGroup(request.Str);

            WorldServer world = worlds.FirstOrDefault(s => s.IsAct4);

            if (world != null)
            {
                return Task.FromResult(world.ToSerializableWorldServer().ToRegisteredWorldServer());
            }

            world = worlds.FirstOrDefault();

            return Task.FromResult(world.ToSerializableWorldServer().ToRegisteredWorldServer());
        }

        public override Task<RegisteredWorldServerResponse> RetrieveRegisteredWorldServers(Void request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worlds = _worldManager.GetWorlds();

            List<RegisteredWorldServer> serverList = worlds.Select(s => s.ToSerializableWorldServer().ToRegisteredWorldServer()).ToList();

            return Task.FromResult(new RegisteredWorldServerResponse
            {
                Servers = { serverList }
            });
        }

        public override Task<Void> UpdateFamily(UpdateFamilyRequest request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.WorldGroup);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.UpdateFamily(request.FamilyId, request.ChangeFaction);
            }

            return Task.FromResult(new Void());
        }

        public override Task<Void> UpdateRelation(UpdateRelationQuery request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.WorldGroup);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.UpdateRelation(request.RelationId);
            }

            return Task.FromResult(new Void());
        }

        public override Task<Void> UpdateBazaar(UpdateBazaarQuery request, ServerCallContext context)
        {
            IEnumerable<WorldServer> worldServers = _worldManager.GetWorldsByWorldGroup(request.WorldGroup);
            foreach (WorldServer world in worldServers)
            {
                ICommunicationClient worldClient = _communicationManager.GetCommunicationClientByWorldId(world.Id);
                worldClient.UpdateBazaar(request.BazaarItemId);
            }

            return Task.FromResult(new Void());
        }

        public override async Task<Void> Cleanup(Void request, ServerCallContext context) => await base.Cleanup(request, context);
    }
}