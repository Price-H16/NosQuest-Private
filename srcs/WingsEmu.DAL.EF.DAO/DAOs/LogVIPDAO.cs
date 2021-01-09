// WingsEmu
// 
// Developed by NosWings Team

using System;
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
    public class LogVIPDAO : MappingBaseDao<LogVip, LogVIPDTO>, ILogVIPDAO
    {
        public LogVIPDAO(IMapper mapper) : base(mapper)
        {
        }

        public SaveResult InsertOrUpdate(ref LogVIPDTO log)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long logId = log.LogId;
                    LogChat entity = context.LogChat.FirstOrDefault(c => c.LogId.Equals(logId));

                    if (entity == null)
                    {
                        log = Insert(log, context);
                        return SaveResult.Inserted;
                    }

                    log.LogId = entity.LogId;
                    log = Update(entity, log, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public LogVIPDTO GetLastByAccountId(long accountId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<LogVIPDTO>(context.LogVip.LastOrDefault(s => s.AccountId == accountId));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error("GET_LAST_BY_ACCOUNT_ID", e);
                return null;
            }
        }

        private LogVIPDTO Insert(LogVIPDTO log, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<LogVip>(log);
                context.LogVip.Add(entity);
                context.SaveChanges();
                return _mapper.Map<LogVIPDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private LogVIPDTO Update(LogChat entity, LogVIPDTO respawn, OpenNosContext context)
        {
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(respawn, entity);
            context.SaveChanges();
            return _mapper.Map<LogVIPDTO>(entity);
        }
    }
}