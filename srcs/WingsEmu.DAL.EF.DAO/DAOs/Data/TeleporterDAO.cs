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
    public class TeleporterDAO : MappingBaseDao<Teleporter, TeleporterDTO>, ITeleporterDAO
    {
        public TeleporterDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public TeleporterDTO Insert(TeleporterDTO teleporter)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Teleporter>(teleporter);
                    context.Teleporter.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<TeleporterDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<TeleporterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Teleporter entity in context.Teleporter)
                {
                    yield return _mapper.Map<TeleporterDTO>(entity);
                }
            }
        }

        public TeleporterDTO LoadById(short teleporterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<TeleporterDTO>(context.Teleporter.FirstOrDefault(i => i.TeleporterId.Equals(teleporterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<TeleporterDTO> LoadFromNpc(int npcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Teleporter entity in context.Teleporter.Where(c => c.MapNpcId.Equals(npcId)))
                {
                    yield return _mapper.Map<TeleporterDTO>(entity);
                }
            }
        }

        #endregion
    }
}