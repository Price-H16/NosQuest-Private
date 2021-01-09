// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$MuteMap", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MuteMapPacket : PacketDefinition
    {
        #region Properties

        public static string ReturnHelp() => "$MuteMap";

        #endregion
    }
}