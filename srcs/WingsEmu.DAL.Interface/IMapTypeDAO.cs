// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IMapTypeDAO : IMappingBaseDAO
    {
        #region Methods

        MapTypeDTO Insert(ref MapTypeDTO mapType);

        IEnumerable<MapTypeDTO> LoadAll();

        MapTypeDTO LoadById(short maptypeId);

        #endregion
    }
}