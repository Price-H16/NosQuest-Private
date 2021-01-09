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
    public class SkillDAO : MappingBaseDao<Skill, SkillDTO>, ISkillDAO
    {
        public SkillDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<SkillDTO> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (SkillDTO skill in skills)
                    {
                        var entity = _mapper.Map<Skill>(skill);
                        context.Skill.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public SkillDTO Insert(SkillDTO skill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Skill>(skill);
                    context.Skill.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<SkillDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<SkillDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Skill Skill in context.Skill)
                {
                    yield return _mapper.Map<SkillDTO>(Skill);
                }
            }
        }

        public SkillDTO LoadById(short skillId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<SkillDTO>(context.Skill.FirstOrDefault(s => s.SkillVNum.Equals(skillId)));
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