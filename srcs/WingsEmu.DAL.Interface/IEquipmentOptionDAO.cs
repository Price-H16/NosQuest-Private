// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IEquipmentOptionDAO : ISynchronizableBaseDAO<EquipmentOptionDTO>
    {
        #region Methods

        IEnumerable<EquipmentOptionDTO> GetOptionsByWearableInstanceId(Guid inventoryitemId);

        DeleteResult DeleteByWearableInstanceId(Guid wearableInstanceId);

        #endregion
    }
}