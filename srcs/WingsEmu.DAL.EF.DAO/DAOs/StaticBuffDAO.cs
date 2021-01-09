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

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class StaticBuffDAO : MappingBaseDao<StaticBuff, StaticBuffDTO>, IStaticBuffDAO
    {
        public StaticBuffDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public SaveResult InsertOrUpdate(ref StaticBuffDTO StaticBuff)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long id = StaticBuff.CharacterId;
                    short cardid = StaticBuff.CardId;
                    StaticBuff entity = context.StaticBuff.FirstOrDefault(c => c.CardId == cardid && c.CharacterId == id);

                    if (entity == null)
                    {
                        StaticBuff = Insert(StaticBuff, context);
                        return SaveResult.Inserted;
                    }

                    StaticBuff.StaticBuffId = entity.StaticBuffId;
                    StaticBuff = Update(entity, StaticBuff, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<StaticBuffDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (StaticBuff entity in context.StaticBuff.Where(i => i.CharacterId == characterId))
                {
                    yield return _mapper.Map<StaticBuffDTO>(entity);
                }
            }
        }

        public StaticBuffDTO LoadById(long sbId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<StaticBuffDTO>(context.RespawnMapType.FirstOrDefault(s => s.RespawnMapTypeId.Equals(sbId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private StaticBuffDTO Insert(StaticBuffDTO sb, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<StaticBuff>(sb);
                context.StaticBuff.Add(entity);
                context.SaveChanges();
                return _mapper.Map<StaticBuffDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public void Delete(short bonusToDelete, long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    StaticBuff bon = context.StaticBuff.FirstOrDefault(c => c.CardId == bonusToDelete && c.CharacterId == characterId);

                    if (bon != null)
                    {
                        context.StaticBuff.Remove(bon);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bonusToDelete, e.Message), e);
            }
        }

        public IEnumerable<short> LoadByTypeCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.StaticBuff.Where(i => i.CharacterId == characterId).Select(qle => qle.CardId).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private StaticBuffDTO Update(StaticBuff entity, StaticBuffDTO sb, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(sb, entity);
                context.SaveChanges();
            }

            return _mapper.Map<StaticBuffDTO>(entity);
        }

        #endregion
    }
}