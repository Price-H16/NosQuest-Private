﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("mz")]
    public class MzPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Ip { get; set; }

        [PacketIndex(1)]
        public short Port { get; set; }

        [PacketIndex(2)]
        public byte Slot { get; set; }

        #endregion
    }
}