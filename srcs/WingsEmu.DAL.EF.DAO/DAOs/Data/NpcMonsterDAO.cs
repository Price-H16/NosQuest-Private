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
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs.Data
{
    public class NpcMonsterDAO : MappingBaseDao<NpcMonster, NpcMonsterDTO>, INpcMonsterDAO
    {
        public NpcMonsterDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public IEnumerable<NpcMonsterDTO> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (NpcMonster npcMonster in context.NpcMonster.Where(s => s.Name.Contains(name)))
                {
                    yield return _mapper.Map<NpcMonsterDTO>(npcMonster);
                }
            }
        }

        public void Insert(List<NpcMonsterDTO> npcs)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (NpcMonsterDTO npc in npcs)
                    {
                        var entity = _mapper.Map<NpcMonster>(npc);
                        context.NpcMonster.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }


        public SaveResult InsertOrUpdate(ref NpcMonsterDTO npcMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    short npcMonsterVNum = npcMonster.NpcMonsterVNum;
                    NpcMonster entity = context.NpcMonster.FirstOrDefault(c => c.NpcMonsterVNum.Equals(npcMonsterVNum));

                    if (entity == null)
                    {
                        npcMonster = Insert(npcMonster, context);
                        return SaveResult.Inserted;
                    }

                    npcMonster = Update(entity, npcMonster, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_NPCMONSTER_ERROR"), npcMonster.NpcMonsterVNum, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<NpcMonsterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (NpcMonster NpcMonster in context.NpcMonster)
                {
                    yield return _mapper.Map<NpcMonsterDTO>(NpcMonster);
                }
            }
        }

        public NpcMonsterDTO LoadByVNum(short vnum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<NpcMonsterDTO>(context.NpcMonster.FirstOrDefault(i => i.NpcMonsterVNum.Equals(vnum)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private NpcMonsterDTO Insert(NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            var entity = _mapper.Map<NpcMonster>(npcMonster);
            context.NpcMonster.Add(entity);
            context.SaveChanges();
            return _mapper.Map<NpcMonsterDTO>(entity);
        }

        private NpcMonsterDTO Update(NpcMonster entity, NpcMonsterDTO npcMonster, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(npcMonster, entity);
                context.SaveChanges();
            }

            return _mapper.Map<NpcMonsterDTO>(entity);
        }

        #endregion
    }
}