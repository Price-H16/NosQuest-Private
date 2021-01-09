// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("git")]
    public class GitPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int ButtonId { get; set; }

        #endregion
    }
}