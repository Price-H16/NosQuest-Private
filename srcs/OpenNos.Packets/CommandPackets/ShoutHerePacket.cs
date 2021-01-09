// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ShoutHere", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ShoutHerePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        public static string ReturnHelp() => "$ShoutHere MESSAGE";

        #endregion
    }
}