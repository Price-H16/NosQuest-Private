// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Teleport", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class TeleportPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Data { get; set; }

        [PacketIndex(1)]
        public short X { get; set; }

        [PacketIndex(2)]
        public short Y { get; set; }

        public static string ReturnHelp() => "$Teleport CHARACTERNAME/MAP X(?) Y(?)";

        #endregion
    }
}