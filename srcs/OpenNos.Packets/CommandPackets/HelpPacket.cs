// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Help", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class HelpPacket : PacketDefinition
    {
    }
}