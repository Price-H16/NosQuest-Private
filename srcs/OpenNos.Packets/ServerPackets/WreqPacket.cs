// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("wreq")]
    public class WreqPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Value { get; set; }

        [PacketIndex(1)]
        public long? Param { get; set; }

        #endregion
    }
}