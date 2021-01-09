// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Unban", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class UnbanPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$Unban CHARACTERNAME";

        #endregion
    }
}