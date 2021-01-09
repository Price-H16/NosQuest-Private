﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("sl")]
    public class SpTransformPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(3)]
        public int TransportId { get; set; }

        [PacketIndex(4)]
        public short SpecialistDamage { get; set; }

        [PacketIndex(5)]
        public short SpecialistDefense { get; set; }

        [PacketIndex(6)]
        public short SpecialistElement { get; set; }

        [PacketIndex(7)]
        public short SpecialistHp { get; set; }

        #endregion
    }
}