﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("qt")]
    public class QtPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short Type { get; set; }

        [PacketIndex(1)]
        public int Data { get; set; }

        #endregion
    }
}