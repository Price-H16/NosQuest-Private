// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("gop")]
    public class CharacterOptionPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public CharacterOption Option { get; set; }

        [PacketIndex(1)]
        public bool IsActive { get; set; }
    }
}