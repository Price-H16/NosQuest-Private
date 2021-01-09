﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.DAL;
using OpenNos.GameObject.Buff;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items
{
    public class WearableItem : Item
    {
        #region Instantiation

        public WearableItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null)
        {
            switch (Effect)
            {
                default:
                    bool delay = false;
                    if (option == 255)
                    {
                        delay = true;
                        option = 0;
                    }

                    Mate mate = null;
                    if (option != 0)
                    {
                        if (session.Character.Mates.Count(s => s.MateType == MateType.Partner) == 1 && option == 2)
                        {
                            option = 1;
                        }

                        mate = session.Character.Mates.FirstOrDefault(s =>
                            s.MateType == MateType.Partner && s.PetId == (option - 1));
                    }

                    short slot = inv.Slot;
                    var equipment = InventoryType.Wear;
                    switch (option)
                    {
                        case 1:
                            equipment = InventoryType.FirstPartnerInventory;
                            break;

                        case 2:
                            equipment = InventoryType.SecondPartnerInventory;
                            break;

                        case 3:
                            equipment = InventoryType.ThirdPartnerInventory;
                            break;
                    }

                    InventoryType itemToWearType = inv.Type;

                    if (inv == null)
                    {
                        return;
                    }

                    if (ItemValidTime > 0 && !inv.IsBound)
                    {
                        inv.ItemDeleteTime = DateTime.Now.AddSeconds(ItemValidTime);
                    }

                    if (!inv.IsBound)
                    {
                        switch (inv.Item.Effect)
                        {
                            case 790: // Tarot
                            case 932: // Attack amulet
                            case 933: // defense amulet
                                inv.BoundCharacterId = session.Character.CharacterId;
                                break;
                        }

                        if (!delay &&
                            (EquipmentSlot == EquipmentType.Fairy && (MaxElementRate == 70 || MaxElementRate == 80) ||
                                EquipmentSlot == EquipmentType.CostumeHat || EquipmentSlot == EquipmentType.CostumeSuit ||
                                EquipmentSlot == EquipmentType.WeaponSkin))
                        {
                            session.SendPacket(
                                $"qna #u_i^1^{session.Character.CharacterId}^{(byte)itemToWearType}^{slot}^1 {Language.Instance.GetMessageFromKey("ASK_BIND")}");
                            return;
                        }

                        if (delay)
                        {
                            inv.BoundCharacterId = session.Character.CharacterId;
                        }
                    }

                    double timeSpanSinceLastSpUsage =
                        (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds -
                        session.Character.LastSp;

                    if (EquipmentSlot == EquipmentType.Sp && inv.Rare == -2)
                    {
                        session.SendPacket(
                            UserInterfaceHelper.Instance.GenerateMsg(
                                Language.Instance.GetMessageFromKey("CANT_EQUIP_DESTROYED_SP"), 0));
                        return;
                    }

                    if (option == 0)
                    {
                        if (EquipmentSlot == EquipmentType.Sp &&
                            timeSpanSinceLastSpUsage <= session.Character.SpCooldown &&
                            session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>((byte)EquipmentType.Sp,
                                InventoryType.Specialist) != null)
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                string.Format(Language.Instance.GetMessageFromKey("SP_INLOADING"),
                                    session.Character.SpCooldown - (int)Math.Round(timeSpanSinceLastSpUsage)), 0));
                            return;
                        }

                        if (ItemType != ItemType.Weapon && ItemType != ItemType.Armor && ItemType != ItemType.Fashion &&
                            ItemType != ItemType.Jewelery && ItemType != ItemType.Specialist ||
                            LevelMinimum > (IsHeroic ? session.Character.HeroLevel : session.Character.Level) ||
                            Sex != 0 && Sex != ((byte)session.Character.Gender + 1)
                            || ItemType != ItemType.Jewelery && EquipmentSlot != EquipmentType.Boots &&
                            EquipmentSlot != EquipmentType.Gloves && (Class >> (byte)session.Character.Class & 1) != 1)
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("BAD_EQUIPMENT"),
                                    10));
                            return;
                        }

                        if (session.Character.UseSp)
                        {
                            var sp =
                                session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>(
                                    (byte)EquipmentType.Sp, equipment);

                            if (sp != null && sp.Item.Element != 0 && EquipmentSlot == EquipmentType.Fairy &&
                                Element != sp.Item.Element && Element != sp.Item.SecondaryElement)
                            {
                                session.SendPacket(
                                    UserInterfaceHelper.Instance.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("BAD_FAIRY"), 0));
                                return;
                            }
                        }

                        if (ItemType == ItemType.Weapon || ItemType == ItemType.Armor)
                        {
                            if (inv.BoundCharacterId.HasValue &&
                                inv.BoundCharacterId.Value != session.Character.CharacterId)
                            {
                                session.SendPacket(
                                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("BAD_EQUIPMENT"),
                                        10));
                                return;
                            }
                        }

                        if (session.Character.UseSp && EquipmentSlot == EquipmentType.Sp)
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SP_BLOCKED"), 10));
                            return;
                        }

                        if (session.Character.JobLevel < LevelJobMinimum)
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOW_JOB_LVL"), 10));
                            return;
                        }
                    }
                    else if (mate != null)
                    {
                        if (mate.Level < LevelMinimum)
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("BAD_EQUIPMENT"),
                                    10));
                            return;
                        }

                        switch (EquipmentSlot)
                        {
                            case EquipmentType.Armor:
                                if (ItemSubType == 4)
                                {
                                    if (MateHelper.Instance.CanWearItem(inv.ItemVNum, mate.Monster.NpcMonsterVNum, session))
                                    {
                                        mate.ArmorInstance = inv;
                                    }
                                    else
                                    {
                                        goto default;
                                    }

                                    break;
                                }
                                else
                                {
                                    goto default;
                                }

                            case EquipmentType.MainWeapon:
                                if (ItemSubType == 12)
                                {
                                    if (MateHelper.Instance.CanWearItem(inv.ItemVNum, mate.Monster.NpcMonsterVNum, session))
                                    {
                                        mate.WeaponInstance = inv;
                                    }
                                    else
                                    {
                                        goto default;
                                    }

                                    break;
                                }
                                else
                                {
                                    goto default;
                                }

                            case EquipmentType.Gloves:
                                mate.GlovesInstance = inv;
                                break;

                            case EquipmentType.Boots:
                                mate.BootsInstance = inv;
                                break;

                            case EquipmentType.Sp:
                                if (ItemSubType == 4)
                                {
                                    if (MateHelper.Instance.CanWearItem(inv.ItemVNum, mate.Monster.NpcMonsterVNum, session))
                                    {
                                        mate.SpInstance = (SpecialistInstance)inv;
                                    }
                                    else
                                    {
                                        goto default;
                                    }

                                    break;
                                }
                                else
                                {
                                    goto default;
                                }

                            default:
                                session.SendPacket(
                                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("BAD_EQUIPMENT"),
                                        10));
                                return;
                        }
                    }

                    ItemInstance currentlyEquippedItem =
                        session.Character.Inventory.LoadBySlotAndType((short)EquipmentSlot, equipment);

                    if (currentlyEquippedItem == null)
                    {
                        // move from equipment to wear
                        session.Character.Inventory.MoveInInventory(inv.Slot, itemToWearType, equipment);
                        session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(itemToWearType, slot));
                    }
                    else
                    {
                        // move from wear to equipment and back
                        session.Character.Inventory.MoveInInventory(currentlyEquippedItem.Slot, equipment,
                            itemToWearType, inv.Slot);
                        session.SendPacket(currentlyEquippedItem.GenerateInventoryAdd());
                        session.Character.BattleEntity.StaticBcards.RemoveWhere(
                            o => o.ItemVNum != currentlyEquippedItem.ItemVNum, out ConcurrentBag<BCard> eqBcards);
                        session.Character.BattleEntity.StaticBcards = eqBcards;
                    }

                    if (inv.Item.ItemType != ItemType.Fashion)
                    {
                        inv.Item.BCards.ForEach(s => session.Character.BattleEntity.StaticBcards.Add(s));
                    }

                    if (inv is WearableInstance wearableInstance)
                    {
                        var specialistInstance =
                            session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>((byte)EquipmentType.Sp,
                                InventoryType.Wear);

                        if (wearableInstance.EquipmentOptions != null)
                        {
                            switch (wearableInstance.Item.ItemType)
                            {
                                case ItemType.Armor:
                                case ItemType.Weapon:
                                case ItemType.Jewelery:
                                case ItemType.Fashion:
                                    switch (wearableInstance.Slot)
                                    {
                                        case (byte)EquipmentType.CostumeHat:
                                            session.Character.BattleEntity.CostumeHatBcards.Clear();

                                            foreach (BCard bc in wearableInstance.Item.BCards)
                                            {
                                                session.Character.BattleEntity.CostumeHatBcards.Add(bc);
                                            }

                                            break;
                                        case (byte)EquipmentType.CostumeSuit:
                                            session.Character.BattleEntity.CostumeSuitBcards.Clear();

                                            foreach (BCard bc in wearableInstance.Item.BCards)
                                            {
                                                session.Character.BattleEntity.CostumeSuitBcards.Add(bc);
                                            }

                                            break;
                                        case (byte)EquipmentType.Armor:
                                            session.Character.Inventory.Armor = wearableInstance;
                                            session.Character.ShellOptionArmor.Clear();

                                            foreach (EquipmentOptionDTO dto in DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(inv.Id))
                                            {
                                                session.Character.ShellOptionArmor.Add(dto);
                                            }

                                            EquipmentOptionHelper.Instance
                                                .ShellToBCards(wearableInstance.EquipmentOptions,
                                                    wearableInstance.ItemVNum)
                                                .ForEach(s => session.Character.BattleEntity.StaticBcards.Add(s));
                                            break;
                                        case (byte)EquipmentType.MainWeapon:
                                            session.Character.ShellOptionsMain.Clear();

                                            foreach (EquipmentOptionDTO dto in DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(inv.Id))
                                            {
                                                session.Character.ShellOptionsMain.Add(dto);
                                            }

                                            session.Character.Inventory.PrimaryWeapon = wearableInstance;
                                            EquipmentOptionHelper.Instance
                                                .ShellToBCards(wearableInstance.EquipmentOptions,
                                                    wearableInstance.ItemVNum)
                                                .ForEach(s => session.Character.BattleEntity.StaticBcards.Add(s));
                                            specialistInstance?.RestorePoints(session, specialistInstance);
                                            break;
                                        case (byte)EquipmentType.SecondaryWeapon:
                                            session.Character.ShellOptionsSecondary.Clear();

                                            foreach (EquipmentOptionDTO dto in DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(inv.Id))
                                            {
                                                session.Character.ShellOptionsSecondary.Add(dto);
                                            }

                                            session.Character.Inventory.SecondaryWeapon = wearableInstance;
                                            EquipmentOptionHelper.Instance
                                                .ShellToBCards(wearableInstance.EquipmentOptions,
                                                    wearableInstance.ItemVNum)
                                                .ForEach(s => session.Character.BattleEntity.StaticBcards.Add(s));
                                            specialistInstance?.RestorePoints(session, specialistInstance);
                                            break;
                                        case (byte)EquipmentType.Ring:
                                        case (byte)EquipmentType.Necklace:
                                        case (byte)EquipmentType.Bracelet:
                                            EquipmentOptionHelper.Instance
                                                .CellonToBCards(wearableInstance.EquipmentOptions,
                                                    wearableInstance.ItemVNum)
                                                .ForEach(s => session.Character.BattleEntity.StaticBcards.Add(s));
                                            session.Character.BattleEntity.CellonOptions.Clear();
                                            var ring = session.Character.Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Ring, InventoryType.Wear);
                                            var bracelet = session.Character.Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Bracelet, InventoryType.Wear);
                                            var necklace = session.Character.Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Necklace, InventoryType.Wear);
                                            List<EquipmentOptionDTO> options = new List<EquipmentOptionDTO>();
                                            if (ring?.EquipmentOptions != null)
                                            {
                                                options.AddRange(ring?.EquipmentOptions);
                                            }

                                            if (bracelet?.EquipmentOptions != null)
                                            {
                                                options.AddRange(bracelet?.EquipmentOptions);
                                            }

                                            if (necklace?.EquipmentOptions != null)
                                            {
                                                options.AddRange(necklace?.EquipmentOptions);
                                            }

                                            foreach (EquipmentOptionDTO opt in options)
                                            {
                                                session.Character.BattleEntity.CellonOptions.Add(opt);
                                            }

                                            break;
                                    }

                                    break;
                            }
                        }
                    }

                    if (option == 0)
                    {
                        session.SendPacket(session.Character.GenerateStatChar());
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateEq());
                        session.SendPacket(session.Character.GenerateEquipment());
                        session.CurrentMapInstance?.Broadcast(session.Character.GeneratePairy());

                        switch (EquipmentSlot)
                        {
                            case EquipmentType.Fairy:
                                var fairy =
                                    session.Character.Inventory.LoadBySlotAndType<WearableInstance>(
                                        (byte)EquipmentType.Fairy, equipment);
                                session.SendPacket(session.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("FAIRYSTATS"), fairy.XP,
                                        CharacterHelper.LoadFairyXpData(fairy.ElementRate + fairy.Item.ElementRate)),
                                    10));
                                break;
                            case EquipmentType.Amulet:
                                session.SendPacket(session.Character.GenerateEff(39));
                                break;
                        }
                    }
                    else if (mate != null)
                    {
                        session.SendPacket(mate.GenerateScPacket());
                    }

                    break;
            }
        }

        #endregion
    }
}