﻿// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("equip")]
    public class EquipPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte WeaponArmourUpgrade { get; set; }

        [PacketIndex(1)]
        public byte Design { get; set; }

        [PacketIndex(2)]
        public List<EquipSubPacket> EquipEntries { get; set; }

        #endregion
    }

    [PacketHeader("sub_equipment")] // actually no header rendered, avoid error
    public class EquipSubPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Index { get; set; }

        [PacketIndex(1)]
        public int ItemVNum { get; set; }

        [PacketIndex(2)]
        public byte Rare { get; set; }

        [PacketIndex(4)]
        public byte Unknown { get; set; }

        [PacketIndex(3)]
        public byte Upgrade { get; set; }

        #endregion
    }
}