// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader(":")]
    public class FamilyChatPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, serializeToEnd: true)]
        public string Message { get; set; }

        #endregion
    }
}