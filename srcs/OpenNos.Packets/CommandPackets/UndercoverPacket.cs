// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Undercover", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class UndercoverPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Undercover";
    }
}