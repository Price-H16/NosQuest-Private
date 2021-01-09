// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Stat", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class StatCommandPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Stat";
    }
}