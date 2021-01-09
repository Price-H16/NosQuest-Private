// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class StaticBuffDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public int RemainingTime { get; set; }

        public long StaticBuffId { get; set; }

        public short CardId { get; set; }

        #endregion
    }
}