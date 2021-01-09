// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$RemovePortal", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class RemovePortalPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$RemovePortal";
    }
}