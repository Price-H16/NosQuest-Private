// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("taw")]
    public class TawPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Username { get; set; }

        #endregion
    }
}