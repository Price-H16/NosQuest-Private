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
    public class MapTypeDAO : MappingBaseDao<MapType, MapTypeDTO>, IMapTypeDAO
    {
        public MapTypeDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public MapTypeDTO Insert(ref MapTypeDTO mapType)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<MapType>(mapType);
                    context.MapType.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<MapTypeDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MapTypeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (MapType MapType in context.MapType)
                {
                    yield return _mapper.Map<MapTypeDTO>(MapType);
                }
            }
        }

        public MapTypeDTO LoadById(short maptypeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<MapTypeDTO>(context.MapType.FirstOrDefault(s => s.MapTypeId.Equals(maptypeId)));
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