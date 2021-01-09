// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$SummonNPC", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class SummonNpcPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short NpcMonsterVNum { get; set; }

        [PacketIndex(1)]
        public byte Amount { get; set; }

        [PacketIndex(2)]
        public bool IsMoving { get; set; }

        public static string ReturnHelp() => "$SummonNPC MONSTERVNUM AMOUNT MOVE";

        #endregion
    }
}