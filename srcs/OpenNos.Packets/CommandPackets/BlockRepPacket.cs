// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$BlockRep", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class BlockRepPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$BlockRep CHARACTERNAME DURATION REASON";

        public override string ToString() => $"BlockRep Command CharacterName: {CharacterName} Duration: {Duration} Reason: {Reason}";

        #endregion
    }
}