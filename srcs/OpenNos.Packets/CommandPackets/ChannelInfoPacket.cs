// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ChannelInfo", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChannelInfoPacket : PacketDefinition
    {
        public static string ReturnHelp() => "$ChannelInfo";
    }
}