// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("vb")]
    public class VbPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte VbType { get; set; }

        [PacketIndex(1)]
        public byte Unknown { get; set; }

        [PacketIndex(2)]
        public long VbBuffTime { get; set; }

        #endregion
    }
}