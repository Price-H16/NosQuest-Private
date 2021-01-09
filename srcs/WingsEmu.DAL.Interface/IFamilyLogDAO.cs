// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IFamilyLogDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(long familyId);

        SaveResult InsertOrUpdate(ref FamilyLogDTO famlog);

        IEnumerable<FamilyLogDTO> LoadByFamilyId(long familyId);

        #endregion
    }
}