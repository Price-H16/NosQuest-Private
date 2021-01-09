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
    public class MinilandObjectDAO : MappingBaseDao<MinilandObject, MinilandObjectDTO>, IMinilandObjectDAO
    {
        public MinilandObjectDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult DeleteById(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MinilandObject item = context.MinilandObject.First(i => i.MinilandObjectId.Equals(id));

                    if (item != null)
                    {
                        context.MinilandObject.Remove(item);
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

        public SaveResult InsertOrUpdate(ref MinilandObjectDTO obj)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long id = obj.MinilandObjectId;
                    MinilandObject entity = context.MinilandObject.FirstOrDefault(c => c.MinilandObjectId.Equals(id));

                    if (entity == null)
                    {
                        obj = Insert(obj, context);
                        return SaveResult.Inserted;
                    }

                    obj.MinilandObjectId = entity.MinilandObjectId;
                    obj = Update(entity, obj, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MinilandObjectDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MinilandObject obj in context.MinilandObject.Where(s => s.CharacterId == characterId))
                {
                    yield return _mapper.Map<MinilandObjectDTO>(obj);
                }
            }
        }

        private MinilandObjectDTO Insert(MinilandObjectDTO obj, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<MinilandObject>(obj);
                context.MinilandObject.Add(entity);
                context.SaveChanges();
                return _mapper.Map<MinilandObjectDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private MinilandObjectDTO Update(MinilandObject entity, MinilandObjectDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawn, entity);
                context.SaveChanges();
            }

            return _mapper.Map<MinilandObjectDTO>(entity);
        }

        #endregion
    }
}