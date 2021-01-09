// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("sr")]
    public class SrPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public int CastingId { get; set; }
    }
}