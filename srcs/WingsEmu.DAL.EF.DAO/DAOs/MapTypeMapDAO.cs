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
    public class MapTypeMapDAO : MappingBaseDao<MapTypeMap, MapTypeMapDTO>, IMapTypeMapDAO
    {
        public MapTypeMapDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<MapTypeMapDTO> mapTypeMaps)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (MapTypeMapDTO mapTypeMap in mapTypeMaps)
                    {
                        var entity = _mapper.Map<MapTypeMap>(mapTypeMap);
                        context.MapTypeMap.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<MapTypeMapDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap)
                {
                    yield return _mapper.Map<MapTypeMapDTO>(MapTypeMap);
                }
            }
        }

        public MapTypeMapDTO LoadByMapAndMapType(short mapId, short maptypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapTypeMapDTO>(context.MapTypeMap.FirstOrDefault(i => i.MapId.Equals(mapId) && i.MapTypeId.Equals(maptypeId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MapTypeMapDTO> LoadByMapId(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap.Where(c => c.MapId.Equals(mapId)))
                {
                    yield return _mapper.Map<MapTypeMapDTO>(MapTypeMap);
                }
            }
        }

        public IEnumerable<MapTypeMapDTO> LoadByMapTypeId(short maptypeId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapTypeMap MapTypeMap in context.MapTypeMap.Where(c => c.MapTypeId.Equals(maptypeId)))
                {
                    yield return _mapper.Map<MapTypeMapDTO>(MapTypeMap);
                }
            }
        }

        #endregion
    }
}