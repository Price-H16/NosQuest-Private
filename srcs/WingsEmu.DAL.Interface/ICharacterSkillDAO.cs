// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.DTOs.Character;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ICharacterSkillDAO : ISynchronizableBaseDAO<CharacterSkillDTO>
    {
        #region Methods

        DeleteResult Delete(long characterId, short skillVNum);

        IEnumerable<CharacterSkillDTO> LoadByCharacterId(long characterId);

        IEnumerable<Guid> LoadKeysByCharacterId(long characterId);

        #endregion
    }
}