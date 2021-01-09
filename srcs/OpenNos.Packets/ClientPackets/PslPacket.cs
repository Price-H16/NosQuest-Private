// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("psl")]
    public class PslPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public int Type { get; set; }
    }
}