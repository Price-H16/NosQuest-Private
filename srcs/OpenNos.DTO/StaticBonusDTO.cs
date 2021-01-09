// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class StaticBonusDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public DateTime DateEnd { get; set; }

        public long StaticBonusId { get; set; }

        public StaticBonusType StaticBonusType { get; set; }

        #endregion
    }
}