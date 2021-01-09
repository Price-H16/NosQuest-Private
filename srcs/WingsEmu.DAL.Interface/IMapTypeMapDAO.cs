// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IMapTypeMapDAO : IMappingBaseDAO
    {
        #region Methods

        void Insert(List<MapTypeMapDTO> mapTypeMaps);

        IEnumerable<MapTypeMapDTO> LoadAll();

        MapTypeMapDTO LoadByMapAndMapType(short mapId, short maptypeId);

        IEnumerable<MapTypeMapDTO> LoadByMapId(short mapId);

        IEnumerable<MapTypeMapDTO> LoadByMapTypeId(short maptypeId);

        #endregion
    }
}