// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Zoom", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ZoomPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Value { get; set; }

        public static string ReturnHelp() => "$Zoom VALUE";

        #endregion
    }
}