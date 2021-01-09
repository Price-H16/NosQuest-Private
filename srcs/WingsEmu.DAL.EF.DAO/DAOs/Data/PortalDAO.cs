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
    public class PortalDAO : MappingBaseDao<Portal, PortalDTO>, IPortalDAO
    {
        public PortalDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<PortalDTO> portals)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (PortalDTO Item in portals)
                    {
                        var entity = _mapper.Map<Portal>(Item);
                        context.Portal.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public PortalDTO Insert(PortalDTO portal)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Portal>(portal);
                    context.Portal.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<PortalDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
        public IEnumerable<PortalDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Portal.ToArray().Select(_mapper.Map<PortalDTO>);
            }
        }

        public IEnumerable<PortalDTO> LoadByMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Portal portal in context.Portal.Where(c => c.SourceMapId.Equals(mapId)))
                {
                    yield return _mapper.Map<PortalDTO>(portal);
                }
            }
        }

        #endregion
    }
}