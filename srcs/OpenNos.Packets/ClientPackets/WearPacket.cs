// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("wear")]
    public class WearPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte InventorySlot { get; set; }

        [PacketIndex(1)]
        public byte Type { get; set; }

        #endregion
    }
}