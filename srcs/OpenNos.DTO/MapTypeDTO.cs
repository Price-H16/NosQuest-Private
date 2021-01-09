// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class MapTypeDTO : MappingBaseDTO
    {
        #region Properties

        public short MapTypeId { get; set; }

        public string MapTypeName { get; set; }

        public short PotionDelay { get; set; }

        public long? RespawnMapTypeId { get; set; }

        public long? ReturnMapTypeId { get; set; }

        #endregion
    }
}