// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class RollGeneratedItemDTO : MappingBaseDTO
    {
        #region Properties

        public short RollGeneratedItemId { get; set; }

        public short OriginalItemDesign { get; set; }

        public short OriginalItemVNum { get; set; }

        public short Probability { get; set; }

        public short ItemGeneratedAmount { get; set; }

        public short ItemGeneratedVNum { get; set; }

        public bool IsRareRandom { get; set; }

        public short MinimumOriginalItemRare { get; set; }

        public short MaximumOriginalItemRare { get; set; }

        public bool IsSuperReward { get; set; }

        public byte ItemGeneratedUpgrade { get; set; }

        #endregion
    }
}