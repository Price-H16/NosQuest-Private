﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("qset")]
    public class QSetPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short Type { get; set; }

        [PacketIndex(1)]
        public short Q1 { get; set; }

        [PacketIndex(2)]
        public short Q2 { get; set; }

        [PacketIndex(3)]
        public short? Data1 { get; set; }

        [PacketIndex(4)]
        public short? Data2 { get; set; }

        #endregion
    }
}