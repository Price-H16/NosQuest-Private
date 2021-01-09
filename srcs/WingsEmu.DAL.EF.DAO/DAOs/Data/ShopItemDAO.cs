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

namespace WingsEmu.DAL.EF.DAO.DAOs.Data
{
    public class ShopItemDAO : MappingBaseDao<ShopItem, ShopItemDTO>, IShopItemDAO
    {
        public ShopItemDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult DeleteById(int itemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    ShopItem Item = context.ShopItem.FirstOrDefault(i => i.ShopItemId.Equals(itemId));

                    if (Item != null)
                    {
                        context.ShopItem.Remove(Item);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public ShopItemDTO Insert(ShopItemDTO item)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<ShopItem>(item);
                    context.ShopItem.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ShopItemDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public void Insert(List<ShopItemDTO> items)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ShopItemDTO Item in items)
                    {
                        var entity = _mapper.Map<ShopItem>(Item);
                        context.ShopItem.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IEnumerable<ShopItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopItem entity in context.ShopItem)
                {
                    yield return _mapper.Map<ShopItemDTO>(entity);
                }
            }
        }

        public ShopItemDTO LoadById(int itemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ShopItemDTO>(context.ShopItem.FirstOrDefault(i => i.ShopItemId.Equals(itemId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ShopItemDTO> LoadByShopId(int shopId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ShopItem ShopItem in context.ShopItem.Where(i => i.ShopId.Equals(shopId)))
                {
                    yield return _mapper.Map<ShopItemDTO>(ShopItem);
                }
            }
        }

        #endregion
    }
}