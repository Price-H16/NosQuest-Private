﻿// WingsEmu
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
    public class LogChatDAO : MappingBaseDao<LogChat, LogChatDTO>, ILogChatDAO
    {
        public LogChatDAO(IMapper mapper) : base(mapper)
        {
        }

        public SaveResult InsertOrUpdate(ref LogChatDTO log)
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

        public DeleteResult DeleteById(long logId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    LogChat log = context.LogChat.First(i => i.LogId.Equals(logId));

                    if (log == null)
                    {
                        return DeleteResult.Deleted;
                    }

                    context.LogChat.Remove(log);
                    context.SaveChanges();

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public IEnumerable<LogChatDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LogChat log in context.LogChat)
                {
                    yield return _mapper.Map<LogChatDTO>(log);
                }
            }
        }

        public LogChatDTO LoadByLogId(long logId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<LogChatDTO>(context.LogChat.FirstOrDefault(i => i.LogId == logId));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        private LogChatDTO Insert(LogChatDTO log, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<LogChat>(log);
                context.LogChat.Add(entity);
                context.SaveChanges();
                return _mapper.Map<LogChatDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private LogChatDTO Update(LogChat entity, LogChatDTO respawn, OpenNosContext context)
        {
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(respawn, entity);
            context.SaveChanges();
            return _mapper.Map<LogChatDTO>(entity);
        }
    }
}