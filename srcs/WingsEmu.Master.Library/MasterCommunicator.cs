// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Communication.RPC
{
    public class MasterCommunicator : ICommunicationService
    {
        private readonly Master.MasterClient _proxy;

        public MasterCommunicator(Master.MasterClient proxy) => _proxy = proxy;

        public bool IsAccountConnected(long accountId) => _proxy.IsAccountConnected(accountId.ToLong()).Boolean;

        public void DisconnectAccount(long accountId)
        {
            _proxy.DisconnectAccount(accountId.ToLong());
        }

        public bool ConnectAccount(Guid worldId, long accountId, long sessionId) => _proxy.ConnectAccountOnWorld(new ConnectAccountOnWorldRequest
        {
            AccountId = accountId,
            SessionId = sessionId,
            WorldId = worldId.ToUUID()
        }).Boolean;

        public void PulseAccount(long accountId)
        {
            _proxy.PulseAccount(accountId.ToLong());
        }

        public bool IsCharacterConnected(string worldGroup, long characterId) => _proxy.IsCharacterConnected(new IsCharacterConnectedRequest
        {
            CharacterId = characterId,
            WorldGroup = worldGroup
        }).Boolean;

        public bool ConnectCharacter(Guid worldId, long characterId) => _proxy.ConnectCharacter(new ConnectCharacterRequest
        {
            CharacterId = characterId,
            Id = worldId.ToUUID()
        }).Boolean;

        public void DisconnectCharacter(Guid worldId, long characterId)
        {
            _proxy.DisconnectCharacter(new DisconnectCharacterRequest
            {
                CharacterId = characterId,
                WorldId = worldId.ToUUID()
            });
        }

        public bool IsLoginPermitted(long accountId, long sessionId)
            => _proxy.IsLoginPermitted(new AccountIdAndSessionIdRequest
            {
                AccountId = accountId,
                SessionId = sessionId
            }).Boolean;

        public bool ChangeAuthority(string worldGroup, string characterName, AuthorityType authority)
            => _proxy.ChangeAuthority(new ChangeAuthorityRequest
            {
                WorldGroup = worldGroup,
                CharacterName = characterName,
                Authority = (int)authority
            }).Boolean;

        public void KickSession(long? accountId, long? sessionId)
        {
            _proxy.KickSession(new AccountIdAndSessionIdRequest
            {
                AccountId = accountId ?? 0,
                SessionId = sessionId ?? 0
            });
        }

        public void RegisterAccountLogin(long accountId, long sessionId, string accountName, string ipAddress)
        {
            _proxy.RegisterAccountLogin(new RegisterAccountLoginRequest
            {
                AccountId = accountId,
                SessionId = sessionId,
                AccountName = accountName,
                IpAddress = ipAddress
            });
        }


        public int? RegisterWorldServer(SerializableWorldServer worldServer, ICommunicationClientEndPoint endpoint) => _proxy.RegisterWorldServer(new RegisterWorldServerRequest
        {
            RegisteredWorldServerInformations = worldServer.ToRegisteredWorldServer(),
            GRPCEndPoint = endpoint.ToGRpcEndPointInformations()
        }).Id;

        public string RetrieveServerStatistics(bool online = false) => throw new NotImplementedException();

        public int? SendMessageToCharacter(SCSCharacterMessage message) => throw new NotImplementedException();

        public void SendMail(string worldGroup, MailDTO mail)
        {
        }

        public int? GetChannelIdByWorldId(Guid worldId) => _proxy.GetChannelIdByWorldId(worldId.ToUUID()).Id;

        public void Shutdown(string worldGroup)
        {
            _proxy.Shutdown(worldGroup.ToName());
        }

        public bool GetMaintenanceState() => _proxy.GetMaintenanceState(new Void()).Boolean;

        public void SetMaintenanceState(bool state)
        {
            _proxy.SetMaintenanceState(state.ToBool());
        }

        public void UnregisterWorldServer(Guid worldId)
        {
            _proxy.UnregisterWorldServer(worldId.ToUUID());
        }

        public void SetWorldServerAsInvisible(Guid worldId)
        {
            throw new NotImplementedException();
        }

        public bool IsMasterOnline() => _proxy.IsMasterOnline(new Void()).Boolean;

        public SerializableWorldServer GetPreviousChannelByAccountId(long accountId) => throw new NotImplementedException();

        public bool ConnectCrossServerAccount(Guid worldId, long accountId, int sessionId) => throw new NotImplementedException();

        public bool IsCrossServerLoginPermitted(long accountId, int sessionId) =>
            _proxy.IsCrossServerLoginPermitted(new AccountIdAndSessionIdRequest
            {
                AccountId = accountId,
                SessionId = sessionId
            }).Boolean;

        public void RegisterCrossServerLogin(long accountId, int sessionId)
        {
            _proxy.RegisterCrossServerLogin(new RegisterSessionQuery
            {
                AccountId = accountId,
                SessionId = sessionId
            });
        }

        public SerializableWorldServer GetAct4ChannelInfo(string worldGroup) => throw new NotImplementedException();

        public IEnumerable<SerializableWorldServer> RetrieveRegisteredWorldServers()
        {
            return _proxy.RetrieveRegisteredWorldServers(new Void()).Servers.Select(s => s.ToSerializableWorldServer()).ToList();
        }

        public void UpdateRelation(string worldGroup, long relationId)
        {
            _proxy.UpdateRelation(new UpdateRelationQuery
            {
                WorldGroup = worldGroup,
                RelationId = relationId
            });
        }

        public void UpdateFamily(string worldGroup, long familyId, bool changeFaction)
        {
            _proxy.UpdateFamily(new UpdateFamilyRequest
            {
                WorldGroup = worldGroup,
                FamilyId = familyId,
                ChangeFaction = changeFaction
            });
        }

        public void UpdateBazaar(string worldGroup, long bazaarItemId)
        {
            _proxy.UpdateBazaar(new UpdateBazaarQuery
            {
                WorldGroup = worldGroup,
                BazaarItemId = bazaarItemId
            });
        }

        public void Cleanup()
        {
            _proxy.Cleanup(new Void());
        }
    }
}