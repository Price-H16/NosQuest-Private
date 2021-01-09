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
    public class ScriptedInstanceDAO : MappingBaseDao<ScriptedInstance, ScriptedInstanceDTO>, IScriptedInstanceDAO
    {
        public ScriptedInstanceDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<ScriptedInstanceDTO> portals)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ScriptedInstanceDTO Item in portals)
                    {
                        var entity = _mapper.Map<ScriptedInstance>(Item);
                        context.ScriptedInstance.Add(entity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public ScriptedInstanceDTO Insert(ScriptedInstanceDTO timespace)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<ScriptedInstance>(timespace);
                    context.ScriptedInstance.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ScriptedInstanceDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ScriptedInstanceDTO> LoadByMap(short mapId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ScriptedInstance timespaceObject in context.ScriptedInstance.Where(c => c.MapId.Equals(mapId)))
                {
                    yield return _mapper.Map<ScriptedInstanceDTO>(timespaceObject);
                }
            }
        }

        #endregion
    }
}