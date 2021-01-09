// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Restart", PassNonParseablePacket = true, Authority = AuthorityType.Administrator)]
    public class RestartPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Restart";
    }
}