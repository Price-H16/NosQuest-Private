// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("rxit")]
    public class RxitPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte State { get; set; }

        #endregion
    }
}