// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class DropDTO : MappingBaseDTO
    {
        #region Properties

        public int Amount { get; set; }

        public int DropChance { get; set; }

        public short DropId { get; set; }

        public short ItemVNum { get; set; }

        public short? MapTypeId { get; set; }

        public short? MonsterVNum { get; set; }

        #endregion
    }
}