// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("withdraw")]
    public class WithDrawPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte ShopSlot { get; set; }

        [PacketIndex(1)]
        public byte InventoryId { get; set; }

        #endregion
    }
}