// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("say")]
    public class SayPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Message { get; set; }

        #endregion
    }
}