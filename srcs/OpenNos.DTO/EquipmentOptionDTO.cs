// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class EquipmentOptionDTO : SynchronizableBaseDTO
    {
        #region Properties

        public Guid WearableInstanceId { get; set; }

        public byte Level { get; set; }

        public byte Type { get; set; }

        public int Value { get; set; }

        #endregion
    }
}