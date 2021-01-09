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
    public class DropDAO : MappingBaseDao<Drop, DropDTO>, IDropDAO
    {
        public DropDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<DropDTO> drops)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (DropDTO Drop in drops)
                    {
                        var entity = _mapper.Map<Drop>(Drop);
                        context.Drop.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public DropDTO Insert(DropDTO drop)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Drop>(drop);
                    context.Drop.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<DropDTO>(drop);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public List<DropDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Drop.ToList().Select(d => _mapper.Map<DropDTO>(d)).ToList();
            }
        }

        public IEnumerable<DropDTO> LoadByMonster(short monsterVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Drop Drop in context.Drop.Where(s => s.MonsterVNum == monsterVNum || s.MonsterVNum == null))
                {
                    yield return _mapper.Map<DropDTO>(Drop);
                }
            }
        }

        #endregion
    }
}