// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class ShopSkillDTO : MappingBaseDTO
    {
        #region Properties

        public int ShopId { get; set; }

        public int ShopSkillId { get; set; }

        public short SkillVNum { get; set; }

        public byte Slot { get; set; }

        public byte Type { get; set; }

        #endregion
    }
}