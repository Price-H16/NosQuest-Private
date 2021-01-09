﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("f_withdraw")]
    public class FWithdrawPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public short Slot { get; set; }

        [PacketIndex(1)]
        public byte Amount { get; set; }

        [PacketIndex(2)]
        public byte? Unknown { get; set; }

        #endregion
    }
}