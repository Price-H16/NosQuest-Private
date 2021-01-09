// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("say")]
    public class SayPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        #endregion
    }
}