// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Resize", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ResizePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Value { get; set; }

        public static string ReturnHelp() => "$Resize VALUE";

        #endregion
    }
}