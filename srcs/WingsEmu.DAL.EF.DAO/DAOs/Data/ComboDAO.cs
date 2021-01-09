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
    public class ComboDAO : MappingBaseDao<Combo, ComboDTO>, IComboDAO
    {
        public ComboDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<ComboDTO> combos)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ComboDTO combo in combos)
                    {
                        var entity = _mapper.Map<Combo>(combo);
                        context.Combo.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public ComboDTO Insert(ComboDTO combo)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Combo>(combo);
                    context.Combo.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ComboDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ComboDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Combo combo in context.Combo)
                {
                    yield return _mapper.Map<ComboDTO>(combo);
                }
            }
        }

        public ComboDTO LoadById(short comboId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ComboDTO>(context.Combo.FirstOrDefault(s => s.SkillVNum.Equals(comboId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ComboDTO> LoadBySkillVnum(short skillVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Combo combo in context.Combo.Where(c => c.SkillVNum == skillVNum))
                {
                    yield return _mapper.Map<ComboDTO>(combo);
                }
            }
        }

        public IEnumerable<ComboDTO> LoadByVNumHitAndEffect(short skillVNum, short hit, short effect)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Combo combo in context.Combo.Where(s => s.SkillVNum == skillVNum && s.Hit == hit && s.Effect == effect))
                {
                    yield return _mapper.Map<ComboDTO>(combo);
                }
            }
        }

        #endregion
    }
}