// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$PortalTo", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class PortalToPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short DestinationMapId { get; set; }

        [PacketIndex(1)]
        public short DestinationX { get; set; }

        [PacketIndex(2)]
        public short DestinationY { get; set; }

        [PacketIndex(3)]
        public PortalType? PortalType { get; set; }

        public static string ReturnHelp() => "$PortalTo MAPID DESTX DESTY PORTALTYPE(?)";

        #endregion
    }
}