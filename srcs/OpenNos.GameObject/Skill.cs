// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenNos.GameObject.Buff;
using WingsEmu.DTOs;

namespace OpenNos.GameObject
{
    public class Skill : SkillDTO
    {
        #region Instantiation

        public Skill()
        {
            Combos = new List<ComboDTO>();
            BCards = new ConcurrentBag<BCard>();
        }

        #endregion


        #region Properties

        public List<ComboDTO> Combos { get; set; }

        public ConcurrentBag<BCard> BCards { get; set; }

        #endregion
    }
}