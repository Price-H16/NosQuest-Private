// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$XpRate", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class XpRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$XpRate VALUE";

        #endregion
    }
}