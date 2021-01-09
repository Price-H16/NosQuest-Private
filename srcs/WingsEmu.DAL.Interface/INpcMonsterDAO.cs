// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface INpcMonsterDAO : IMappingBaseDAO
    {
        #region Methods

        /// <summary>
        ///     Used for searching monster by what it contains in name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<NpcMonsterDTO> FindByName(string name);

        /// <summary>
        ///     Used for inserting list of data to entity
        /// </summary>
        /// <param name="npc"></param>
        void Insert(List<NpcMonsterDTO> npc);

        /// <summary>
        ///     Inser or Update data in entity
        /// </summary>
        /// <param name="npcMonster"></param>
        /// <returns></returns>
        SaveResult InsertOrUpdate(ref NpcMonsterDTO npcMonster);

        /// <summary>
        ///     Used for loading all monsters from entity
        /// </summary>
        /// <returns></returns>
        IEnumerable<NpcMonsterDTO> LoadAll();

        /// <summary>
        ///     Used for loading monsters with specified VNum
        /// </summary>
        /// <param name="npcMonsterVNum"></param>
        /// <returns></returns>
        NpcMonsterDTO LoadByVNum(short npcMonsterVNum);

        #endregion
    }
}