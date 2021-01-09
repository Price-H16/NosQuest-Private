// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoMapper;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL.EF.DB;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs.Base;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs.Data
{
    public class ItemDAO : MappingBaseDao<Item, ItemDTO>, IItemDAO
    {
        public ItemDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public IEnumerable<ItemDTO> FindByName(string name)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                if (string.IsNullOrEmpty(name))
                {
                    foreach (Item item in context.Item.Where(s => s.Name.Equals(string.Empty)))
                    {
                        yield return _mapper.Map<ItemDTO>(item);
                    }
                }
                else
                {
                    foreach (Item item in context.Item.Where(s => s.Name.Contains(name)))
                    {
                        yield return _mapper.Map<ItemDTO>(item);
                    }
                }
            }
        }

        public void Insert(IEnumerable<ItemDTO> items)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (ItemDTO Item in items)
                    {
                        var entity = _mapper.Map<Item>(Item);
                        context.Item.Add(entity);
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public ItemDTO Insert(ItemDTO item)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Item>(item);
                    context.Item.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<ItemDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ItemDTO ToDto(Item item) =>
            new ItemDTO
            {
                BasicUpgrade = item.BasicUpgrade,
                CellonLvl = item.CellonLvl,
                Class = item.Class,
                CloseDefence = item.CloseDefence,
                Color = item.Color,
                Concentrate = item.Concentrate,
                CriticalLuckRate = item.CriticalLuckRate,
                CriticalRate = item.CriticalRate,
                DamageMaximum = item.DamageMaximum,
                DamageMinimum = item.DamageMinimum,
                DarkElement = item.DarkElement,
                DarkResistance = item.DarkResistance,
                DefenceDodge = item.DefenceDodge,
                DistanceDefence = item.DistanceDefence,
                DistanceDefenceDodge = item.DistanceDefenceDodge,
                Effect = item.Effect,
                EffectValue = item.EffectValue,
                Element = item.Element,
                ElementRate = item.ElementRate,
                EquipmentSlot = item.EquipmentSlot,
                FireElement = item.FireElement,
                FireResistance = item.FireResistance,
                Height = item.Height,
                HitRate = item.HitRate,
                Hp = item.Hp,
                HpRegeneration = item.HpRegeneration,
                IsMinilandActionable = item.IsMinilandActionable,
                IsColored = item.IsColored,
                Flag1 = item.Flag1,
                Flag2 = item.Flag2,
                Flag3 = item.Flag3,
                Flag4 = item.Flag4,
                Flag5 = item.Flag5,
                Flag6 = item.Flag6,
                Flag7 = item.Flag7,
                Flag8 = item.Flag8,
                IsConsumable = item.IsConsumable,
                IsDroppable = item.IsDroppable,
                IsHeroic = item.IsHeroic,
                Flag9 = item.Flag9,
                IsWarehouse = item.IsWarehouse,
                IsSoldable = item.IsSoldable,
                IsTradable = item.IsTradable,
                ItemSubType = item.ItemSubType,
                ItemType = (ItemType)item.ItemType,
                ItemValidTime = item.ItemValidTime,
                LevelJobMinimum = item.LevelJobMinimum,
                LevelMinimum = item.LevelMinimum,
                LightElement = item.LightElement,
                LightResistance = item.LightResistance,
                MagicDefence = item.MagicDefence,
                MaxCellon = item.MaxCellon,
                MaxCellonLvl = item.MaxCellonLvl,
                MaxElementRate = item.MaxElementRate,
                MaximumAmmo = item.MaximumAmmo,
                MinilandObjectPoint = item.MinilandObjectPoint,
                MoreHp = item.MoreHp,
                MoreMp = item.MoreMp,
                Morph = item.Morph,
                Mp = item.Mp,
                MpRegeneration = item.MpRegeneration,
                Name = item.Name,
                Price = item.Price,
                PvpDefence = item.PvpDefence,
                PvpStrength = item.PvpStrength,
                ReduceOposantResistance = item.ReduceOposantResistance,
                ReputationMinimum = item.ReputationMinimum,
                ReputPrice = item.ReputPrice,
                SecondaryElement = item.SecondaryElement,
                Sex = item.Sex,
                Speed = item.Speed,
                SpType = item.SpType,
                Type = (InventoryType)item.Type,
                VNum = item.VNum,
                WaitDelay = item.WaitDelay,
                WaterElement = item.WaterElement,
                WaterResistance = item.WaterResistance,
                Width = item.Width
            };

        public IEnumerable<ItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Item item in context.Item)
                {
                    yield return _mapper.Map<ItemDTO>(item);
                }
            }
        }

        public ItemDTO LoadById(short ItemVnum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<ItemDTO>(context.Item.FirstOrDefault(i => i.VNum.Equals(ItemVnum)));
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