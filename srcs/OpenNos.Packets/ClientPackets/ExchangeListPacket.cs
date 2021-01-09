// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("exc_list")]
    public class ExchangeListPacket
    {
        [PacketIndex(1)]
        public long CharacterId { get; set; }

        [PacketIndex(2)]
        public long Gold { get; set; }
    }
}