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
    public class LogCommandsDAO : MappingBaseDao<LogCommands, LogCommandsDTO>, ILogCommandsDAO
    {
        public LogCommandsDAO(IMapper mapper) : base(mapper)
        {
        }

        public SaveResult InsertOrUpdate(ref LogCommandsDTO logCommand)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long logId = logCommand.CommandId;
                    LogCommands entity = context.LogCommands.FirstOrDefault(c => c.CommandId.Equals(logId));

                    if (entity == null)
                    {
                        logCommand = Insert(logCommand, context);
                        return SaveResult.Inserted;
                    }

                    logCommand.CommandId = entity.CommandId;
                    logCommand = Update(entity, logCommand, context);
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
                    LogCommands log = context.LogCommands.First(i => i.CommandId.Equals(logId));

                    if (log != null)
                    {
                        context.LogCommands.Remove(log);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public IEnumerable<LogCommandsDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (LogCommands log in context.LogCommands)
                {
                    yield return _mapper.Map<LogCommandsDTO>(log);
                }
            }
        }

        public LogCommandsDTO LoadByLogId(long logId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<LogCommandsDTO>(context.LogCommands.FirstOrDefault(i => i.CommandId == logId));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        private LogCommandsDTO Insert(LogCommandsDTO log, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<LogCommands>(log);
                context.LogCommands.Add(entity);
                context.SaveChanges();
                return _mapper.Map<LogCommandsDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private LogCommandsDTO Update(LogCommands entity, LogCommandsDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawn, entity);
                context.SaveChanges();
            }

            return _mapper.Map<LogCommandsDTO>(entity);
        }
    }
}