// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ChangeDignity", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeDignityPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public float Dignity { get; set; }

        public static string ReturnHelp() => "$ChangeDignity AMOUNT";

        #endregion
    }
}