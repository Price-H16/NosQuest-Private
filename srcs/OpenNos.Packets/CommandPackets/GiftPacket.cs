// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Gift", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class GiftPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public short VNum { get; set; }

        [PacketIndex(2)]
        public byte Amount { get; set; }

        [PacketIndex(3)]
        public sbyte Rare { get; set; }

        [PacketIndex(4)]
        public byte Upgrade { get; set; }

        public static string ReturnHelp() => "$Gift CHARACTERNAME(*) VNUM AMOUNT RARE UPGRADE";

        #endregion
    }
}