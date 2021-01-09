// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.DTOs.Interfaces;

namespace WingsEmu.DTOs
{
    public class MapDTO : MappingBaseDTO, IMapDTO
    {
        #region Properties

        public byte[] Data { get; set; }

        public short MapId { get; set; }

        public int Music { get; set; }

        public string Name { get; set; }

        public bool ShopAllowed { get; set; }

        #endregion
    }
}