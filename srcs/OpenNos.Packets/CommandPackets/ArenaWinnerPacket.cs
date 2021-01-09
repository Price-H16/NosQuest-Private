// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ArenaWinner", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ArenaWinner : PacketDefinition
    {
        public static string ReturnHelp() => "$ArenaWinner";
    }
}