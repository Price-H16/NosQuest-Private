// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class LogVIPDTO : MappingBaseDTO
    {
        public long LogId { get; set; }

        public long? AccountId { get; set; }

        public DateTime Timestamp { get; set; }

        public string VipPack { get; set; }
    }
}