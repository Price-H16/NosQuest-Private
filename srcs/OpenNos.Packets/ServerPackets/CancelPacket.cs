// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("cancel")]
    public class CancelPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public CancelType Type { get; set; }

        [PacketIndex(1)]
        public int TargetId { get; set; }
    }
}