// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.DTOs.Character;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ICharacterQuestDAO : ISynchronizableBaseDAO<CharacterQuestDTO>
    {
        #region Methods

        DeleteResult Delete(long characterId, long questId);

        IEnumerable<CharacterQuestDTO> LoadByCharacterId(long characterId);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        #endregion
    }
}