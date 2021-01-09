// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("sell")]
    public class SellPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(2)]
        public short Data { get; set; }

        [PacketIndex(3)]
        public byte? Slot { get; set; }

        [PacketIndex(4)]
        public byte? Amount { get; set; }

        #endregion
    }
}