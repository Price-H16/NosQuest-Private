// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("lbs")]
    public class LbsPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        #endregion
    }
}