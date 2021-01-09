// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class RespawnMapTypeDTO : MappingBaseDTO
    {
        #region Properties

        public short DefaultMapId { get; set; }

        public short DefaultX { get; set; }

        public short DefaultY { get; set; }

        public string Name { get; set; }

        public long RespawnMapTypeId { get; set; }

        #endregion
    }
}