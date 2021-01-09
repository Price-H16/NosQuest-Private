// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IQuicklistEntryDAO : ISynchronizableBaseDAO<QuicklistEntryDTO>
    {
        #region Methods

        IEnumerable<QuicklistEntryDTO> LoadByCharacterId(long characterId);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        #endregion
    }
}