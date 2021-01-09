// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class BazaarItemDTO : MappingBaseDTO
    {
        #region Properties

        public ushort Amount { get; set; }

        public long BazaarItemId { get; set; }

        public DateTime DateStart { get; set; }

        public short Duration { get; set; }

        public bool IsPackage { get; set; }

        public Guid ItemInstanceId { get; set; }

        public bool MedalUsed { get; set; }

        public long Price { get; set; }

        public long SellerId { get; set; }

        #endregion
    }
}