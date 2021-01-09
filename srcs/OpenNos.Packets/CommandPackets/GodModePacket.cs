// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$GodMode", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class GodModePacket : PacketDefinition
    {
        public static string ReturnHelp() => "$GodMode";
    }
}