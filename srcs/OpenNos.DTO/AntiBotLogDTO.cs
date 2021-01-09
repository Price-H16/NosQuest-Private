// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class AntiBotLogDTO : MappingBaseDTO
    {
        public long Id { get; set; }

        public long CharacterId { get; set; }

        public string CharacterName { get; set; }

        public bool Timeout { get; set; }

        public DateTime DateTime { get; set; }
    }
}