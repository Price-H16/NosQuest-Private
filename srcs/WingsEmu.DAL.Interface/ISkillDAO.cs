// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface ISkillDAO : IMappingBaseDAO
    {
        #region Methods

        SkillDTO Insert(SkillDTO skill);

        void Insert(List<SkillDTO> skills);

        IEnumerable<SkillDTO> LoadAll();

        SkillDTO LoadById(short SkillId);

        #endregion
    }
}