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

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class GeneralLogDAO : MappingBaseDao<GeneralLog, GeneralLogDTO>, IGeneralLogDAO
    {
        public GeneralLogDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public bool IdAlreadySet(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.GeneralLog.Any(gl => gl.LogId == id);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public GeneralLogDTO Insert(GeneralLogDTO generallog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<GeneralLog>(generallog);
                    context.GeneralLog.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<GeneralLogDTO>(generallog);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<GeneralLogDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (GeneralLog generalLog in context.GeneralLog)
                {
                    yield return _mapper.Map<GeneralLogDTO>(generalLog);
                }
            }
        }

        public IEnumerable<GeneralLogDTO> LoadByAccount(long? accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (GeneralLog generalLog in context.GeneralLog.Where(s => s.AccountId == accountId))
                {
                    yield return _mapper.Map<GeneralLogDTO>(generalLog);
                }
            }
        }

        public IEnumerable<GeneralLogDTO> LoadByLogType(string logType, long? characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (GeneralLog log in context.GeneralLog.Where(c => c.LogType.Equals(logType) && c.CharacterId == characterId))
                {
                    yield return _mapper.Map<GeneralLogDTO>(log);
                }
            }
        }

        public void SetCharIdNull(long? characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (GeneralLog log in context.GeneralLog.Where(c => c.CharacterId == characterId))
                    {
                        log.CharacterId = null;
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void WriteGeneralLog(long accountId, string ipAddress, long? characterId, string logType, string logData)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var log = new GeneralLog
                    {
                        AccountId = accountId,
                        IpAddress = ipAddress,
                        Timestamp = DateTime.Now,
                        LogType = logType,
                        LogData = logData,
                        CharacterId = characterId
                    };

                    context.GeneralLog.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}