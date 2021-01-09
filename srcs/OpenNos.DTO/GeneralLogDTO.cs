// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class GeneralLogDTO : MappingBaseDTO
    {
        #region Properties

        public long? AccountId { get; set; }

        public long? CharacterId { get; set; }

        public string IpAddress { get; set; }

        public string LogData { get; set; }

        public long LogId { get; set; }

        public string LogType { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}