// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$HairStyle", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class HairStylePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public HairStyleType HairStyle { get; set; }

        public static string ReturnHelp() => "$HairStyle STYLEID";

        #endregion
    }
}