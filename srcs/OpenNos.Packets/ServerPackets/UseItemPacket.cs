﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("u_i")]
    public class UseItemPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte ObjectType { get; set; }

        [PacketIndex(1)]
        public long ObjectId { get; set; }

        [PacketIndex(2)]
        public byte Inventory { get; set; }

        [PacketIndex(3)]
        public byte InventorySlot { get; set; }

        [PacketIndex(4)]
        public byte Unknown { get; set; }

        [PacketIndex(5)]
        public byte Unknown1 { get; set; }

        #endregion
    }
}