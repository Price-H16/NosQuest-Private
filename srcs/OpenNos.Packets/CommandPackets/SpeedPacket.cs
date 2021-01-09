// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Speed", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class SpeedPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Value { get; set; }

        public static string ReturnHelp() => "$Speed SPEED";

        #endregion
    }
}