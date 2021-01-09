// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ChangeSex", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeSexPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$ChangeSex";
    }
}