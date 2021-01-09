// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("remove")]
    public class RemovePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte InventorySlot { get; set; }

        [PacketIndex(1)]
        public byte Type { get; set; }

        #endregion
    }
}