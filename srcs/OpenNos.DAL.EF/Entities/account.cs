// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WingsEmu.Packets.Enums;

namespace OpenNos.DAL.EF.Entities
{
    public class Account
    {
        #region Instantiation

        public Account()
        {
            Character = new HashSet<Character>();
            GeneralLog = new HashSet<GeneralLog>();
            PenaltyLog = new HashSet<PenaltyLog>();
        }

        #endregion

        #region Properties

        public long AccountId { get; set; }

        public AuthorityType Authority { get; set; }

        public long BankMoney { get; set; }

        public virtual ICollection<Character> Character { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        public virtual ICollection<GeneralLog> GeneralLog { get; set; }

        public long Money { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        public virtual ICollection<PenaltyLog> PenaltyLog { get; set; }

        [MaxLength(45)]
        public string RegistrationIP { get; set; }

        [MaxLength(32)]
        public string VerificationToken { get; set; }

        #endregion
    }
}