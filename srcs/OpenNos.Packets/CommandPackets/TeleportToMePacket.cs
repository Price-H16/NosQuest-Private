// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$TeleportToMe", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class TeleportToMePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$TeleportToMe CHARACTERNAME";

        #endregion
    }
}