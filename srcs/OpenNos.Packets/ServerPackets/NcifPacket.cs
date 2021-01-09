// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("ncif")]
    public class NcifPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte ObjectType { get; set; }

        [PacketIndex(1)]
        public long ObjectId { get; set; }

        #endregion
    }
}