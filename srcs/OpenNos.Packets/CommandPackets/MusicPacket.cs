// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Music", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MusicPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Music { get; set; }

        public static string ReturnHelp() => "$Music BGM";

        #endregion
    }
}