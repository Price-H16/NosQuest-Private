// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs.Character;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ICharacterRelationDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(long characterrelationid);

        SaveResult InsertOrUpdate(ref CharacterRelationDTO relation);

        IEnumerable<CharacterRelationDTO> LoadAll();

        CharacterRelationDTO LoadById(long relId);

        #endregion
    }
}