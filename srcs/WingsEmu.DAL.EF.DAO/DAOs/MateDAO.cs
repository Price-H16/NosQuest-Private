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
    public class MateDAO : MappingBaseDao<Mate, MateDTO>, IMateDAO
    {
        public MateDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Mate mate = context.Mate.FirstOrDefault(c => c.MateId.Equals(id));
                    if (mate != null)
                    {
                        context.Mate.Remove(mate);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_MATE_ERROR"), e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MateDTO mate)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long MateId = mate.MateId;
                    Mate entity = context.Mate.FirstOrDefault(c => c.MateId.Equals(MateId));

                    if (entity == null)
                    {
                        mate = Insert(mate, context);
                        return SaveResult.Inserted;
                    }

                    mate = Update(entity, mate, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), mate, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MateDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Mate mate in context.Mate.Where(s => s.CharacterId == characterId))
                {
                    yield return _mapper.Map<MateDTO>(mate);
                }
            }
        }

        private MateDTO Insert(MateDTO mate, OpenNosContext context)
        {
            var entity = _mapper.Map<Mate>(mate);
            context.Mate.Add(entity);
            context.SaveChanges();
            return _mapper.Map<MateDTO>(entity);
        }

        private MateDTO Update(Mate entity, MateDTO character, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(character, entity);
                context.SaveChanges();
            }

            return _mapper.Map<MateDTO>(entity);
        }

        #endregion
    }
}