﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("mv")]
    public class MovePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte MoveType { get; set; }

        [PacketIndex(1)]
        public long CharacterId { get; set; }

        [PacketIndex(2)]
        public short MapX { get; set; }

        [PacketIndex(3)]
        public short MapY { get; set; }

        [PacketIndex(4)]
        public byte Speed { get; set; }

        #endregion
    }
}