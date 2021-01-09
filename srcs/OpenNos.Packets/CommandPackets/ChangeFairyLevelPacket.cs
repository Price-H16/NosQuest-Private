// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$FLvl", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeFairyLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short FairyLevel { get; set; }

        public static string ReturnHelp() => "$FLvl FAIRYLEVEL";

        #endregion
    }
}