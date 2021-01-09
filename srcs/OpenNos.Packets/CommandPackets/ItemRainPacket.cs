// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ItemRain", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ItemRainPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short VNum { get; set; }

        [PacketIndex(1)]
        public int Amount { get; set; }

        [PacketIndex(2)]
        public int Count { get; set; }

        [PacketIndex(3)]
        public int Time { get; set; }

        public static string ReturnHelp() => "$ItemRain ITEMVNUM AMOUNT COUNT TIME";

        #endregion
    }
}