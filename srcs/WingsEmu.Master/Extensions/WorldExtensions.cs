// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Linq;
using WingsEmu.Communication;
using WingsEmu.Master.Datas;

namespace WingsEmu.Master.Extensions
{
    internal static class WorldExtensions
    {
        public static int CountVisibleServer(this List<WorldServer> servers)
        {
            return servers.Count(s => !s.IsAct4 && s.IsInvisible);
        }

        public static WorldServer ToWorldServer(this SerializableWorldServer serialized) =>
            new WorldServer
            {
                WorldGroup = serialized.WorldGroup,
                Id = serialized.Id,
                ChannelId = serialized.ChannelId,
                AccountLimit = serialized.AccountLimit,
                Port = serialized.EndPointPort,
                Endpoint = serialized.EndPointIp
            };

        public static SerializableWorldServer ToSerializableWorldServer(this WorldServer world) =>
            new SerializableWorldServer
            {
                WorldGroup = world.WorldGroup,
                Id = world.Id,
                EndPointIp = world.Endpoint,
                EndPointPort = world.Port,
                ChannelId = world.ChannelId,
                AccountLimit = world.AccountLimit
            };
    }
}