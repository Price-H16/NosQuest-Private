// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IMateDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(long id);

        SaveResult InsertOrUpdate(ref MateDTO mate);

        IEnumerable<MateDTO> LoadByCharacterId(long characterId);

        #endregion
    }
}