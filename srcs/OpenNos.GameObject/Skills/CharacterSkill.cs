// WingsEmu
// 
// Developed by NosWings Team

using System;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs.Character;

namespace OpenNos.GameObject.Skills
{
    public class CharacterSkill : CharacterSkillDTO
    {
        #region Members

        private Skill _skill;

        #endregion

        #region Methods

        public bool CanBeUsed() => Skill != null && LastUse.AddMilliseconds(Skill.Cooldown * 100) < DateTime.Now;

        #endregion

        #region Instantiation

        public CharacterSkill(CharacterSkillDTO characterSkill)
        {
            CharacterId = characterSkill.CharacterId;
            Id = characterSkill.Id;
            SkillVNum = characterSkill.SkillVNum;
            LastUse = DateTime.Now.AddHours(-1);
            Hit = 0;
        }

        public CharacterSkill()
        {
            LastUse = DateTime.Now.AddHours(-1);
            Hit = 0;
        }

        #endregion

        #region Properties

        public short Hit { get; set; }

        public DateTime LastUse { get; set; }

        public Skill Skill => _skill ?? (_skill = ServerManager.Instance.GetSkill(SkillVNum));

        #endregion
    }
}