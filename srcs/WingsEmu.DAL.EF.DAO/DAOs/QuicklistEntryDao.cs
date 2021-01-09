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
    public class QuicklistEntryDAO : SynchronizableBaseDAO<QuicklistEntry, QuicklistEntryDTO>, IQuicklistEntryDAO
    {
        public QuicklistEntryDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public IEnumerable<QuicklistEntryDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (QuicklistEntry QuicklistEntryobject in context.QuicklistEntry.Where(i => i.CharacterId == characterId))
                {
                    yield return _mapper.Map<QuicklistEntryDTO>(QuicklistEntryobject);
                }
            }
        }

        public IEnumerable<Guid> LoadKeysByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.QuicklistEntry.Where(i => i.CharacterId == characterId).Select(qle => qle.Id).ToList();
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