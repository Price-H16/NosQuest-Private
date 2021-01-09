// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Summon", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class SummonPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short NpcMonsterVNum { get; set; }

        [PacketIndex(1)]
        public byte Amount { get; set; }

        [PacketIndex(2)]
        public bool IsMoving { get; set; }

        public static string ReturnHelp() => "$Summon MONSTERVNUM AMOUNT MOVE";

        #endregion
    }
}