// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Unmute", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class UnmutePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$Unmute CHARACTERNAME";

        #endregion
    }
}