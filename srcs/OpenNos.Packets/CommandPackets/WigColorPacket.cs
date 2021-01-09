// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$WigColor", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class WigColorPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Color { get; set; }

        public static string ReturnHelp() => "$WigColor COLORID";

        #endregion
    }
}