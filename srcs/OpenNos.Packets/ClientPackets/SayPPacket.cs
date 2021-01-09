// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("say_p")]
    public class SayPPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int PetId { get; set; }

        [PacketIndex(1, SerializeToEnd = true)]
        public string Message { get; set; }

        #endregion
    }
}