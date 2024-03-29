﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("c_blist")]
    public class CbListPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Index { get; set; }

        [PacketIndex(1)]
        public BazaarListType TypeFilter { get; set; }

        [PacketIndex(2)]
        public byte SubTypeFilter { get; set; }

        [PacketIndex(3)]
        public byte LevelFilter { get; set; }

        [PacketIndex(4)]
        public byte RareFilter { get; set; }

        [PacketIndex(5)]
        public byte UpgradeFilter { get; set; }

        [PacketIndex(6)]
        public byte OrderFilter { get; set; }

        [PacketIndex(7)]
        public byte Unknown1 { get; set; }

        [PacketIndex(8, SerializeToEnd = true)]
        public string ItemVNumFilter { get; set; }

        #endregion
    }
}