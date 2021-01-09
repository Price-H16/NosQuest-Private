// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$HeroXpRate", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class HeroXpRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$HeroXpRate VALUE";

        #endregion
    }
}