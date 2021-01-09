// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Bot", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class BotPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public long Identificator { get; set; }
    }
}