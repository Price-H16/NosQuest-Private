// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.HomePackets
{
    [PacketHeader("$Home", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class HomePacket : PacketDefinition
    {
        [PacketIndex(0)]
        public string Name { get; set; }
    }
}