// WingsEmu
// 
// Developed by NosWings Team

using System;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;

namespace OpenNos.GameObject.Skills
{
    public class NpcMonsterSkill : NpcMonsterSkillDTO
    {
        #region Members

        private Skill _skill;

        #endregion
        
        #region Properties

        public short Hit { get; set; } = 0;

        public DateTime LastSkillUse { get; set; } = DateTime.Now.AddHours(-1);

        public Skill Skill => _skill ?? (_skill = ServerManager.Instance.GetSkill(SkillVNum));

        #endregion
    }
}