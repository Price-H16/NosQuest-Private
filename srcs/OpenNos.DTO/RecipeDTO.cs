// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class RecipeDTO : MappingBaseDTO
    {
        #region Properties

        public byte Amount { get; set; }

        public short ItemVNum { get; set; }

        public int MapNpcId { get; set; }

        public short RecipeId { get; set; }

        public short ProduceItemVNum { get; set; }

        #endregion
    }
}