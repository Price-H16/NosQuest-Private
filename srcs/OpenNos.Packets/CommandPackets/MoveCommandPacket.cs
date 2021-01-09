// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Move", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class MoveCommandPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$Move";

        public override string ToString() => "$Move";
    }
}