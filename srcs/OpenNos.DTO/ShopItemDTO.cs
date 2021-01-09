// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class ShopItemDTO : MappingBaseDTO
    {
        #region Properties

        public byte Color { get; set; }

        public short ItemVNum { get; set; }

        public sbyte Rare { get; set; }

        public int ShopId { get; set; }

        public int ShopItemId { get; set; }

        public byte Slot { get; set; }

        public byte Type { get; set; }

        public byte Upgrade { get; set; }

        #endregion
    }
}