// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs.Character
{
    public class CharacterSkillDTO : SynchronizableBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public short SkillVNum { get; set; }

        #endregion
    }
}