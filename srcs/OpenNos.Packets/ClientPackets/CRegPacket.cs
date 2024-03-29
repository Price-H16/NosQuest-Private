﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("c_reg")]
    public class CRegPacket : PacketDefinition
    {
        //c_reg 0 1 0 9 1 4 1 99 63 90 2

        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        [PacketIndex(1)]
        public byte Inventory { get; set; }

        [PacketIndex(2)]
        public byte Slot { get; set; }

        [PacketIndex(3)]
        public int Unknown1 { get; set; }

        [PacketIndex(4)]
        public int Unknown2 { get; set; }

        [PacketIndex(5)]
        public byte Durability { get; set; }

        [PacketIndex(6)]
        public int IsPackage { get; set; }

        [PacketIndex(7)]
        public short Amount { get; set; }

        [PacketIndex(8)]
        public long Price { get; set; }

        [PacketIndex(9)]
        public int Taxe { get; set; }

        [PacketIndex(10)]
        public byte MedalUsed { get; set; }

        #endregion
    }
}