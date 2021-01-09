// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("bldel")]
    public class BlDelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long CharacterId { get; set; }

        #endregion
    }
}