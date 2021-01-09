﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("pst")]
    public class PstPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Argument { get; set; }

        [PacketIndex(1)]
        public int Type { get; set; }

        [PacketIndex(2)]
        public long Id { get; set; }

        [PacketIndex(3)]
        public int? Unknow1 { get; set; }

        [PacketIndex(4)]
        public int Unknow2 { get; set; }

        [PacketIndex(5)]
        public string Receiver { get; set; }

        [PacketIndex(6, SerializeToEnd = true)]
        public string Data { get; set; }

        #endregion
    }
}