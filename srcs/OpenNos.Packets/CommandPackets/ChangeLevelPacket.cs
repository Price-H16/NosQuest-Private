// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Lvl", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Level { get; set; }

        public static string ReturnHelp() => "$Lvl LEVEL";

        #endregion
    }
}