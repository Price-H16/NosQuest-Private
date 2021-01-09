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

namespace WingsEmu.DAL.EF.DAO.DAOs.Data
{
    public class MapDAO : MappingBaseDao<Map, MapDTO>, IMapDAO
    {
        public MapDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<MapDTO> maps)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (MapDTO Item in maps)
                    {
                        var entity = _mapper.Map<Map>(Item);
                        context.Map.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public MapDTO Insert(MapDTO map)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    if (context.Map.FirstOrDefault(c => c.MapId.Equals(map.MapId)) == null)
                    {
                        var entity = _mapper.Map<Map>(map);
                        context.Map.Add(entity);
                        context.SaveChanges();
                        return _mapper.Map<MapDTO>(entity);
                    }

                    return new MapDTO();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MapDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Map Map in context.Map)
                {
                    yield return _mapper.Map<MapDTO>(Map);
                }
            }
        }

        public MapDTO LoadById(short mapId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapDTO>(context.Map.FirstOrDefault(c => c.MapId.Equals(mapId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        #endregion
    }
}