// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class RespawnDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public short MapId { get; set; }

        public long RespawnId { get; set; }

        public long RespawnMapTypeId { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        #endregion
    }
}