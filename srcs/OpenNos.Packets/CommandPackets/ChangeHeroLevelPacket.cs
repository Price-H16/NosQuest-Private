// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$HeroLvl", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeHeroLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte HeroLevel { get; set; }

        public static string ReturnHelp() => "$HeroLvl HEROLEVEL";

        #endregion
    }
}