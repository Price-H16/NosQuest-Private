// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$SearchMonster", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class SearchMonsterPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Name { get; set; }

        public static string ReturnHelp() => "$SearchMonster NAME(*)";

        #endregion
    }
}