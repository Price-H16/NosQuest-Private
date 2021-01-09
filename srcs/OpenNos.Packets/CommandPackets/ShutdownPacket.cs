// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Shutdown", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ShutdownPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Shutdown";
    }
}