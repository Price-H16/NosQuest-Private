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
    public class ShopSkillDAO : MappingBaseDao<ShopSkill, ShopSkillDTO>, IShopSkillDAO
    {
        public ShopSkillDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public ShopSkillDTO Insert(ShopSkillDTO shopSkill)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<ShopSkill>(shopSkill);
                    context.ShopSkill.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ShopSkillDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public void Insert(List<ShopSkillDTO> skills)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ShopSkillDTO Skill in skills)
                    {
                        var entity = _mapper.Map<ShopSkill>(Skill);
                        context.ShopSkill.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<ShopSkillDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopSkill entity in context.ShopSkill)
                {
                    yield return _mapper.Map<ShopSkillDTO>(entity);
                }
            }
        }

        public IEnumerable<ShopSkillDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopSkill ShopSkill in context.ShopSkill.Where(s => s.ShopId.Equals(shopId)))
                {
                    yield return _mapper.Map<ShopSkillDTO>(ShopSkill);
                }
            }
        }

        #endregion
    }
}