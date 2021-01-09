// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class MapMonsterDTO : MappingBaseDTO
    {
        #region Properties

        public bool IsDisabled { get; set; }

        public bool IsMoving { get; set; }

        public short MapId { get; set; }

        public int MapMonsterId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public short MonsterVNum { get; set; }

        public byte Position { get; set; }

        #endregion
    }
}