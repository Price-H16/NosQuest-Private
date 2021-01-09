// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ChangeClass", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeClassPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public ClassType ClassType { get; set; }

        public static string ReturnHelp() => "$ChangeClass CLASS";

        #endregion
    }
}