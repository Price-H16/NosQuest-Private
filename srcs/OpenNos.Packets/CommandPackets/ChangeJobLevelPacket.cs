// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$JLvl", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeJobLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte JobLevel { get; set; }

        public static string ReturnHelp() => "$JLvl JOBLEVEL";

        #endregion
    }
}