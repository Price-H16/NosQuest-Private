// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Packet", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class PacketCallbackPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Packet { get; set; }

        public static string ReturnHelp() => "$Packet PACKET";

        #endregion
    }
}