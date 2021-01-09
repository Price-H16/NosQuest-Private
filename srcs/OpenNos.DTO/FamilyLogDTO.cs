// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class FamilyLogDTO : MappingBaseDTO
    {
        #region Properties

        public long FamilyId { get; set; }

        public string FamilyLogData { get; set; }

        public long FamilyLogId { get; set; }

        public FamilyLogType FamilyLogType { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}