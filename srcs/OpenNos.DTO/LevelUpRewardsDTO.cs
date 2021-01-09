// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class LevelUpRewardsDTO : MappingBaseDTO
    {
        public long Id { get; set; }

        public short Level { get; set; }

        public short JobLevel { get; set; }

        public short HeroLvl { get; set; }

        public short Vnum { get; set; }

        public short Amount { get; set; }

        public bool IsMate { get; set; }

        public short MateLevel { get; set; }

        public MateType MateType { get; set; }
    }
}