// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("rmvobj")]
    public class RmvobjPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short Slot { get; set; }

        #endregion
    }
}