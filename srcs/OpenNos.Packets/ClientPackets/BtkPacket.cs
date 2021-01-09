// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("btk")]
    public class BtkPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long CharacterId { get; set; }

        [PacketIndex(1, serializeToEnd: true)]
        public string Message { get; set; }

        #endregion
    }
}