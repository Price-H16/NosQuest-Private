// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$BlockPM", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class BlockPmPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$BlockPM";
    }
}