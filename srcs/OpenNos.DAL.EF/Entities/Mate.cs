﻿// WingsEmu
// 
// Developed by NosWings Team

using System.ComponentModel.DataAnnotations;
using WingsEmu.Packets.Enums;

namespace OpenNos.DAL.EF.Entities
{
    public class Mate
    {
        #region Properties

        public byte Attack { get; set; }

        public bool CanPickUp { get; set; }

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public byte Defence { get; set; }

        public byte Direction { get; set; }

        public long Experience { get; set; }

        public int Hp { get; set; }

        public bool IsSummonable { get; set; }

        public bool IsTeamMember { get; set; }

        public byte Level { get; set; }

        public short Loyalty { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        [Key]
        public long MateId { get; set; }

        public MateType MateType { get; set; }

        public int Mp { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public virtual NpcMonster NpcMonster { get; set; }

        public short NpcMonsterVNum { get; set; }

        public short Skin { get; set; }

        #endregion
    }
}