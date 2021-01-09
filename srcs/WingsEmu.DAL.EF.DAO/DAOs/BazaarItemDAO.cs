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
    public class BazaarItemDAO : MappingBaseDao<BazaarItem, BazaarItemDTO>, IBazaarItemDAO
    {
        public BazaarItemDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long bazaarItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    BazaarItem BazaarItem = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (BazaarItem != null)
                    {
                        context.BazaarItem.Remove(BazaarItem);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), bazaarItemId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref BazaarItemDTO bazaarItem)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long bazaarItemId = bazaarItem.BazaarItemId;
                    BazaarItem entity = context.BazaarItem.FirstOrDefault(c => c.BazaarItemId.Equals(bazaarItemId));

                    if (entity == null)
                    {
                        bazaarItem = Insert(bazaarItem, context);
                        return SaveResult.Inserted;
                    }

                    bazaarItem = Update(entity, bazaarItem, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_ERROR"), bazaarItem.BazaarItemId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<BazaarItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (BazaarItem bazaarItem in context.BazaarItem)
                {
                    yield return _mapper.Map<BazaarItemDTO>(bazaarItem);
                }
            }
        }

        public BazaarItemDTO LoadById(long bazaarItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<BazaarItemDTO>(context.BazaarItem.FirstOrDefault(i => i.BazaarItemId.Equals(bazaarItemId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }


        public void RemoveOutDated()
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (BazaarItem entity in context.BazaarItem.Where(e => e.DateStart.AddDays(e.MedalUsed ? 30 : 7).AddHours(e.Duration) < DateTime.Now))
                    {
                        context.BazaarItem.Remove(entity);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private BazaarItemDTO Insert(BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            var entity = _mapper.Map<BazaarItem>(bazaarItem);
            context.BazaarItem.Add(entity);
            context.SaveChanges();
            return _mapper.Map<BazaarItemDTO>(entity);
        }

        private BazaarItemDTO Update(BazaarItem entity, BazaarItemDTO bazaarItem, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(bazaarItem, entity);
                context.SaveChanges();
            }

            return _mapper.Map<BazaarItemDTO>(entity);
        }

        #endregion
    }
}