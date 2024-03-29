﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("c_scalc")]
    public class CScalcPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public long BazaarId { get; set; }

        [PacketIndex(1)]
        public short VNum { get; set; }

        [PacketIndex(2)]
        public short Amount { get; set; }

        [PacketIndex(3)]
        public short MaxAmount { get; set; }

        [PacketIndex(4)]
        public long Price { get; set; }

        #endregion
    }
}