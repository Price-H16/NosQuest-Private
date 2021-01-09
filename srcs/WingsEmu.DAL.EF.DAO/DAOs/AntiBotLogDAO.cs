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
    public class AntiBotLogDAO : MappingBaseDao<AntiBotLog, AntiBotLogDTO>, IAntiBotLogDAO
    {
        public AntiBotLogDAO(IMapper mapper) : base(mapper)
        {
        }

        public SaveResult InsertOrUpdate(ref AntiBotLogDTO log)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long logId = log.Id;
                    AntiBotLog entity = context.AntiBotLog.FirstOrDefault(c => c.Id.Equals(logId));

                    if (entity == null)
                    {
                        log = Insert(log, context);
                        return SaveResult.Inserted;
                    }

                    log.Id = entity.Id;
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

        private AntiBotLogDTO Insert(AntiBotLogDTO log, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<AntiBotLog>(log);
                context.AntiBotLog.Add(entity);
                context.SaveChanges();
                return _mapper.Map<AntiBotLogDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private AntiBotLogDTO Update(AntiBotLog entity, AntiBotLogDTO respawn, OpenNosContext context)
        {
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(respawn, entity);
            context.SaveChanges();
            return _mapper.Map<AntiBotLogDTO>(entity);
        }
    }
}