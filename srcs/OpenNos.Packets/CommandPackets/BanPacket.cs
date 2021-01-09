// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Ban", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class BanPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$Ban CHARACTERNAME DURATION(DAYS) REASON";

        public override string ToString() => $"Ban Command CharacterName: {CharacterName} Duration: {Duration} Reason: {Reason}";

        #endregion
    }
}