// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class CardDTO : MappingBaseDTO
    {
        #region Properties

        public short CardId { get; set; }

        public int Duration { get; set; }

        public int EffectId { get; set; }

        public byte Level { get; set; }

        public string Name { get; set; }

        public short TimeoutBuff { get; set; }

        public BuffType BuffType { get; set; }

        public byte TimeoutBuffChance { get; set; }

        public int Delay { get; set; }

        public byte Propability { get; set; }

        #endregion
    }
}