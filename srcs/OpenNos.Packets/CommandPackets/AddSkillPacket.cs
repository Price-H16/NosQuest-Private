// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$AddSkill", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class AddSkillPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short SkillVnum { get; set; }

        public static string ReturnHelp() => "$AddSkill SKILLVNUM";

        #endregion
    }
}