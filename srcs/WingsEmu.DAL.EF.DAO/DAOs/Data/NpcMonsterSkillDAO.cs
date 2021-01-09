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
using WingsEmu.DTOs;

namespace WingsEmu.DAL.EF.DAO.DAOs.Data
{
    public class NpcMonsterSkillDAO : MappingBaseDao<NpcMonsterSkill, NpcMonsterSkillDTO>, INpcMonsterSkillDAO
    {
        public NpcMonsterSkillDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public NpcMonsterSkillDTO Insert(ref NpcMonsterSkillDTO npcMonsterskill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<NpcMonsterSkill>(npcMonsterskill);
                    context.NpcMonsterSkill.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<NpcMonsterSkillDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public void Insert(List<NpcMonsterSkillDTO> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (NpcMonsterSkillDTO Skill in skills)
                    {
                        var entity = _mapper.Map<NpcMonsterSkill>(Skill);
                        context.NpcMonsterSkill.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public List<NpcMonsterSkillDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.NpcMonsterSkill.ToList().Select(n => _mapper.Map<NpcMonsterSkillDTO>(n)).ToList();
            }
        }

        public IEnumerable<NpcMonsterSkillDTO> LoadByNpcMonster(short npcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (NpcMonsterSkill NpcMonsterSkillobject in context.NpcMonsterSkill.Where(i => i.NpcMonsterVNum == npcId))
                {
                    yield return _mapper.Map<NpcMonsterSkillDTO>(NpcMonsterSkillobject);
                }
            }
        }

        #endregion
    }
}