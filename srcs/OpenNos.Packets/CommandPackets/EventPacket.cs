// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Event", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class EventPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public EventType EventType { get; set; }

        [PacketIndex(1)]
        public int? UseTimer { get; set; }

        public static string ReturnHelp() => "$Event EVENT USETIMER";

        #endregion
    }
}