// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("remove")]
    public class RemovePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte EquipSlot { get; set; }

        [PacketIndex(1)]
        public long MateId { get; set; }

        #endregion
    }
}