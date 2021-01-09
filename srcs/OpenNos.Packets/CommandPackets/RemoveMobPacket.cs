// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$RemoveMob", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class RemoveMobPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$RemoveMob";
    }
}