// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IItemInstanceDAO : ISynchronizableBaseDAO<ItemInstanceDTO>
    {
        #region Methods

        DeleteResult DeleteFromSlotAndType(long characterId, short slot, InventoryType type);

        IEnumerable<ItemInstanceDTO> LoadByCharacterId(long characterId);

        ItemInstanceDTO LoadBySlotAndType(long characterId, short slot, InventoryType type);

        IEnumerable<ItemInstanceDTO> LoadByType(long characterId, InventoryType type);

        IList<Guid> LoadSlotAndTypeByCharacterId(long characterId);

        #endregion
    }
}