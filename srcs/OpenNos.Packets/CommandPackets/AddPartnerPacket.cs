// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$AddPartner", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class AddPartnerPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short MonsterVNum { get; set; }

        [PacketIndex(1)]
        public byte Level { get; set; }

        #endregion

        #region Methods

        public static string ReturnHelp() => "$AddPartner MONSTERVNUM LEVEL";

        public override string ToString() => $"AddPartner Command MonsterVNum: {MonsterVNum} Level: {Level}";

        #endregion
    }
}