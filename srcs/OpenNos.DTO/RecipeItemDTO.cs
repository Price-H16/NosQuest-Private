// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class RecipeItemDTO : MappingBaseDTO
    {
        #region Properties

        public short Amount { get; set; }

        public short ItemVNum { get; set; }

        public short RecipeId { get; set; }

        public short RecipeItemId { get; set; }

        #endregion
    }
}