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
    public class CharacterSkillDAO : SynchronizableBaseDAO<CharacterSkill, CharacterSkillDTO>, ICharacterSkillDAO
    {
        public CharacterSkillDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long characterId, short skillVNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterSkill invItem = context.CharacterSkill.FirstOrDefault(i => i.CharacterId == characterId && i.SkillVNum == skillVNum);
                    if (invItem != null)
                    {
                        context.CharacterSkill.Remove(invItem);
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

        public IEnumerable<CharacterSkillDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.CharacterSkill.Where(s => s.CharacterId == characterId).ToArray().Select(_mapper.Map<CharacterSkillDTO>);
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.CharacterSkill.Where(i => i.CharacterId == characterId).Select(c => c.Id).ToList();
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