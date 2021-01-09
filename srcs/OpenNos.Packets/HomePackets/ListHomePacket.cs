// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.HomePackets
{
    [PacketHeader("$ListHome", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class ListHomePacket : PacketDefinition
    {
    }
}