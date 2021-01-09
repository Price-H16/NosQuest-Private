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
    public class MapNpcDAO : MappingBaseDao<MapNpc, MapNpcDTO>, IMapNpcDAO
    {
        public MapNpcDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<MapNpcDTO> npcs)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (MapNpcDTO Item in npcs)
                    {
                        var entity = _mapper.Map<MapNpc>(Item);
                        context.MapNpc.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public MapNpcDTO Insert(MapNpcDTO npc)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<MapNpc>(npc);
                    context.MapNpc.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<MapNpcDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MapNpcDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapNpc.ToArray().Select(_mapper.Map<MapNpcDTO>);
            }
        }

        public MapNpcDTO LoadById(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapNpcDTO>(context.MapNpc.FirstOrDefault(i => i.MapNpcId.Equals(mapNpcId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MapNpcDTO> LoadFromMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.MapNpc.Where(s => s.MapId == mapId).ToArray().Select(_mapper.Map<MapNpcDTO>);
            }
        }

        #endregion
    }
}