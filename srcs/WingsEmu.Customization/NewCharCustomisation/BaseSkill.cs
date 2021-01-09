// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs.Character;

namespace WingsEmu.Customization.NewCharCustomisation
{
    public class BaseSkill
    {
        public BaseSkill() => Skills = new List<CharacterSkillDTO>
        {
            new CharacterSkillDTO { CharacterId = 0, SkillVNum = 200 },
            new CharacterSkillDTO { CharacterId = 0, SkillVNum = 201 },
            new CharacterSkillDTO { CharacterId = 0, SkillVNum = 209 }
        };

        public IEnumerable<CharacterSkillDTO> Skills { get; set; }
    }
}