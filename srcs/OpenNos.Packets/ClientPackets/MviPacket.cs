// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("mvi")]
    public class MviPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public InventoryType InventoryType { get; set; }

        [PacketIndex(1)]
        public short Slot { get; set; }

        [PacketIndex(2)]
        public short Amount { get; set; }

        [PacketIndex(3)]
        public byte DestinationSlot { get; set; }

        #endregion
    }
}