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
    public class ShopDAO : MappingBaseDao<Shop, ShopDTO>, IShopDAO
    {
        public ShopDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public void Insert(List<ShopDTO> shops)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ShopDTO Item in shops)
                    {
                        var entity = _mapper.Map<Shop>(Item);
                        context.Shop.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public ShopDTO Insert(ShopDTO shop)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    if (context.Shop.FirstOrDefault(c => c.MapNpcId.Equals(shop.MapNpcId)) == null)
                    {
                        var entity = _mapper.Map<Shop>(shop);
                        context.Shop.Add(entity);
                        context.SaveChanges();
                        return _mapper.Map<ShopDTO>(entity);
                    }

                    return new ShopDTO();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ShopDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Shop entity in context.Shop.ToArray())
                {
                    yield return _mapper.Map<ShopDTO>(entity);
                }
            }
        }

        public ShopDTO LoadById(int shopId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ShopDTO>(context.Shop.FirstOrDefault(s => s.ShopId.Equals(shopId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public ShopDTO LoadByNpc(int mapNpcId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ShopDTO>(context.Shop.FirstOrDefault(s => s.MapNpcId.Equals(mapNpcId)));
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