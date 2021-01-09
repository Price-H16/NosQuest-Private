// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class TeleporterDTO : MappingBaseDTO
    {
        #region Properties

        public short Index { get; set; }

        public TeleporterType Type { get; set; }

        public short MapId { get; set; }

        public int MapNpcId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public short TeleporterId { get; set; }

        #endregion
    }
}