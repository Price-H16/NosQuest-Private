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
    public class RaidLogDAO : MappingBaseDao<RaidLog, RaidLogDTO>, IRaidLogDAO
    {
        public RaidLogDAO(IMapper mapper) : base(mapper)
        {
        }

        public SaveResult InsertOrUpdate(ref RaidLogDTO raid)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long raidId = raid.RaidId;
                    RaidLog entity = context.RaidLog.FirstOrDefault(c => c.RaidId.Equals(raidId));

                    if (entity == null)
                    {
                        raid = Insert(raid, context);
                        return SaveResult.Inserted;
                    }

                    raid.RaidId = entity.RaidId;
                    raid = Update(entity, raid, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<RaidLogDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RaidLog id in context.RaidLog.Where(c => c.CharacterId == characterId))
                {
                    yield return _mapper.Map<RaidLogDTO>(id);
                }
            }
        }

        public IEnumerable<RaidLogDTO> LoadByFamilyId(long familyId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RaidLog id in context.RaidLog.Where(c => c.FamilyId == familyId))
                {
                    yield return _mapper.Map<RaidLogDTO>(id);
                }
            }
        }

        public SaveResult InsertOrUpdateList(ref List<RaidLogDTO> raidList)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (RaidLogDTO r in raidList)
                    {
                        RaidLogDTO raid = r;
                        long raidId = raid.RaidId;
                        RaidLog entity = context.RaidLog.FirstOrDefault(c => c.RaidId.Equals(raidId));

                        if (entity == null)
                        {
                            raid = Insert(raid, context);
                            return SaveResult.Inserted;
                        }

                        raid.RaidId = entity.RaidId;
                        raid = Update(entity, raid, context);
                        return SaveResult.Updated;
                    }

                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public RaidLogDTO Insert(RaidLogDTO raid, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<RaidLog>(raid);
                context.RaidLog.Add(entity);
                context.SaveChanges();
                return _mapper.Map<RaidLogDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public RaidLogDTO Update(RaidLog old, RaidLogDTO replace, OpenNosContext context)
        {
            if (old != null)
            {
                _mapper.Map(old, replace);
                context.SaveChanges();
            }

            return _mapper.Map<RaidLogDTO>(old);
        }
    }
}