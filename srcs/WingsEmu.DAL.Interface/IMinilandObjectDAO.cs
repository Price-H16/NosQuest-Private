// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IMinilandObjectDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult DeleteById(long id);

        SaveResult InsertOrUpdate(ref MinilandObjectDTO obj);

        IEnumerable<MinilandObjectDTO> LoadByCharacterId(long characterId);

        #endregion
    }
}