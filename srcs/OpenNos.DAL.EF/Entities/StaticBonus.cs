// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.Packets.Enums;

namespace OpenNos.DAL.EF.Entities
{
    public class StaticBonus
    {
        #region Properties

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        public DateTime DateEnd { get; set; }

        public long StaticBonusId { get; set; }

        public StaticBonusType StaticBonusType { get; set; }

        #endregion
    }
}