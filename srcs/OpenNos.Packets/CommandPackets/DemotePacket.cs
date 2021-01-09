// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Demote", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class DemotePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$Demote CHARACTERNAME";

        #endregion
    }
}