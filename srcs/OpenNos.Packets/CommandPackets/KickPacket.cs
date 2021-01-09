// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Kick", PassNonParseablePacket = true, Authority = AuthorityType.Moderator)]
    public class KickPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$Kick CHARACTERNAME";

        #endregion
    }
}