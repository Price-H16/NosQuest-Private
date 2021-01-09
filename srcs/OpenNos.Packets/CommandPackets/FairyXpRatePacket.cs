// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$FairyXpRate", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class FairyXpRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$FairyXpRate VALUE";

        #endregion
    }
}