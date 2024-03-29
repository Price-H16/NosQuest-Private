﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("stat")]
    public class StatPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int CurrentHp { get; set; }

        [PacketIndex(1)]
        public int MaxHp { get; set; }

        [PacketIndex(2)]
        public short CurrentMp { get; set; }

        [PacketIndex(3)]
        public short MaxMp { get; set; }

        [PacketIndex(4)]
        public byte Unknown { get; set; }

        [PacketIndex(5)]
        public short Options { get; set; }

        #endregion
    }
}