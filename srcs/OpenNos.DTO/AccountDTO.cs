// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class AccountDTO : MappingBaseDTO
    {
        #region Properties

        public long AccountId { get; set; }

        public AuthorityType Authority { get; set; }

        public long BankMoney { get; set; }

        public string Email { get; set; }

        public long Money { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string RegistrationIP { get; set; }

        public string VerificationToken { get; set; }

        #endregion
    }
}