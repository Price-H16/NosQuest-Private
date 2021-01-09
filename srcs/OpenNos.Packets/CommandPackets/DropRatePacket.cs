// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$DropRate", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class DropRatePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$DropRate VALUE";

        #endregion
    }
}