﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("c_buy")]
    public class CBuyPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int BazaarId { get; set; }

        [PacketIndex(1)]
        public short VNum { get; set; }

        [PacketIndex(2)]
        public short Amount { get; set; }

        [PacketIndex(3)]
        public long Price { get; set; }

        #endregion
    }
}