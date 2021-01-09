// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$SPLvl", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ChangeSpecialistLevelPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte SpecialistLevel { get; set; }

        public static string ReturnHelp() => "$SPLvl SPLEVEL";

        #endregion
    }
}