// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL.EF.DB;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs.Base;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs.Character;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class CharacterQuestDAO : SynchronizableBaseDAO<CharacterQuest, CharacterQuestDTO>, ICharacterQuestDAO
    {
        public CharacterQuestDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long characterId, long questId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterQuest charQuest = context.CharacterQuest.FirstOrDefault(i => i.CharacterId == characterId && i.QuestId == questId);
                    if (charQuest != null)
                    {
                        context.CharacterQuest.Remove(charQuest);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public IEnumerable<CharacterQuestDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (CharacterQuest entity in context.CharacterQuest.Where(i => i.CharacterId == characterId))
                {
                    yield return _mapper.Map<CharacterQuestDTO>(entity);
                }
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.CharacterQuest.Where(i => i.CharacterId == characterId).Select(c => c.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        #endregion
    }
}