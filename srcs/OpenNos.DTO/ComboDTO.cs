// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class ComboDTO : MappingBaseDTO
    {
        #region Properties

        public short Animation { get; set; }

        public int ComboId { get; set; }

        public short Effect { get; set; }

        public short Hit { get; set; }

        public short SkillVNum { get; set; }

        #endregion
    }
}