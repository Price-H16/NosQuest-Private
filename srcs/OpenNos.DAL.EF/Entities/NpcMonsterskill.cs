// WingsEmu
// 
// Developed by NosWings Team

using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF.Entities
{
    public class NpcMonsterSkill
    {
        #region Properties

        public virtual NpcMonster NpcMonster { get; set; }

        [Key]
        public long NpcMonsterSkillId { get; set; }

        public short NpcMonsterVNum { get; set; }

        public short Rate { get; set; }

        public virtual Skill Skill { get; set; }

        public short SkillVNum { get; set; }

        #endregion
    }
}