﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Interfaces;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items.Instance
{
    public class WearableInstance : ItemInstance, IWearableInstance
    {
        #region Members

        #endregion

        #region Instantiation

        public WearableInstance()
        {
            new Random();
            EquipmentOptions = new List<EquipmentOptionDTO>();
        }

        public WearableInstance(Guid id)
        {
            Id = id;
            new Random();
            EquipmentOptions = new List<EquipmentOptionDTO>();
        }

        public WearableInstance(short vNum, byte amount) : base(vNum, amount)
        {
            new Random();
            EquipmentOptions = new List<EquipmentOptionDTO>();
        }

        #endregion

        #region Properties

        public byte Ammo { get; set; }

        public byte Cellon { get; set; }

        public short CloseDefence { get; set; }

        public short Concentrate { get; set; }

        public short CriticalDodge { get; set; }

        public byte CriticalLuckRate { get; set; }

        public short CriticalRate { get; set; }

        public short DamageMaximum { get; set; }

        public short DamageMinimum { get; set; }

        public byte DarkElement { get; set; }

        public short DarkResistance { get; set; }

        public short DefenceDodge { get; set; }

        public short DistanceDefence { get; set; }

        public short DistanceDefenceDodge { get; set; }

        public List<EquipmentOptionDTO> EquipmentOptions { get; set; }

        public short ElementRate { get; set; }

        public byte FireElement { get; set; }

        public short FireResistance { get; set; }

        public short HitRate { get; set; }

        public short HP { get; set; }

        public bool IsEmpty { get; set; }

        public bool IsFixed { get; set; }

        public byte LightElement { get; set; }

        public short LightResistance { get; set; }

        public short MagicDefence { get; set; }

        public byte MaxElementRate { get; set; }

        public short MP { get; set; }

        public sbyte? ShellRarity { get; set; }

        public byte WaterElement { get; set; }

        public short WaterResistance { get; set; }

        public long XP { get; set; }

        #endregion

        #region Methods

        public string GenerateEInfo()
        {
            EquipmentType equipmentslot = Item.EquipmentSlot;
            ItemType itemType = Item.ItemType;
            byte classe = Item.Class;
            byte subtype = Item.ItemSubType;
            DateTime test = ItemDeleteTime ?? DateTime.Now;
            long time = ItemDeleteTime != null ? (long)(test - DateTime.Now).TotalSeconds : 0;
            long seconds = IsBound ? time : Item.ItemValidTime;
            List<EquipmentOptionDTO> options = EquipmentOptions.Where(s => s.Level <= (s.Level > 12 ? 20 : 8))
                .OrderBy(s => s.Level).ToList();
            options.AddRange(EquipmentOptions.Where(s => s.Level > (s.Level > 12 ? 20 : 8))
                .OrderByDescending(s => s.Level));
            if (seconds < 0)
            {
                seconds = 0;
            }

            switch (itemType)
            {
                case ItemType.Weapon:
                    switch (equipmentslot)
                    {
                        case EquipmentType.MainWeapon:
                            switch (classe)
                            {
                                case 4:
                                    return
                                        $"e_info 1 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {(ShellRarity == null ? "0" : $"{ShellRarity}")} {(BoundCharacterId == null ? "0" : $"{BoundCharacterId}")} {options.Count}{options.OrderBy(s => s.Level).Aggregate(string.Empty, (current, option) => current + $" {option.Level}.{option.Type}.{option.Value}")}"; // -1 = {ShellEffectValue} {FirstShell}...
                                case 8:
                                    return
                                        $"e_info 5 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {(ShellRarity == null ? "0" : $"{ShellRarity}")} {(BoundCharacterId == null ? "0" : $"{BoundCharacterId}")} {options.Count}{options.OrderBy(s => s.Level).Aggregate(string.Empty, (current, option) => current + $" {option.Level}.{option.Type}.{option.Value}")}"; // -1 = {ShellEffectValue} {FirstShell}...

                                default:
                                    return
                                        $"e_info 0 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {(ShellRarity == null ? "-1" : $"{ShellRarity}")} {(BoundCharacterId == null ? "0" : $"{BoundCharacterId}")} {options.Count}{options.OrderBy(s => s.Level).Aggregate(string.Empty, (current, option) => current + $" {option.Level}.{option.Type}.{option.Value}")}"; // -1 = {ShellEffectValue} {FirstShell}...
                            }

                        case EquipmentType.SecondaryWeapon:
                            switch (classe)
                            {
                                case 1:
                                    return
                                        $"e_info 1 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {(ShellRarity == null ? "-1" : $"{ShellRarity}")} {(BoundCharacterId == null ? "0" : $"{BoundCharacterId}")} {options.Count}{options.OrderBy(s => s.Level).Aggregate(string.Empty, (current, option) => current + $" {option.Level}.{option.Type}.{option.Value}")}"; // -1 = {ShellEffectValue} {FirstShell}...

                                case 2:
                                    return
                                        $"e_info 1 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {(ShellRarity == null ? "-1" : $"{ShellRarity}")} {(BoundCharacterId == null ? "0" : $"{BoundCharacterId}")} {options.Count}{options.OrderBy(s => s.Level).Aggregate(string.Empty, (current, option) => current + $" {option.Level}.{option.Type}.{option.Value}")}"; // -1 = {ShellEffectValue} {FirstShell}...

                                default:
                                    return
                                        $"e_info 0 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {(ShellRarity == null ? "-1" : $"{ShellRarity}")} {(BoundCharacterId == null ? "0" : $"{BoundCharacterId}")} {options.Count}{options.OrderBy(s => s.Level).Aggregate(string.Empty, (current, option) => current + $" {option.Level}.{option.Type}.{option.Value}")}"; // -1 = {ShellEffectValue} {FirstShell}...
                            }
                    }

                    break;

                case ItemType.Armor:
                    return
                        $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.Price} -1 {(ShellRarity == null ? "0" : $"{ShellRarity}")} {(BoundCharacterId == null ? "0" : $"{BoundCharacterId}")} {options.Count}{options.Aggregate(string.Empty, (current, option) => current + $" {(option.Level > 12 ? option.Level - 12 : option.Level)}.{(option.Type > 50 ? option.Type - 50 : option.Type)}.{option.Value}")}";

                case ItemType.Fashion:
                    switch (equipmentslot)
                    {
                        case EquipmentType.CostumeHat:
                            return
                                $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.Price} {(Item.ItemValidTime == 0 ? -1 : 0)} 2 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}";

                        case EquipmentType.CostumeSuit:
                            return
                                $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.Price} {(Item.ItemValidTime == 0 ? -1 : 0)} 1 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}"; // 1 = IsCosmetic -1 = no shells

                        default:
                            return
                                $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.Price} {Upgrade} 0 -1"; // after Item.Price theres TimesConnected {(Item.ItemValidTime == 0 ? -1 : Item.ItemValidTime / (3600))}
                    }

                case ItemType.Jewelery:
                    switch (equipmentslot)
                    {
                        case EquipmentType.Amulet:
                            if (DurabilityPoint > 0)
                            {
                                return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {DurabilityPoint} 100 0 {Item.Price}";
                            }

                            return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {seconds * 10} 0 0 {Item.Price}";

                        case EquipmentType.Fairy:
                            return
                                $"e_info 4 {ItemVNum} {Item.Element} {ElementRate + Item.ElementRate} 0 0 0 0 0"; // last IsNosmall

                        case EquipmentType.Necklace:
                        case EquipmentType.Bracelet:
                        case EquipmentType.Ring:
                            return
                                $"e_info 4 {ItemVNum} {Item.LevelMinimum} {Item.MaxCellonLvl} {Item.MaxCellon} {options.Count} {Item.Price}{options.Aggregate(string.Empty, (current, option) => current + $" {option.Type} {option.Level} {option.Value}")}";

                        default:
                            return
                                $"e_info 4 {ItemVNum} {Item.LevelMinimum} {Item.MaxCellonLvl} {Item.MaxCellon} {Cellon} {Item.Price}";
                    }

                case ItemType.Specialist:
                    return $"e_info 8 {ItemVNum}";

                case ItemType.Box:
                    if (GetType() != typeof(BoxInstance))
                    {
                        return $"e_info 7 {ItemVNum} 0";
                    }

                    var specialist = (BoxInstance)this;

                    // 0 = NOSMATE pearl 1= npc pearl 2 = sp box 3 = raid box 4= VEHICLE pearl
                    // 5=fairy pearl
                    switch (subtype)
                    {
                        case 0:
                            return specialist.HoldingVNum == 0
                                ? $"e_info 7 {ItemVNum} 0"
                                : $"e_info 7 {ItemVNum} 1 {specialist.HoldingVNum} {specialist.SpLevel} {specialist.XP} 100 {specialist.SpDamage} {specialist.SpDefence}";

                        case 2:
                            Item spitem = ServerManager.Instance.GetItem(specialist.HoldingVNum);
                            return specialist.HoldingVNum == 0
                                ? $"e_info 7 {ItemVNum} 0"
                                : $"e_info 7 {ItemVNum} 1 {specialist.HoldingVNum} {specialist.SpLevel} {specialist.XP} {CharacterHelper.Instance.SpxpData[specialist.SpLevel - 1]} {Upgrade} {CharacterHelper.Instance.SlPoint(specialist.SlDamage, 0)} {CharacterHelper.Instance.SlPoint(specialist.SlDefence, 1)} {CharacterHelper.Instance.SlPoint(specialist.SlElement, 2)} {CharacterHelper.Instance.SlPoint(specialist.SlHP, 3)} {CharacterHelper.Instance.SpPoint(specialist.SpLevel, Upgrade) - specialist.SlDamage - specialist.SlHP - specialist.SlElement - specialist.SlDefence} {specialist.SpStoneUpgrade} {spitem.FireResistance} {spitem.WaterResistance} {spitem.LightResistance} {spitem.DarkResistance} {specialist.SpDamage} {specialist.SpDefence} {specialist.SpElement} {specialist.SpHP} {specialist.SpFire} {specialist.SpWater} {specialist.SpLight} {specialist.SpDark}";

                        case 4:
                            return specialist.HoldingVNum == 0
                                ? $"e_info 11 {ItemVNum} 0"
                                : $"e_info 11 {ItemVNum} 1 {specialist.HoldingVNum}";

                        case 5:
                            Item fairyitem = ServerManager.Instance.GetItem(specialist.HoldingVNum);
                            return specialist.HoldingVNum == 0
                                ? $"e_info 12 {ItemVNum} 0"
                                : $"e_info 12 {ItemVNum} 1 {specialist.HoldingVNum} {specialist.ElementRate + fairyitem.ElementRate}";

                        default:
                            return $"e_info 8 {ItemVNum} {Design} {Rare}";
                    }

                case ItemType.Shell:
                    return
                        $"e_info 9 {ItemVNum} {Upgrade} {Rare} {Item.Price} {options.Count}{options.Aggregate(string.Empty, (current, option) => current + $" {(option.Level > 12 ? option.Level - 12 : option.Level)}.{(option.Type > 50 ? option.Type - 50 : option.Type)}.{option.Value}")}";
            }

            return string.Empty;
        }


        public void GenerateHeroicShell(RarifyProtection protection)
        {
            byte shellType;
            if (protection != RarifyProtection.RandomHeroicAmulet)
            {
                return;
            }

            if (Item.ItemType == ItemType.Jewelery)
            {
                return;
            }

            if (!Item.IsHeroic || Rare <= 0)
            {
                return;
            }

            if (Rare < 8)
            {
                shellType = (byte)(Item.ItemType == ItemType.Armor ? 11 : 10);
                if (shellType != 11 && shellType != 10)
                {
                    return;
                }
            }
            else
            {
                List<byte> possibleTypes = new List<byte> { 4, 5, 6, 7 };
                int probability = ServerManager.Instance.RandomNumber();
                shellType = (byte)(Item.ItemType == ItemType.Armor
                    ? probability > 50 ? 5 : 7
                    : probability > 50
                        ? 4
                        : 6);
                if (!possibleTypes.Contains(shellType))
                {
                    return;
                }
            }

            EquipmentOptions.Clear();
            int shellLevel = Item.LevelMinimum == 25 ? 101 : 106;
            EquipmentOptions.AddRange(ShellGeneratorHelper.Instance.GenerateShell(shellType, Rare == 8 ? 7 : Rare, shellLevel));
        }

        public void RarifyItem(ClientSession session, RarifyMode mode, RarifyProtection protection,
            bool isCommand = false)
        {
            double raren2 = 80;
            double raren1 = 70;
            double rare0 = 60;
            double rare1 = 40;
            double rare2 = 30;
            double rare3 = 15;
            double rare4 = 10;
            double rare5 = 5;
            double rare6 = 3;
            double rare7 = 2;
            double rare8 = 0.5;
            const short goldprice = 500;
            const double reducedpricefactor = 0.5;
            const double reducedchancefactor = 1.1;
            const byte cella = 5;
            const int cellaVnum = 1014;
            const int scrollVnum = 1218;
            double rnd;

            if (session != null && !session.HasCurrentMapInstance)
            {
                return;
            }

            if (mode != RarifyMode.Drop || Item.ItemType == ItemType.Shell)
            {
                raren2 = 0;
                raren1 = 0;
                rare0 = 0;
                rnd = ServerManager.Instance.RandomNumber(0, 80);
            }
            else
            {
                rnd = ServerManager.Instance.RandomNumber(0, 1000) / 10D;
            }

            if (protection == RarifyProtection.RedAmulet ||
                protection == RarifyProtection.HeroicAmulet ||
                protection == RarifyProtection.RandomHeroicAmulet)
            {
                raren2 = raren1 * reducedchancefactor;
                raren1 = raren1 * reducedchancefactor;
                rare0 = rare0 * reducedchancefactor;
                rare1 = rare1 * reducedchancefactor;
                rare2 = rare2 * reducedchancefactor;
                rare3 = rare3 * reducedchancefactor;
                rare4 = rare4 * reducedchancefactor;
                rare5 = rare5 * reducedchancefactor;
                rare6 = rare6 * reducedchancefactor;
                rare7 = rare7 * reducedchancefactor;
                rare8 = rare8 * reducedchancefactor;
            }

            if (session != null)
            {
                var amulet =
                    session.Character.Inventory.LoadBySlotAndType<WearableInstance>(
                        (short)EquipmentType.Amulet, InventoryType.Wear);
                switch (mode)
                {
                    case RarifyMode.Free:
                        break;
                    case RarifyMode.Reduce:
                        if (amulet == null)
                        {
                            return;
                        }

                        if (Rare < 8 || !Item.IsHeroic)
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_MAX_RARITY"),
                                    10));
                            return;
                        }

                        Rare -= (sbyte)ServerManager.Instance.RandomNumber(0, 7);
                        GenerateHeroicShell(protection);
                        SetRarityPoint();
                        ItemInstance inv = session.Character.Inventory.GetItemInstanceById(Id);
                        session.SendPacket(inv?.GenerateInventoryAdd());
                        session.Character.NotifyRarifyResult(Rare);
                        session.Character.DeleteItemByItemInstanceId(amulet.Id);
                        session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                        session.SendPacket(session.Character.GenerateEquipment());
                        LogHelper.Instance.InsertUpgradeLog(session, "bet", true, true, inv);
                        return;
                    case RarifyMode.Success:
                        if (amulet == null)
                        {
                            return;
                        }

                        if (Item.IsHeroic && Rare >= 8 || !Item.IsHeroic && Rare <= 7)
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ALREADY_MAX_RARE"),
                                    10));
                            return;
                        }

                        Rare += 1;
                        SetRarityPoint();
                        if (Item.IsHeroic)
                        {
                            GenerateHeroicShell(RarifyProtection.RandomHeroicAmulet);
                        }

                        ItemInstance inventory = session?.Character.Inventory.GetItemInstanceById(Id);
                        if (inventory != null)
                        {
                            session.SendPacket(inventory.GenerateInventoryAdd());
                            session.Character.NotifyRarifyResult(Rare);
                            session.Character.DeleteItemByItemInstanceId(amulet.Id);
                            session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                            session.SendPacket(session.Character.GenerateEquipment());
                        }

                        LogHelper.Instance.InsertUpgradeLog(session, "bet", true, true, inventory);
                        return;

                    case RarifyMode.Normal:
                        // TODO: Normal Item Amount
                        if (session.Character.Gold < goldprice)
                        {
                            return;
                        }

                        if (session.Character.Inventory.CountItem(cellaVnum) < cella)
                        {
                            return;
                        }

                        if (protection == RarifyProtection.Scroll && !isCommand &&
                            session.Character.Inventory.CountItem(scrollVnum) < 1)
                        {
                            return;
                        }

                        if ((protection == RarifyProtection.Scroll || protection == RarifyProtection.BlueAmulet ||
                            protection == RarifyProtection.RedAmulet) && !isCommand && Item.IsHeroic)
                        {
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("ITEM_IS_HEROIC"), 0));
                            return;
                        }

                        if ((protection == RarifyProtection.HeroicAmulet ||
                            protection == RarifyProtection.RandomHeroicAmulet) && !Item.IsHeroic)
                        {
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("ITEM_NOT_HEROIC"), 0));
                            return;
                        }

                        if (Item.IsHeroic && Rare == 8)
                        {
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("ALREADY_MAX_RARE"), 0));
                            return;
                        }

                        if (protection == RarifyProtection.Scroll && !isCommand)
                        {
                            session.Character.Inventory.RemoveItemAmount(scrollVnum);
                            session.SendPacket(session.Character.Inventory.CountItem(scrollVnum) < 1
                                ? "shop_end 2"
                                : "shop_end 1");
                        }

                        session.Character.Gold -= goldprice;
                        session.Character.Inventory.RemoveItemAmount(cellaVnum, cella);
                        session.SendPacket(session.Character.GenerateGold());
                        break;

                    case RarifyMode.Drop:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

            if (Item.IsHeroic && protection != RarifyProtection.None)
            {
                if (rnd < rare8)
                {
                    if (mode != RarifyMode.Drop)
                    {
                        session?.Character.NotifyRarifyResult(8);
                    }

                    Rare = 8;
                    GenerateHeroicShell(protection);
                    SetRarityPoint();
                    ItemInstance inventory = session?.Character.Inventory.GetItemInstanceById(Id);
                    if (inventory != null)
                    {
                        session.SendPacket(inventory.GenerateInventoryAdd());
                        LogHelper.Instance.InsertUpgradeLog(session, "bet", false, true, inventory);
                    }

                    return;
                }
            }

            if (rnd < rare7 && !(protection == RarifyProtection.Scroll && Rare >= 7))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(7);
                }

                Rare = 7;
                GenerateHeroicShell(protection);
                SetRarityPoint();
            }
            else if (rnd < rare6 && !(protection == RarifyProtection.Scroll && Rare >= 6))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(6);
                }

                Rare = 6;
                GenerateHeroicShell(protection);
                SetRarityPoint();
            }
            else if (rnd < rare5 && !(protection == RarifyProtection.Scroll && Rare >= 5))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(5);
                }

                Rare = 5;
                GenerateHeroicShell(protection);
                SetRarityPoint();
            }
            else if (rnd < rare4 && !(protection == RarifyProtection.Scroll && Rare >= 4))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(4);
                }

                Rare = 4;
                GenerateHeroicShell(protection);
                SetRarityPoint();
            }
            else if (rnd < rare3 && !(protection == RarifyProtection.Scroll && Rare >= 3))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(3);
                }

                Rare = 3;
                GenerateHeroicShell(protection);
                SetRarityPoint();
            }
            else if (rnd < rare2 && !(protection == RarifyProtection.Scroll && Rare >= 2))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(2);
                }

                Rare = 2;
                GenerateHeroicShell(protection);
                SetRarityPoint();
            }
            else if (rnd < rare1 && !(protection == RarifyProtection.Scroll && Rare >= 1))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(1);
                }

                Rare = 1;
                GenerateHeroicShell(protection);
                SetRarityPoint();
            }
            else if (rnd < rare0 && !(protection == RarifyProtection.Scroll && Rare >= 0))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(0);
                }

                Rare = 0;
                SetRarityPoint();
            }
            else if (rnd < raren1 && !(protection == RarifyProtection.Scroll && Rare >= -1))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(-1);
                }

                Rare = -1;
                SetRarityPoint();
            }
            else if (rnd < raren2 && !(protection == RarifyProtection.Scroll && Rare >= -2))
            {
                if (mode != RarifyMode.Drop)
                {
                    session?.Character.NotifyRarifyResult(-2);
                }

                Rare = -2;
                SetRarityPoint();
            }
            else if (Rare < 1 && Item.ItemType == ItemType.Shell)
            {
                Rare = 1;
            }
            else
            {
                if (mode != RarifyMode.Drop && session != null)
                {
                    ItemInstance item = session.Character.Inventory.GetItemInstanceById(Id);
                    switch (protection)
                    {
                        case RarifyProtection.BlueAmulet:
                        case RarifyProtection.RedAmulet:
                        case RarifyProtection.HeroicAmulet:
                        case RarifyProtection.RandomHeroicAmulet:
                            var amulet =
                                session.Character.Inventory.LoadBySlotAndType<WearableInstance>(
                                    (short)EquipmentType.Amulet, InventoryType.Wear);
                            if (amulet == null)
                            {
                                return;
                            }

                            amulet.DurabilityPoint -= 1;
                            if (amulet.DurabilityPoint <= 0)
                            {
                                session.Character.DeleteItemByItemInstanceId(amulet.Id);
                                session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                                session.SendPacket(session.Character.GenerateEquipment());
                            }

                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("AMULET_FAIL_SAVED"),
                                    11));
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("AMULET_FAIL_SAVED"), 0));
                            LogHelper.Instance.InsertUpgradeLog(session, "bet", true, false, item);
                            return;
                        case RarifyProtection.None:
                            session.Character.DeleteItemByItemInstanceId(Id);
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED"),
                                    11));
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 0));
                            LogHelper.Instance.InsertUpgradeLog(session, "bet", false, false, item);
                            return;
                    }

                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"),
                            11));
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(
                            Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 0));
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004), session.Character.MapX,
                        session.Character.MapY);
                    LogHelper.Instance.InsertUpgradeLog(session, "bet", true, false, item);
                    return;
                }
            }

            if (mode == RarifyMode.Drop || session == null)
            {
                return;
            }

            ItemInstance inventoryb = session.Character.Inventory.GetItemInstanceById(Id);
            if (inventoryb != null)
            {
                session.SendPacket(inventoryb.GenerateInventoryAdd());
            }
        }

        public void SetRarityPoint()
        {
            switch (Item.EquipmentSlot)
            {
                case EquipmentType.MainWeapon:
                case EquipmentType.SecondaryWeapon:
                {
                    int point = CharacterHelper.Instance.RarityPoint(Rare,
                        Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum);
                    Concentrate = 0;
                    HitRate = 0;
                    DamageMinimum = 0;
                    DamageMaximum = 0;
                    if (Rare >= 0)
                    {
                        for (int i = 0; i < point; i++)
                        {
                            int rndn = ServerManager.Instance.RandomNumber(0, 3);
                            if (rndn == 0)
                            {
                                Concentrate++;
                                HitRate++;
                            }
                            else
                            {
                                DamageMinimum++;
                                DamageMaximum++;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i > Rare * 10; i--)
                        {
                            DamageMinimum--;
                            DamageMaximum--;
                        }
                    }
                }
                    break;

                case EquipmentType.Armor:
                {
                    int point = CharacterHelper.Instance.RarityPoint(Rare,
                        Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum);
                    DefenceDodge = 0;
                    DistanceDefenceDodge = 0;
                    DistanceDefence = 0;
                    MagicDefence = 0;
                    CloseDefence = 0;
                    if (Rare >= 0)
                    {
                        for (int i = 0; i < point; i++)
                        {
                            int rndn = ServerManager.Instance.RandomNumber(0, 3);
                            if (rndn == 0)
                            {
                                DefenceDodge++;
                                DistanceDefenceDodge++;
                            }
                            else
                            {
                                DistanceDefence++;
                                MagicDefence++;
                                CloseDefence++;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i > Rare * 10; i--)
                        {
                            DistanceDefence--;
                            MagicDefence--;
                            CloseDefence--;
                        }
                    }
                }
                    break;
            }
        }

        public void Sum(ClientSession session, WearableInstance itemToSum)
        {
            if (!session.HasCurrentMapInstance)
            {
                return;
            }

            if (Upgrade >= 6)
            {
                return;
            }

            short[] upsuccess = { 100, 100, 85, 70, 50, 20 };
            int[] goldprice = { 1500, 3000, 6000, 12000, 24000, 48000 };
            short[] sand = { 5, 10, 15, 20, 25, 30 };
            const int sandVnum = 1027;
            if (Upgrade + itemToSum.Upgrade >= 6 || (itemToSum.Item.EquipmentSlot != EquipmentType.Gloves ||
                    Item.EquipmentSlot != EquipmentType.Gloves) &&
                (Item.EquipmentSlot != EquipmentType.Boots || itemToSum.Item.EquipmentSlot != EquipmentType.Boots))
            {
                return;
            }

            if (session.Character.Gold < goldprice[Upgrade + itemToSum.Upgrade])
            {
                return;
            }

            if (session.Character.Inventory.CountItem(sandVnum) < sand[Upgrade + itemToSum.Upgrade])
            {
                return;
            }

            session.Character.Inventory.RemoveItemAmount(sandVnum, (byte)sand[Upgrade + itemToSum.Upgrade]);
            session.Character.Gold -= goldprice[Upgrade + itemToSum.Upgrade];

            int rnd = ServerManager.Instance.RandomNumber();
            if (rnd < upsuccess[Upgrade + itemToSum.Upgrade])
            {
                Upgrade += (byte)(itemToSum.Upgrade + 1);
                DarkResistance += (short)(itemToSum.DarkResistance + itemToSum.Item.DarkResistance);
                LightResistance += (short)(itemToSum.LightResistance + itemToSum.Item.LightResistance);
                WaterResistance += (short)(itemToSum.WaterResistance + itemToSum.Item.WaterResistance);
                FireResistance += (short)(itemToSum.FireResistance + itemToSum.Item.FireResistance);
                session.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                session.SendPacket($"pdti 10 {ItemVNum} 1 27 {Upgrade} 0");
                session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_SUCCESS"), 0));
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_SUCCESS"), 12));
                session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateGuri(19, 1, session.Character.CharacterId, 1324));
                session.SendPacket(GenerateInventoryAdd());
            }
            else
            {
                session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_FAILED"), 0));
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_FAILED"),
                    11));
                session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateGuri(19, 1, session.Character.CharacterId, 1332));
                session.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                session.Character.DeleteItemByItemInstanceId(Id);
            }

            session.CurrentMapInstance?.Broadcast(
                UserInterfaceHelper.Instance.GenerateGuri(6, 1, session.Character.CharacterId), session.Character.MapX,
                session.Character.MapY);
            session.SendPacket(session.Character.GenerateGold());
            session.SendPacket("shop_end 1");
        }

        public void UpgradeSpFun(ClientSession CharacterSession, UpgradeProtection protect, int value)
        {
            if (CharacterSession == null)
            {
                return;
            }

            if (Upgrade >= 15)
            {
                return;
            }

            short ScrollChiken = 5107;
            short ScrollPyjama = 5207;
            short ScrollPirate = 5519;
            if (!CharacterSession.HasCurrentMapInstance)
            {
                return;
            }

            if (value == 1)
            {
                if (protect == UpgradeProtection.Protected)
                {
                    if (CharacterSession.Character.Inventory.CountItem(ScrollChiken) < 1)
                    {
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(
                            Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.Instance.GetItem(ScrollChiken).Name, 1)), 10));
                        return;
                    }

                    CharacterSession.Character.Inventory.RemoveItemAmount(ScrollChiken);
                    CharacterSession.SendPacket(CharacterSession.Character.Inventory.CountItem(ScrollChiken) < 1 ? "shop_end 2" : "shop_end 1");
                }
            }

            if (value == 3)
            {
                if (protect == UpgradeProtection.Protected)
                {
                    if (CharacterSession.Character.Inventory.CountItem(ScrollPirate) < 1)
                    {
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(
                            Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.Instance.GetItem(ScrollPirate).Name, 1)), 10));
                        return;
                    }

                    CharacterSession.Character.Inventory.RemoveItemAmount(ScrollPirate);
                    CharacterSession.SendPacket(CharacterSession.Character.Inventory.CountItem(ScrollPirate) < 1 ? "shop_end 2" : "shop_end 1");
                }
            }

            if (value == 2)
            {
                if (protect == UpgradeProtection.Protected)
                {
                    if (CharacterSession.Character.Inventory.CountItem(ScrollPyjama) < 1)
                    {
                        CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(
                            Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.Instance.GetItem(ScrollPyjama).Name, 1)), 10));
                        return;
                    }

                    CharacterSession.Character.Inventory.RemoveItemAmount(ScrollPyjama);
                    CharacterSession.SendPacket(CharacterSession.Character.Inventory.CountItem(ScrollPyjama) < 1 ? "shop_end 2" : "shop_end 1");
                }
            }

            ItemInstance wearable = CharacterSession.Character.Inventory.GetItemInstanceById(Id);
            ItemInstance inventory = CharacterSession.Character.Inventory.GetItemInstanceById(Id);
            int rnd = ServerManager.Instance.RandomNumber();
            if (protect == UpgradeProtection.Protected)
            {
                CharacterSession.CurrentMapInstance.Broadcast(CharacterSession.Character.GenerateEff(3004), CharacterSession.Character.MapX, CharacterSession.Character.MapY);
            }

            CharacterSession.CurrentMapInstance.Broadcast(CharacterSession.Character.GenerateEff(3005), CharacterSession.Character.MapX, CharacterSession.Character.MapY);
            CharacterSession.SendPacket(CharacterSession.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 12));
            CharacterSession.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 0));
            wearable.Upgrade++;
            if (wearable.Upgrade > 8)
            {
                CharacterSession.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, CharacterSession.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
            }

            CharacterSession.SendPacket(wearable.GenerateInventoryAdd());

            CharacterSession.SendPacket(CharacterSession.Character.GenerateGold());
            CharacterSession.SendPacket(CharacterSession.Character.GenerateEq());
            CharacterSession.SendPacket("shop_end 1");
        }

        public void UpgradeItem(ClientSession session, UpgradeMode mode, UpgradeProtection protection,
            bool isCommand = false, FixedUpMode hasAmulet = FixedUpMode.None)
        {
            if (!session.HasCurrentMapInstance)
            {
                return;
            }

            if (Upgrade >= 10)
            {
                return;
            }

            short[] upfail;
            short[] upfix;
            int[] goldprice;
            short[] cella;
            short[] gem;

            if (Rare >= 8)
            {
                upfix = new short[] { 50, 40, 70, 65, 80, 90, 95, 97, 98, 99 };
                upfail = new short[] { 50, 40, 60, 50, 60, 70, 75, 77, 83, 89 };

                goldprice = new[] { 5000, 15000, 30000, 100000, 300000, 800000, 1500000, 4000000, 7000000, 10000000 };
                cella = new short[] { 40, 100, 160, 240, 320, 440, 560, 760, 960, 1200 };
                gem = new short[] { 2, 2, 4, 4, 6, 2, 2, 4, 4, 6 };
            }
            else
            {
                upfix = new short[] { 0, 0, 10, 15, 20, 20, 10, 5, 3, 1 };
                upfail = new short[] { 0, 0, 0, 5, 20, 40, 70, 85, 92, 98 };

                goldprice = new[] { 500, 1500, 3000, 10000, 30000, 80000, 150000, 400000, 700000, 1000000 };
                cella = new short[] { 20, 50, 80, 120, 160, 220, 280, 380, 480, 600 };
                gem = new short[] { 1, 1, 2, 2, 3, 1, 1, 2, 2, 3 };
            }

            if (hasAmulet == FixedUpMode.HasAmulet && IsFixed)
            {
                upfix = new short[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            const short cellaVnum = 1014;
            const short gemVnum = 1015;
            const short gemFullVnum = 1016;
            const double reducedpricefactor = 0.5;
            const short normalScrollVnum = 1218;
            const short goldScrollVnum = 5369;

            if (IsFixed && hasAmulet == FixedUpMode.None)
            {
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ITEM_IS_FIXED"),
                    10));
                session.SendPacket("shop_end 2");
                return;
            }

            if (IsFixed && hasAmulet == FixedUpMode.HasAmulet)
            {
                IsFixed = !IsFixed;
            }

            switch (mode)
            {
                case UpgradeMode.Free:
                    break;

                case UpgradeMode.Reduced:

                    // TODO: Reduced Item Amount
                    if (session.Character.Gold < (long)(goldprice[Upgrade] * reducedpricefactor))
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                        return;
                    }

                    if (session.Character.Inventory.CountItem(cellaVnum) < cella[Upgrade] * reducedpricefactor)
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.Instance.GetItem(cellaVnum).Name, cella[Upgrade] * reducedpricefactor),
                            10));
                        return;
                    }

                    if (protection == UpgradeProtection.Protected && !isCommand &&
                        session.Character.Inventory.CountItem(goldScrollVnum) < 1)
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.Instance.GetItem(goldScrollVnum).Name,
                                cella[Upgrade] * reducedpricefactor), 10));
                        return;
                    }

                    if (Upgrade < 5)
                    {
                        if (session.Character.Inventory.CountItem(gemVnum) < gem[Upgrade])
                        {
                            session.SendPacket(session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                    ServerManager.Instance.GetItem(gemVnum).Name, gem[Upgrade]), 10));
                            return;
                        }

                        session.Character.Inventory.RemoveItemAmount(gemVnum, gem[Upgrade]);
                    }
                    else
                    {
                        if (session.Character.Inventory.CountItem(gemFullVnum) < gem[Upgrade])
                        {
                            session.SendPacket(session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                    ServerManager.Instance.GetItem(gemFullVnum).Name, gem[Upgrade]), 10));
                            return;
                        }

                        session.Character.Inventory.RemoveItemAmount(gemFullVnum, gem[Upgrade]);
                    }

                    if (protection == UpgradeProtection.Protected && !isCommand)
                    {
                        session.Character.Inventory.RemoveItemAmount(goldScrollVnum);
                        session.SendPacket(session.Character.Inventory.CountItem(goldScrollVnum) < 1
                            ? "shop_end 2"
                            : "shop_end 1");
                    }

                    if (hasAmulet == FixedUpMode.HasAmulet && IsFixed)
                    {
                        var amulet =
                            session.Character.Inventory.LoadBySlotAndType<WearableInstance>(
                                (short)EquipmentType.Amulet, InventoryType.Wear);
                        amulet.DurabilityPoint -= 1;
                        if (amulet.DurabilityPoint <= 0)
                        {
                            session.Character.DeleteItemByItemInstanceId(amulet.Id);
                            session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                            session.SendPacket(session.Character.GenerateEquipment());
                        }
                    }

                    session.Character.Gold -= (long)(goldprice[Upgrade] * reducedpricefactor);
                    session.Character.Inventory.RemoveItemAmount(cellaVnum,
                        (int)(cella[Upgrade] * reducedpricefactor));
                    session.SendPacket(session.Character.GenerateGold());
                    break;

                case UpgradeMode.Normal:

                    // TODO: Normal Item Amount
                    if (session.Character.Inventory.CountItem(cellaVnum) < cella[Upgrade])
                    {
                        return;
                    }

                    if (session.Character.Gold < goldprice[Upgrade])
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                        return;
                    }

                    if (protection == UpgradeProtection.Protected && !isCommand &&
                        session.Character.Inventory.CountItem(normalScrollVnum) < 1)
                    {
                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                ServerManager.Instance.GetItem(normalScrollVnum).Name, 1), 10));
                        return;
                    }

                    if (Upgrade < 5)
                    {
                        if (session.Character.Inventory.CountItem(gemVnum) < gem[Upgrade])
                        {
                            session.SendPacket(session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                    ServerManager.Instance.GetItem(gemVnum).Name, gem[Upgrade]), 10));
                            return;
                        }

                        session.Character.Inventory.RemoveItemAmount(gemVnum, gem[Upgrade]);
                    }
                    else
                    {
                        if (session.Character.Inventory.CountItem(gemFullVnum) < gem[Upgrade])
                        {
                            session.SendPacket(session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"),
                                    ServerManager.Instance.GetItem(gemFullVnum).Name, gem[Upgrade]), 10));
                            return;
                        }

                        session.Character.Inventory.RemoveItemAmount(gemFullVnum, gem[Upgrade]);
                    }

                    if (protection == UpgradeProtection.Protected && !isCommand)
                    {
                        session.Character.Inventory.RemoveItemAmount(normalScrollVnum);
                        session.SendPacket(session.Character.Inventory.CountItem(normalScrollVnum) < 1
                            ? "shop_end 2"
                            : "shop_end 1");
                    }

                    if (hasAmulet == FixedUpMode.HasAmulet && IsFixed)
                    {
                        var amulet =
                            session.Character.Inventory.LoadBySlotAndType<WearableInstance>(
                                (short)EquipmentType.Amulet, InventoryType.Wear);
                        amulet.DurabilityPoint -= 1;
                        if (amulet.DurabilityPoint <= 0)
                        {
                            session.Character.DeleteItemByItemInstanceId(amulet.Id);
                            session.SendPacket($"info {Language.Instance.GetMessageFromKey("AMULET_DESTROYED")}");
                            session.SendPacket(session.Character.GenerateEquipment());
                        }
                    }

                    session.Character.Inventory.RemoveItemAmount(cellaVnum, cella[Upgrade]);
                    session.Character.Gold -= goldprice[Upgrade];
                    session.SendPacket(session.Character.GenerateGold());
                    break;
            }

            var wearable = session.Character.Inventory.LoadByItemInstance<WearableInstance>(Id);

            int rnd = ServerManager.Instance.RandomNumber();
            if (Rare == 8)
            {
                if (rnd < upfail[Upgrade])
                {
                    if (protection == UpgradeProtection.None)
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                        session.SendPacket(
                            UserInterfaceHelper.Instance.GenerateMsg(
                                Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                        session.Character.DeleteItemByItemInstanceId(Id);
                    }
                    else
                    {
                        session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004),
                            session.Character.MapX, session.Character.MapY);
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"),
                                11));
                        session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                            Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                    }
                }
                else if (rnd < upfix[Upgrade])
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004), session.Character.MapX,
                        session.Character.MapY);
                    wearable.IsFixed = true;
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"),
                            0));
                }
                else
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3005), session.Character.MapX,
                        session.Character.MapY);
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"),
                            0));
                    wearable.Upgrade++;
                    if (wearable.Upgrade > 4)
                    {
                        session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name,
                            itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                    }

                    session.SendPacket(wearable.GenerateInventoryAdd());
                }
            }
            else
            {
                if (rnd < upfix[Upgrade])
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004), session.Character.MapX,
                        session.Character.MapY);
                    wearable.IsFixed = true;
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"),
                            0));
                }
                else if (rnd < upfail[Upgrade] + upfix[Upgrade])
                {
                    if (protection == UpgradeProtection.None)
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                        session.SendPacket(
                            UserInterfaceHelper.Instance.GenerateMsg(
                                Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                        session.Character.DeleteItemByItemInstanceId(Id);
                    }
                    else
                    {
                        session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3004),
                            session.Character.MapX, session.Character.MapY);
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"),
                                11));
                        session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                            Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                    }
                }
                else
                {
                    session.CurrentMapInstance.Broadcast(session.Character.GenerateEff(3005), session.Character.MapX,
                        session.Character.MapY);
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"),
                            0));
                    wearable.Upgrade++;
                    if (wearable.Upgrade > 4)
                    {
                        session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name,
                            itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                    }

                    session.SendPacket(wearable.GenerateInventoryAdd());
                }
            }

            session.SendPacket("shop_end 1");
        }

        #endregion
    }
}