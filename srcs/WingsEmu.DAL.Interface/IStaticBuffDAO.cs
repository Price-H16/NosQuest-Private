// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IStaticBonusDAO : IMappingBaseDAO
    {
        #region Methods

        /// <summary>
        ///     Inserts new object to database context
        /// </summary>
        /// <param name="staticBonus"></param>
        /// <returns></returns>
        SaveResult InsertOrUpdate(ref StaticBonusDTO staticBonus);

        /// <summary>
        ///     Loads staticBonus by characterid
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        IEnumerable<StaticBonusDTO> LoadByCharacterId(long characterId);

        void Delete(short bonusToDelete, long characterId);

        IEnumerable<short> LoadTypeByCharacterId(long characterId);

        #endregion
    }
}