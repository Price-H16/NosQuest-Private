// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class LogCommandsDTO : MappingBaseDTO
    {
        public long CommandId { get; set; }

        public long? CharacterId { get; set; }

        public string Command { get; set; }

        public string Data { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}