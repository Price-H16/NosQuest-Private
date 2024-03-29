﻿// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("bf")]
    public class BfPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1)]
        public long CharacterId { get; set; }

        [PacketIndex(2)]
        public List<BuffSubPacket> BuffEntries { get; set; }

        [PacketIndex(3)]
        public int CharacterLevel { get; set; }
    }

    [PacketHeader("sub_buff")]
    public class BuffSubPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public int Value { get; set; } // uses left of buff, used for example for rarifying

        [PacketIndex(1)]
        public int BuffId { get; set; }

        [PacketIndex(2)]
        public int Duration { get; set; } // divided by 10
    }
}