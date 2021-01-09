// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class ShopDTO : MappingBaseDTO
    {
        #region Properties

        public int MapNpcId { get; set; }

        public byte MenuType { get; set; }

        public string Name { get; set; }

        public int ShopId { get; set; }

        public byte ShopType { get; set; }

        #endregion
    }
}