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
    public class FamilyLogDAO : MappingBaseDao<FamilyLog, FamilyLogDTO>, IFamilyLogDAO
    {
        public FamilyLogDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long familylogId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilyLog famlog = context.FamilyLog.FirstOrDefault(c => c.FamilyLogId.Equals(familylogId));

                    if (famlog != null)
                    {
                        context.FamilyLog.Remove(famlog);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), familylogId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilyLogDTO famlog)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long FamilyLog = famlog.FamilyLogId;
                    FamilyLog entity = context.FamilyLog.FirstOrDefault(c => c.FamilyLogId.Equals(FamilyLog));

                    if (entity == null)
                    {
                        famlog = Insert(famlog, context);
                        return SaveResult.Inserted;
                    }

                    famlog = Update(entity, famlog, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_FAMILYLOG_ERROR"), famlog.FamilyLogId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<FamilyLogDTO> LoadByFamilyId(long familyId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (FamilyLog familylog in context.FamilyLog.Where(fc => fc.FamilyId.Equals(familyId)))
                {
                    yield return _mapper.Map<FamilyLogDTO>(familylog);
                }
            }
        }

        private FamilyLogDTO Insert(FamilyLogDTO famlog, OpenNosContext context)
        {
            var entity = _mapper.Map<FamilyLog>(famlog);
            context.FamilyLog.Add(entity);
            context.SaveChanges();
            return _mapper.Map<FamilyLogDTO>(entity);
        }

        private FamilyLogDTO Update(FamilyLog entity, FamilyLogDTO famlog, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(famlog, entity);
                context.SaveChanges();
            }

            return _mapper.Map<FamilyLogDTO>(entity);
        }

        #endregion
    }
}