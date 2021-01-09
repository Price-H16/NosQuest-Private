// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Guri", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class GuriCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1)]
        public byte Argument { get; set; }

        [PacketIndex(2)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$Guri TYPE ARGUMENT VALUE";

        #endregion
    }
}