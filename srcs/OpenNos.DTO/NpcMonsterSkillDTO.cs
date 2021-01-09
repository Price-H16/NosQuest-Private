// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class NpcMonsterSkillDTO : MappingBaseDTO
    {
        #region Properties

        public long NpcMonsterSkillId { get; set; }

        public short NpcMonsterVNum { get; set; }

        public short Rate { get; set; }

        public short SkillVNum { get; set; }

        #endregion
    }
}