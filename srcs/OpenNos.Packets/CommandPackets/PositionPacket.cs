// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Position", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class PositionPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Position";
    }
}