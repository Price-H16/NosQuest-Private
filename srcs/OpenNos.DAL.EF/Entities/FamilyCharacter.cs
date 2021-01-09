// WingsEmu
// 
// Developed by NosWings Team

using System.ComponentModel.DataAnnotations;
using WingsEmu.Packets.Enums;

namespace OpenNos.DAL.EF.Entities
{
    public class FamilyCharacter
    {
        #region Properties

        public FamilyAuthority Authority { get; set; }

        public virtual Character Character { get; set; }

        public long CharacterId { get; set; }

        [MaxLength(255)]
        public string DailyMessage { get; set; }

        public int Experience { get; set; }

        public virtual Family Family { get; set; }

        public long FamilyCharacterId { get; set; }

        public long FamilyId { get; set; }

        public FamilyMemberRank Rank { get; set; }

        #endregion
    }
}