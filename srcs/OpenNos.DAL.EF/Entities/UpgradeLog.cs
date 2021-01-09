// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class UpgradeLog
    {
        #region Instantiation

        #endregion

        #region Properties

        [Key]
        public long Id { get; set; }

        public long AccountId { get; set; }

        public long CharacterId { get; set; }

        public string CharacterName { get; set; }

        public string UpgradeType { get; set; }

        public bool? HasAmulet { get; set; }

        public DateTime Date { get; set; }

        public bool Success { get; set; }

        public short ItemVnum { get; set; }

        public string ItemName { get; set; }

        #endregion
    }
}