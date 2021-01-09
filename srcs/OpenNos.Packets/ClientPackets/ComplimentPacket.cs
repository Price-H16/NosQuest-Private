// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("compl")]
    public class ComplimentPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(1)]
        public long CharacterId { get; set; }

        #endregion
    }
}