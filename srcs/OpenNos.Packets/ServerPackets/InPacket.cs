﻿// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("in")]
    public class InPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public InType InType { get; set; }

        [PacketIndex(1)]
        public string Name { get; set; }

        [PacketIndex(2)]
        public string UneccesaryNameOffset { get; set; }

        [PacketIndex(3)]
        public long Id { get; set; }

        [PacketIndex(4)]
        public short MapX { get; set; }

        [PacketIndex(5)]
        public short MapY { get; set; }

        [PacketIndex(6)]
        public byte Direction { get; set; }

        [PacketIndex(7)]
        public byte Authority { get; set; }

        [PacketIndex(8)]
        public byte Gender { get; set; }

        [PacketIndex(9)]
        public byte Hairstyle { get; set; }

        [PacketIndex(10)]
        public byte Color { get; set; }

        [PacketIndex(11)]
        public ClassType Class { get; set; }

        [PacketIndex(12)]
        public List<int?> Equipments { get; set; }

        [PacketIndex(13)]
        public int CurrentHp { get; set; }

        [PacketIndex(14)]
        public int CurrentMp { get; set; }

        [PacketIndex(15)]
        public bool IsSitting { get; set; }

        [PacketIndex(16)]
        public bool? HasGroup { get; set; }

        [PacketIndex(17)]
        public byte? FairyMovement { get; set; }

        [PacketIndex(18)]
        public byte FairyElement { get; set; }

        [PacketIndex(20)]
        public byte FairyMorph { get; set; }

        [PacketIndex(22)]
        public byte Morph { get; set; }

        [PacketIndex(23)]
        public byte? WeaponUpgradeRarity { get; set; }

        [PacketIndex(24)]
        public byte? ArmourUpgradeRarity { get; set; }

        [PacketIndex(25)]
        public long? FamilyId { get; set; }

        [PacketIndex(26)]
        public string FamilyName { get; set; }

        [PacketIndex(27)]
        public byte Icon { get; set; }

        [PacketIndex(28)]
        public bool IsInvisible { get; set; }

        [PacketIndex(29)]
        public byte MorphUpgrade { get; set; }

        [PacketIndex(31)]
        public byte MorphUpgrade2 { get; set; }

        [PacketIndex(32)]
        public byte Level { get; set; }

        [PacketIndex(33)]
        public byte FamilyLevel { get; set; }

        [PacketIndex(34)]
        public int ArenaWinner { get; set; }

        [PacketIndex(35)]
        public short Compliment { get; set; }

        [PacketIndex(36)]
        public int Size { get; set; }

        [PacketIndex(37)]
        public byte HeroLevel { get; set; }

        #endregion
    }
}