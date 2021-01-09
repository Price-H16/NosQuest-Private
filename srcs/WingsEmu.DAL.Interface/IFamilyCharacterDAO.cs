// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IFamilyCharacterDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(string characterName);

        SaveResult InsertOrUpdate(ref FamilyCharacterDTO character);

        FamilyCharacterDTO LoadByCharacterId(long characterId);

        IList<FamilyCharacterDTO> LoadByFamilyId(long familyId);

        FamilyCharacterDTO LoadById(long familyCharacterId);

        #endregion
    }
}