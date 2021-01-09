// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$GoldRate", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class GoldRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$GoldRate VALUE";

        #endregion
    }
}