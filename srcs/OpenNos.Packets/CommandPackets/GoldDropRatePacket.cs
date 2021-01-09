// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$GoldDropRate", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class GoldDropRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$GoldDropRate VALUE";

        #endregion
    }
}