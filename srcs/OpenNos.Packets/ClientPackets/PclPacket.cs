// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("pcl")]
    public class PclPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        [PacketIndex(1)]
        public int Unknown { get; set; }

        #endregion
    }
}