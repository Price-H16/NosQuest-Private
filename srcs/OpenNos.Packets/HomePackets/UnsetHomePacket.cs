// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.HomePackets
{
    [PacketHeader("$UnsetHome", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class UnsetHomePacket : PacketDefinition
    {
        [PacketIndex(0)]
        public string Name { get; set; }
    }
}