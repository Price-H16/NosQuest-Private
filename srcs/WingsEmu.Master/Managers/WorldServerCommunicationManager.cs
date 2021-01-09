// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.Communication;
using WingsEmu.Communication.RPC;

namespace WingsEmu.Master.Managers
{
    public class WorldServerCommunicationManager
    {
        private readonly ICommunicationClientFactory _clientFactory;
        private readonly WorldServerManager _worldManager;
        private readonly Dictionary<Guid, ICommunicationClient> _worldServerCommunicationClients;

        public WorldServerCommunicationManager(WorldServerManager worldManager, ICommunicationClientFactory clientFactory)
        {
            _worldManager = worldManager;
            _clientFactory = clientFactory;
            _worldServerCommunicationClients = new Dictionary<Guid, ICommunicationClient>();
        }

        public ICommunicationClient CreateCommunicationClient(Guid worldId, string communicationClientIp, int communicationClientPort)
        {
            if (!_worldManager.IsWorldRegistered(worldId))
            {
                return null;
            }

            ICommunicationClient client = _clientFactory.CreateClient(communicationClientIp, communicationClientPort);
            RegisterCommunicationClient(worldId, client);
            return client;
        }

        public void RegisterCommunicationClient(Guid id, ICommunicationClient client)
        {
            _worldServerCommunicationClients[id] = client;
        }

        public void UnregisterCommunicationClient(Guid id)
        {
            _worldServerCommunicationClients.Remove(id);
        }

        public ICommunicationClient GetCommunicationClientByWorldId(Guid worldId) => _worldServerCommunicationClients.TryGetValue(worldId, out ICommunicationClient client) ? client : null;
    }
}