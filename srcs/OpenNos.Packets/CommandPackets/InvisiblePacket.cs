// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Invisible", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class InvisiblePacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Invisible";
    }
}