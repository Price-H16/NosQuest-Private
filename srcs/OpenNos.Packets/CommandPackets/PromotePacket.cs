// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Promote", PassNonParseablePacket = true, Authority = AuthorityType.SuperGameMaster)]
    public class PromotePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public string Authority { get; set; }

        public static string ReturnHelp() => "$Promote CHARACTERNAME ROLE(WH/GM/SGM/ADMIN)";

        #endregion
    }
}