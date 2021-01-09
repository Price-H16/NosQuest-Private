// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("buy")]
    public class BuyPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public BuyShopType Type { get; set; }

        [PacketIndex(1)]
        public long OwnerId { get; set; }

        [PacketIndex(2)]
        public short Slot { get; set; }

        [PacketIndex(3)]
        public short Amount { get; set; }

        public override string ToString() => $"BuyShop {Type} {OwnerId} {Slot} {Amount}";

        #endregion
    }
}