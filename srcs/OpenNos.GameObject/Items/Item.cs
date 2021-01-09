﻿// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Linq;
using OpenNos.DAL;
using OpenNos.GameObject.Buff;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;

namespace OpenNos.GameObject.Items
{
    public abstract class Item : ItemDTO
    {
        #region Instantiation

        protected Item()
        {
        }

        protected Item(ItemDTO item)
        {
            InitializeItem(item);
        }

        #endregion

        #region Properties

        public List<BCard> BCards { get; set; }
        public List<RollGeneratedItemDTO> RollGeneratedItems { get; set; }

        #endregion

        #region Methods

        public void InitializeItem(ItemDTO item)
        {
            // manual mapping to avoid automapper outside of DAO
            Height = item.Height;
            Width = item.Width;
            MinilandObjectPoint = item.MinilandObjectPoint;
            BasicUpgrade = item.BasicUpgrade;
            CellonLvl = item.CellonLvl;
            Class = item.Class;
            CloseDefence = item.CloseDefence;
            Color = item.Color;
            Concentrate = item.Concentrate;
            CriticalRate = item.CriticalRate;
            CriticalLuckRate = item.CriticalLuckRate;
            DamageMaximum = item.DamageMaximum;
            DamageMinimum = item.DamageMinimum;
            DarkElement = item.DarkElement;
            DarkResistance = item.DarkResistance;
            DefenceDodge = item.DefenceDodge;
            DistanceDefence = item.DistanceDefence;
            DistanceDefenceDodge = item.DistanceDefenceDodge;
            Effect = item.Effect;
            EffectValue = item.EffectValue;
            Element = item.Element;
            ElementRate = item.ElementRate;
            EquipmentSlot = item.EquipmentSlot;
            FireElement = item.FireElement;
            FireResistance = item.FireResistance;
            HitRate = item.HitRate;
            Hp = item.Hp;
            HpRegeneration = item.HpRegeneration;
            IsMinilandActionable = item.IsMinilandActionable;
            IsColored = item.IsColored;
            IsConsumable = item.IsConsumable;
            IsDroppable = item.IsDroppable;
            IsHeroic = item.IsHeroic;
            IsWarehouse = item.IsWarehouse;
            IsSoldable = item.IsSoldable;
            IsTradable = item.IsTradable;
            Flag9 = item.Flag9;
            ItemSubType = item.ItemSubType;
            ItemType = item.ItemType;
            ItemValidTime = item.ItemValidTime;
            LevelJobMinimum = item.LevelJobMinimum;
            LevelMinimum = item.LevelMinimum;
            LightElement = item.LightElement;
            LightResistance = item.LightResistance;
            MagicDefence = item.MagicDefence;
            MaxCellon = item.MaxCellon;
            MaxCellonLvl = item.MaxCellonLvl;
            MaxElementRate = item.MaxElementRate;
            MaximumAmmo = item.MaximumAmmo;
            MoreHp = item.MoreHp;
            MoreMp = item.MoreMp;
            Morph = item.Morph;
            Mp = item.Mp;
            MpRegeneration = item.MpRegeneration;
            Name = item.Name;
            Price = item.Price;
            PvpDefence = item.PvpDefence;
            PvpStrength = item.PvpStrength;
            ReduceOposantResistance = item.ReduceOposantResistance;
            ReputationMinimum = item.ReputationMinimum;
            ReputPrice = item.ReputPrice;
            SecondaryElement = item.SecondaryElement;
            Sex = item.Sex;
            Speed = item.Speed;
            SpType = item.SpType;
            Type = item.Type;
            VNum = item.VNum;
            WaitDelay = item.WaitDelay;
            WaterElement = item.WaterElement;
            WaterResistance = item.WaterResistance;
            BCards = new List<BCard>();
            RollGeneratedItems = new List<RollGeneratedItemDTO>();
        }

        //TODO: Convert to PacketDefinition
        public abstract void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null);

        #endregion
    }
}