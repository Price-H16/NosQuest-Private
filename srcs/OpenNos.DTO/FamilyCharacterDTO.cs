// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class FamilyCharacterDTO : MappingBaseDTO
    {
        #region Properties

        public FamilyAuthority Authority { get; set; }

        public long CharacterId { get; set; }

        public string DailyMessage { get; set; }

        public int Experience { get; set; }

        public long FamilyCharacterId { get; set; }

        public long FamilyId { get; set; }

        public FamilyMemberRank Rank { get; set; }

        #endregion
    }
}