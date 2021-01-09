// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("mve")]
    public class MvePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public InventoryType InventoryType { get; set; }

        [PacketIndex(1)]
        public short Slot { get; set; }

        [PacketIndex(2)]
        public InventoryType DestinationInventoryType { get; set; }

        [PacketIndex(3)]
        public short DestinationSlot { get; set; }

        #endregion
    }
}