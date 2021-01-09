// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$CharStat", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class CharacterStatsPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$CharStat CHARACTERNAME";

        #endregion
    }
}