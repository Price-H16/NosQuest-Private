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
    public class RespawnMapTypeDAO : MappingBaseDao<RespawnMapType, RespawnMapTypeDTO>, IRespawnMapTypeDAO
    {
        public RespawnMapTypeDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<RespawnMapTypeDTO> respawnMapType)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (RespawnMapTypeDTO RespawnMapType in respawnMapType)
                    {
                        var entity = _mapper.Map<RespawnMapType>(RespawnMapType);
                        context.RespawnMapType.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public SaveResult InsertOrUpdate(ref RespawnMapTypeDTO respawnMapType)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    short mapId = respawnMapType.DefaultMapId;
                    RespawnMapType entity = context.RespawnMapType.FirstOrDefault(c => c.DefaultMapId.Equals(mapId));

                    if (entity == null)
                    {
                        respawnMapType = Insert(respawnMapType, context);
                        return SaveResult.Inserted;
                    }

                    respawnMapType.RespawnMapTypeId = entity.RespawnMapTypeId;
                    respawnMapType = Update(entity, respawnMapType, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public RespawnMapTypeDTO LoadById(long respawnMapTypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RespawnMapTypeDTO>(context.RespawnMapType.FirstOrDefault(s => s.RespawnMapTypeId.Equals(respawnMapTypeId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public RespawnMapTypeDTO LoadByMapId(short mapId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RespawnMapTypeDTO>(context.RespawnMapType.FirstOrDefault(s => s.DefaultMapId.Equals(mapId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RespawnMapTypeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.RespawnMapType.ToArray().Select(_mapper.Map<RespawnMapTypeDTO>);
            }
        }

        private RespawnMapTypeDTO Insert(RespawnMapTypeDTO respawnMapType, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<RespawnMapType>(respawnMapType);
                context.RespawnMapType.Add(entity);
                context.SaveChanges();
                return _mapper.Map<RespawnMapTypeDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private RespawnMapTypeDTO Update(RespawnMapType entity, RespawnMapTypeDTO respawnMapType, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawnMapType, entity);
                context.SaveChanges();
            }

            return _mapper.Map<RespawnMapTypeDTO>(entity);
        }

        #endregion
    }
}