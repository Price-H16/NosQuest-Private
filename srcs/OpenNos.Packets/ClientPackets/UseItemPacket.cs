// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("u_i")]
    public class UseItemPacket : PacketDefinition
    {
        [PacketIndex(2)]
        public InventoryType Type { get; set; }

        [PacketIndex(3)]
        public short Slot { get; set; }
    }
}