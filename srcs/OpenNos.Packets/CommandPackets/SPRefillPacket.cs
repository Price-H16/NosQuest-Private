// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$SPRefill", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class SpRefillPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$SPRefill";
    }
}