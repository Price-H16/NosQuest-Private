// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Kill", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class KillPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        public static string ReturnHelp() => "$Kill CHARACTERNAME";

        #endregion
    }
}