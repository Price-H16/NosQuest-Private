﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("guri")]
    public class GuriPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public int Type { get; set; }

        [PacketIndex(1)]
        public int Argument { get; set; }

        [PacketIndex(2)]
        public long? User { get; set; }

        [PacketIndex(3)]
        public int Data { get; set; }

        [PacketIndex(4, serializeToEnd: true)]
        public string Value { get; set; }

        #endregion
    }
}