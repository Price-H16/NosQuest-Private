// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IFamilyDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(long familyId);

        SaveResult InsertOrUpdate(ref FamilyDTO family);

        IEnumerable<FamilyDTO> LoadAll();

        FamilyDTO LoadByCharacterId(long characterId);

        FamilyDTO LoadById(long familyId);

        FamilyDTO LoadByName(string name);

        #endregion
    }
}