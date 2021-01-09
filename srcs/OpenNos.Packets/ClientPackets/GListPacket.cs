// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("glist")]
    public class GListPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(1)]
        public byte Type { get; set; }

        #endregion
    }
}