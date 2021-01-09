// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Interfaces;

namespace WingsEmu.DTOs
{
    public class BoxItemDTO : SpecialistInstanceDTO, IBoxInstance
    {
        #region Properties

        public short HoldingVNum { get; set; }

        #endregion
    }
}