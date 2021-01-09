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
    public class ExchangeLogDao : MappingBaseDao<ExchangeLog, ExchangeLogDTO>, IExchangeLogDao
    {
        public ExchangeLogDao(IMapper mapper) : base(mapper)
        {
        }

        public SaveResult InsertOrUpdate(ref ExchangeLogDTO exchange)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long id = exchange.Id;
                    ExchangeLog entity = context.ExchangeLog.FirstOrDefault(c => c.Id.Equals(id));

                    if (entity == null)
                    {
                        exchange = Insert(exchange, context);
                        return SaveResult.Inserted;
                    }

                    exchange.Id = entity.Id;
                    exchange = Update(entity, exchange, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public ExchangeLogDTO Insert(ExchangeLogDTO exchange, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<ExchangeLog>(exchange);
                context.ExchangeLog.Add(entity);
                context.SaveChanges();
                return _mapper.Map<ExchangeLogDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public ExchangeLogDTO Update(ExchangeLog old, ExchangeLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                _mapper.Map(old, replace);
                context.SaveChanges();
            }

            return _mapper.Map<ExchangeLogDTO>(old);
        }
    }
}