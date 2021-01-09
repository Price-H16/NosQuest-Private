// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("pdtse")]
    public class PdtsePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1)]
        public short VNum { get; set; }

        #endregion
    }
}