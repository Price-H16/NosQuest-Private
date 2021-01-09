// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class QuestLogDTO : MappingBaseDTO
    {
        public long CharacterId { get; set; }

        public long QuestId { get; set; }

        public string IpAddress { get; set; }

        public DateTime? LastDaily { get; set; }
    }
}