// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class RaidLogDTO : MappingBaseDTO
    {
        public long? CharacterId { get; set; }

        public long? FamilyId { get; set; }

        public long RaidId { get; set; }

        public DateTime Time { get; set; }
    }
}