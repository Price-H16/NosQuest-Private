// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface INpcMonsterSkillDAO : IMappingBaseDAO
    {
        #region Methods

        NpcMonsterSkillDTO Insert(ref NpcMonsterSkillDTO npcmonsterskill);

        void Insert(List<NpcMonsterSkillDTO> skills);

        List<NpcMonsterSkillDTO> LoadAll();

        IEnumerable<NpcMonsterSkillDTO> LoadByNpcMonster(short npcId);

        #endregion
    }
}