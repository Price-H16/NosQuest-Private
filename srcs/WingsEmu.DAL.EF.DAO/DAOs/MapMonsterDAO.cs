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
    public class MapMonsterDAO : MappingBaseDao<MapMonster, MapMonsterDTO>, IMapMonsterDAO
    {
        public MapMonsterDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public IEnumerable<MapMonsterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapMonster.ToArray().Select(_mapper.Map<MapMonsterDTO>);
            }
        }

        public DeleteResult DeleteById(int mapMonsterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    MapMonster monster = context.MapMonster.First(i => i.MapMonsterId.Equals(mapMonsterId));

                    if (monster != null)
                    {
                        context.MapMonster.Remove(monster);
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

        public bool DoesMonsterExist(int mapMonsterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapMonster.Any(i => i.MapMonsterId.Equals(mapMonsterId));
            }
        }

        public void Insert(IEnumerable<MapMonsterDTO> monsters)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (MapMonsterDTO monster in monsters)
                    {
                        var entity = _mapper.Map<MapMonster>(monster);
                        context.MapMonster.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public MapMonsterDTO Insert(MapMonsterDTO mapMonster)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<MapMonster>(mapMonster);
                    context.MapMonster.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<MapMonsterDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public MapMonsterDTO LoadById(int mapMonsterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapMonsterDTO>(context.MapMonster.FirstOrDefault(i => i.MapMonsterId.Equals(mapMonsterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MapMonsterDTO> LoadFromMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapMonster.Where(c => c.MapId.Equals(mapId)).ToArray().Select(_mapper.Map<MapMonsterDTO>);
            }
        }

        #endregion
    }
}