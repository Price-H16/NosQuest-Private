// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("csp")]
    public class CspPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long CharacterId { get; set; }

        [PacketIndex(1)]
        public string Message { get; set; }

        #endregion
    }
}