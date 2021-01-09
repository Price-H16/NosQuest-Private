// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$AddQuest", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class AddQuestPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short QuestId { get; set; }

        public static string ReturnHelp() => "$AddQuest QUESTID";

        #endregion
    }
}