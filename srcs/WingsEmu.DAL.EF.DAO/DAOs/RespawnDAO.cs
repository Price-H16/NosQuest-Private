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
    public class RespawnDAO : MappingBaseDao<Respawn, RespawnDTO>, IRespawnDAO
    {
        public RespawnDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public SaveResult InsertOrUpdate(ref RespawnDTO respawn)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long CharacterId = respawn.CharacterId;
                    long RespawnMapTypeId = respawn.RespawnMapTypeId;
                    Respawn entity = context.Respawn.FirstOrDefault(c => c.RespawnMapTypeId.Equals(RespawnMapTypeId) && c.CharacterId.Equals(CharacterId));

                    if (entity == null)
                    {
                        respawn = Insert(respawn, context);
                        return SaveResult.Inserted;
                    }

                    respawn.RespawnId = entity.RespawnId;
                    respawn = Update(entity, respawn, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<RespawnDTO> LoadByCharacter(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Respawn Respawnobject in context.Respawn.Where(i => i.CharacterId.Equals(characterId)))
                {
                    yield return _mapper.Map<RespawnDTO>(Respawnobject);
                }
            }
        }

        public RespawnDTO LoadById(long respawnId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RespawnDTO>(context.Respawn.FirstOrDefault(s => s.RespawnId.Equals(respawnId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private RespawnDTO Insert(RespawnDTO respawn, OpenNosContext context)
        {
            try
            {
                var entity = _mapper.Map<Respawn>(respawn);
                context.Respawn.Add(entity);
                context.SaveChanges();
                return _mapper.Map<RespawnDTO>(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private RespawnDTO Update(Respawn entity, RespawnDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(respawn, entity);
                context.SaveChanges();
            }

            return _mapper.Map<RespawnDTO>(entity);
        }

        #endregion
    }
}