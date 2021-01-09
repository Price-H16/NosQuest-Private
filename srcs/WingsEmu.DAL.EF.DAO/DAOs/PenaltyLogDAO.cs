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
    public class PenaltyLogDAO : MappingBaseDao<PenaltyLog, PenaltyLogDTO>, IPenaltyLogDAO
    {
        public PenaltyLogDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(int penaltylogid)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    PenaltyLog PenaltyLog = context.PenaltyLog.FirstOrDefault(c => c.PenaltyLogId.Equals(penaltylogid));

                    if (PenaltyLog != null)
                    {
                        context.PenaltyLog.Remove(PenaltyLog);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_PENALTYLOG_ERROR"), penaltylogid, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref PenaltyLogDTO log)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    int id = log.PenaltyLogId;
                    PenaltyLog entity = context.PenaltyLog.FirstOrDefault(c => c.PenaltyLogId.Equals(id));

                    if (entity == null)
                    {
                        log = Insert(log, context);
                        return SaveResult.Inserted;
                    }

                    log = Update(entity, log, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_PENALTYLOG_ERROR"), log.PenaltyLogId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<PenaltyLogDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (PenaltyLog entity in context.PenaltyLog)
                {
                    yield return _mapper.Map<PenaltyLogDTO>(entity);
                }
            }
        }

        public IEnumerable<PenaltyLogDTO> LoadByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (PenaltyLog PenaltyLog in context.PenaltyLog.Where(s => s.AccountId.Equals(accountId)))
                {
                    yield return _mapper.Map<PenaltyLogDTO>(PenaltyLog);
                }
            }
        }

        public PenaltyLogDTO LoadById(int relId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<PenaltyLogDTO>(context.PenaltyLog.FirstOrDefault(s => s.PenaltyLogId.Equals(relId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private PenaltyLogDTO Insert(PenaltyLogDTO penaltylog, OpenNosContext context)
        {
            var entity = _mapper.Map<PenaltyLog>(penaltylog);
            context.PenaltyLog.Add(entity);
            context.SaveChanges();
            return _mapper.Map<PenaltyLogDTO>(entity);
        }

        private PenaltyLogDTO Update(PenaltyLog entity, PenaltyLogDTO penaltylog, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(penaltylog, entity);
                context.SaveChanges();
            }

            return _mapper.Map<PenaltyLogDTO>(entity);
        }

        #endregion
    }
}