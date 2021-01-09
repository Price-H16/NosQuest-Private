// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Shout", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ShoutPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        public static string ReturnHelp() => "$Shout MESSAGE";

        #endregion
    }
}