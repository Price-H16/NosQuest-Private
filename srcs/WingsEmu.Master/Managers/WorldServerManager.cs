// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.Communication;
using WingsEmu.Master.Datas;
using WingsEmu.Master.Extensions;

namespace WingsEmu.Master.Managers
{
    public class WorldServerManager
    {
        private readonly Dictionary<Guid, WorldServer> _worldServerById;
        private readonly Dictionary<string, List<WorldServer>> _worldServersByWorldGroup;

        public WorldServerManager()
        {
            _worldServersByWorldGroup = new Dictionary<string, List<WorldServer>>();
            _worldServerById = new Dictionary<Guid, WorldServer>();
        }

        public WorldServer GetWorldById(Guid worldId) => _worldServerById.TryGetValue(worldId, out WorldServer server) ? server : null;

        public bool IsWorldRegistered(Guid worldId) => _worldServerById.ContainsKey(worldId);

        public IEnumerable<WorldServer> GetWorldsByWorldGroup(string requestWorldGroup) => _worldServersByWorldGroup.TryGetValue(requestWorldGroup, out List<WorldServer> servers) ? servers : null;

        public void UnregisterWorld(Guid worldId)
        {
            if (!_worldServerById.Remove(worldId, out WorldServer value))
            {
                return;
            }

            if (!_worldServersByWorldGroup.TryGetValue(value.WorldGroup, out List<WorldServer> servers))
            {
                return;
            }

            servers.RemoveAll(s => s.Id == worldId);
        }

        public WorldServer RegisterWorldServer(SerializableWorldServer serialized)
        {
            WorldServer worldServ = serialized.ToWorldServer();
            if (_worldServerById.ContainsKey(worldServ.Id))
            {
                // already registered
                return null;
            }

            _worldServerById[worldServ.Id] = worldServ;

            if (!_worldServersByWorldGroup.TryGetValue(worldServ.WorldGroup, out List<WorldServer> servers))
            {
                servers = new List<WorldServer>();
                _worldServersByWorldGroup[worldServ.WorldGroup] = servers;
            }

            servers.Add(worldServ);
            worldServ.ChannelId = servers.Count;

            return worldServ;
        }

        public IEnumerable<WorldServer> GetWorlds()
        {
            foreach (KeyValuePair<Guid, WorldServer> keyValuePair in _worldServerById)
            {
                yield return keyValuePair.Value;
            }
        }
    }
}