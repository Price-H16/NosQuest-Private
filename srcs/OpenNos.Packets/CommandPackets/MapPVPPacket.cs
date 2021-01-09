// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$MapPVP", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MapPvpPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$MapPVP";
    }
}