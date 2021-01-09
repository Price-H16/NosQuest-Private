// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IStaticBuffDAO : IMappingBaseDAO
    {
        #region Methods

        /// <summary>
        ///     Inserts new object to database context
        /// </summary>
        /// <param name="staticBonus"></param>
        /// <returns></returns>
        SaveResult InsertOrUpdate(ref StaticBuffDTO staticBuff);

        /// <summary>
        ///     Loads staticBonus by characterid
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        IEnumerable<StaticBuffDTO> LoadByCharacterId(long characterId);

        IEnumerable<short> LoadByTypeCharacterId(long characterId);

        void Delete(short bonusToDelete, long characterId);

        #endregion
    }
}