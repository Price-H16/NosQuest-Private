// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$MapDance", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MapDancePacket : PacketDefinition
    {
        public static string ReturnHelp() => "$MapDance";
    }
}