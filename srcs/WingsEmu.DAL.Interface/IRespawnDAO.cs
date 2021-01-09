// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IRespawnDAO : IMappingBaseDAO
    {
        #region Methods

        SaveResult InsertOrUpdate(ref RespawnDTO respawn);

        IEnumerable<RespawnDTO> LoadByCharacter(long characterId);

        RespawnDTO LoadById(long respawnId);

        #endregion
    }
}