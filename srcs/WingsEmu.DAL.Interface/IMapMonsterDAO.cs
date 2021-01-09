// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IMapMonsterDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult DeleteById(int mapMonsterId);

        bool DoesMonsterExist(int mapMonsterId);

        MapMonsterDTO Insert(MapMonsterDTO mapmonster);

        void Insert(IEnumerable<MapMonsterDTO> monsters);

        MapMonsterDTO LoadById(int mapMonsterId);

        IEnumerable<MapMonsterDTO> LoadFromMap(short MapId);

        #endregion

        IEnumerable<MapMonsterDTO> LoadAll();
    }
}