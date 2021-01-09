﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("u_s")]
    public class UseSkillPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public int CastId { get; set; }

        [PacketIndex(1)]
        public UserType UserType { get; set; }

        [PacketIndex(2)]
        public int MapMonsterId { get; set; }

        [PacketIndex(3)]
        public short? MapX { get; set; }

        [PacketIndex(4)]
        public short? MapY { get; set; }

        public override string ToString() => $"{CastId} {UserType} {MapMonsterId} {MapX} {MapY}";

        #endregion
    }
}