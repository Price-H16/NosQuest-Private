// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Backpack", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class BackpackPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Backpack";
    }
}