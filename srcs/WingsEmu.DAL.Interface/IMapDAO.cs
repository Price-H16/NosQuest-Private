// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IMapDAO : IMappingBaseDAO
    {
        #region Methods

        MapDTO Insert(MapDTO map);

        void Insert(List<MapDTO> maps);

        IEnumerable<MapDTO> LoadAll();

        MapDTO LoadById(short mapId);

        #endregion
    }
}