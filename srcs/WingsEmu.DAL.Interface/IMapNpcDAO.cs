// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IMapNpcDAO : IMappingBaseDAO
    {
        #region Methods

        MapNpcDTO Insert(MapNpcDTO npc);

        void Insert(List<MapNpcDTO> npcs);

        IEnumerable<MapNpcDTO> LoadAll();

        MapNpcDTO LoadById(int MapNpcId);

        IEnumerable<MapNpcDTO> LoadFromMap(short MapId);

        #endregion
    }
}