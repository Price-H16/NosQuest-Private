// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class PenaltyLogDTO : MappingBaseDTO
    {
        #region Properties

        public long AccountId { get; set; }

        public string AdminName { get; set; }

        public DateTime DateEnd { get; set; }

        public DateTime DateStart { get; set; }

        public PenaltyType Penalty { get; set; }

        public int PenaltyLogId { get; set; }

        public string Reason { get; set; }

        #endregion
    }
}