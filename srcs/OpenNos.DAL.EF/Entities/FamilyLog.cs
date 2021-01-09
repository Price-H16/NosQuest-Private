// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.ComponentModel.DataAnnotations;
using WingsEmu.Packets.Enums;

namespace OpenNos.DAL.EF.Entities
{
    public class FamilyLog
    {
        #region Properties

        public virtual Family Family { get; set; }

        public long FamilyId { get; set; }

        [MaxLength(255)]
        public string FamilyLogData { get; set; }

        public long FamilyLogId { get; set; }

        public FamilyLogType FamilyLogType { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}