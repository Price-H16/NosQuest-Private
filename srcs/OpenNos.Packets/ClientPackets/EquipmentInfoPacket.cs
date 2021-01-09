﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("eqinfo")]
    public class EquipmentInfoPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1)]
        public short Slot { get; set; }

        [PacketIndex(2)]
        public byte PartnerEqSlot { get; set; }

        [PacketIndex(3)]
        public long? ShopOwnerId { get; set; }

        #endregion
    }
}