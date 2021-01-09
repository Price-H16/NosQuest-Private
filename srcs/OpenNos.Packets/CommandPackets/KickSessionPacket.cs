// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$KickSession", PassNonParseablePacket = true, Authority = AuthorityType.Moderator)]
    public class KickSessionPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string AccountName { get; set; }

        [PacketIndex(1)]
        public long? SessionId { get; set; }

        public static string ReturnHelp() => "$KickSession ACCOUNTNAME SESSIONID(?)";

        #endregion
    }
}