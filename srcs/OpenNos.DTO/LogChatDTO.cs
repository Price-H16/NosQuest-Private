// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class LogChatDTO : MappingBaseDTO
    {
        public long LogId { get; set; }

        public long? CharacterId { get; set; }

        public byte ChatType { get; set; }

        public string ChatMessage { get; set; }

        public string IpAddress { get; set; }

        public DateTime Timestamp { get; set; }
    }
}