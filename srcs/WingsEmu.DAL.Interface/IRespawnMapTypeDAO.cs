// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IRespawnMapTypeDAO : IMappingBaseDAO
    {
        #region Methods

        void Insert(List<RespawnMapTypeDTO> respawnmaptypemaps);

        SaveResult InsertOrUpdate(ref RespawnMapTypeDTO respawnMapType);

        RespawnMapTypeDTO LoadById(long respawnMapTypeId);

        RespawnMapTypeDTO LoadByMapId(short mapId);

        #endregion

        IEnumerable<RespawnMapTypeDTO> LoadAll();
    }
}