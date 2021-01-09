// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("eq")]
    public class EqPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public long Id { get; set; }
    }

    [PacketHeader("eqsub")]
    public class EqSubPacket : PacketDefinition
    {
    }

    [PacketHeader("eqraresub")]
    public class EqRareSubPacket : PacketDefinition
    {
    }
}