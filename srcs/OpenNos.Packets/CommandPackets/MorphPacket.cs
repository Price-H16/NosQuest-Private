// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Morph", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class MorphPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short MorphId { get; set; }

        [PacketIndex(1)]
        public byte Upgrade { get; set; }

        [PacketIndex(2)]
        public byte MorphDesign { get; set; }

        [PacketIndex(3)]
        public int ArenaWinner { get; set; }

        public static string ReturnHelp() => "$Morph MORPHID UPGRADE WINGS ARENA";

        #endregion
    }
}