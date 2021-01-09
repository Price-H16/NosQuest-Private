﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("r_info")]
    public class RInfoPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short? VNum { get; set; }

        [PacketIndex(1)]
        public short? Slot { get; set; }

        #endregion
    }
}