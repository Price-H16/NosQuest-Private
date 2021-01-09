// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$HairColor", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class HairColorPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public HairColorType HairColor { get; set; }

        public static string ReturnHelp() => "$HairColor COLORID";

        #endregion
    }
}