// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("useobj")]
    public class UseObjPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Name { get; set; }

        [PacketIndex(1)]
        public long ObjectId { get; set; }

        #endregion
    }
}