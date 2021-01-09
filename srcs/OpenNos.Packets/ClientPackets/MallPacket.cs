// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("mall")]
    public class MallPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        #endregion
    }
}