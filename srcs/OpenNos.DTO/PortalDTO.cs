// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class PortalDTO : MappingBaseDTO
    {
        #region Properties

        public short DestinationMapId { get; set; }

        public short DestinationX { get; set; }

        public short DestinationY { get; set; }

        public bool IsDisabled { get; set; }

        public int PortalId { get; set; }

        public short SourceMapId { get; set; }

        public short SourceX { get; set; }

        public short SourceY { get; set; }

        public short Type { get; set; }

        #endregion
    }
}