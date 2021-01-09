// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Gold", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class GoldPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long Amount { get; set; }

        public static string ReturnHelp() => "$Gold AMOUNT";

        #endregion
    }
}