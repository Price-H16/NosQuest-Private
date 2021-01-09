// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("fdel")]
    public class FDelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long CharacterId { get; set; }

        #endregion
    }
}