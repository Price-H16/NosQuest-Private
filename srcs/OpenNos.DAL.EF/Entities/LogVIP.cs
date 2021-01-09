// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class LogVip
    {
        #region Properties

        [Key]
        public long LogId { get; set; }

        public virtual Account Account { get; set; }

        public long? AccountId { get; set; }

        public virtual Character Character { get; set; }

        public long? CharacterId { get; set; }

        public DateTime Timestamp { get; set; }

        public string VipPack { get; set; }

        #endregion
    }
}