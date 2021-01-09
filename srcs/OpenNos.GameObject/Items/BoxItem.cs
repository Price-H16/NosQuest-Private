﻿// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Npc;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items
{
    public class BoxItem : Item
    {
        #region Instantiation

        public BoxItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null)
        {
            if (session == null)
            {
                return;
            }

            switch (Effect)
            {
                case 0:
                case 999:
                    if (option == 0)
                    {
                        if (packetsplit != null && packetsplit.Length == 9)
                        {
                            var box =
                                session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot,
                                    InventoryType.Equipment);
                            if (box != null)
                            {
                                if (box.Item.ItemSubType == 3)
                                {
                                    session.SendPacket(
                                        $"qna #guri^300^8023^{inv.Slot} {Language.Instance.GetMessageFromKey("ASK_OPEN_BOX")}");
                                }
                                else if (box.HoldingVNum == 0)
                                {
                                    session.SendPacket(
                                        $"qna #guri^300^8023^{inv.Slot}^{packetsplit[3]} {Language.Instance.GetMessageFromKey("ASK_STORE_PET")}");
                                }
                                else
                                {
                                    session.SendPacket(
                                        $"qna #guri^300^8023^{inv.Slot} {Language.Instance.GetMessageFromKey("ASK_RELEASE_PET")}");
                                }
                            }
                        }
                    }
                    else
                    {
                        //u_i 2 2000000 0 21 0 0
                        var box =
                            session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot,
                                InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.Item.ItemSubType == 3)
                            {
                                List<RollGeneratedItemDTO> roll = box.Item.RollGeneratedItems.Where(s =>
                                    s.MinimumOriginalItemRare <= box.Rare && s.MaximumOriginalItemRare >= box.Rare &&
                                    s.OriginalItemDesign == box.Design).ToList();
                                int probabilities = roll.Sum(s => s.Probability);
                                int rnd = ServerManager.Instance.RandomNumber(0, probabilities);
                                int currentrnd = 0;
                                List<ItemInstance> newInv = null;
                                foreach (RollGeneratedItemDTO rollitem in roll)
                                {
                                    Item createdItem = ServerManager.Instance.GetItem(rollitem.ItemGeneratedVNum);
                                    if (newInv != null)
                                    {
                                        continue;
                                    }

                                    currentrnd += rollitem.Probability;
                                    if (currentrnd < rnd)
                                    {
                                        continue;
                                    }

                                    if (createdItem.ItemType == ItemType.Shell)
                                    {
                                        rollitem.ItemGeneratedUpgrade = (byte)ServerManager.Instance.RandomNumber();
                                        if (rollitem.ItemGeneratedUpgrade >= 95)
                                        {
                                            rollitem.ItemGeneratedUpgrade =
                                                (byte)ServerManager.Instance.RandomNumber(80, 91);
                                        }
                                        else
                                        {
                                            rollitem.ItemGeneratedUpgrade =
                                                (byte)ServerManager.Instance.RandomNumber(70, 80);
                                        }
                                    }

                                    newInv = session.Character.Inventory.AddNewToInventory(rollitem.ItemGeneratedVNum,
                                        (ushort)rollitem.ItemGeneratedAmount, rare: box.Rare,
                                        upgrade: rollitem.ItemGeneratedUpgrade);
                                    if (newInv.Count == 0)
                                    {
                                        continue;
                                    }

                                    short slot = inv.Slot;
                                    if (slot == -1)
                                    {
                                        continue;
                                    }

                                    session.SendPacket(session.Character.GenerateSay(
                                        $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newInv.FirstOrDefault()?.Item.Name ?? ""} x {rollitem.ItemGeneratedAmount})",
                                        12));
                                    session.SendPacket(
                                        $"rdi {rollitem.ItemGeneratedVNum} {rollitem.ItemGeneratedAmount}");
                                    newInv.ForEach(s => session.SendPacket(s?.GenerateInventoryAdd()));
                                    session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                }
                            }
                            else if (box.HoldingVNum == 0)
                            {
                                if (packetsplit != null && packetsplit.Length == 1)
                                {
                                    if (int.TryParse(packetsplit[0], out int petId))
                                    {
                                        Mate mate = session.Character.Mates.FirstOrDefault(s =>
                                            s.MateTransportId == petId);
                                        if (mate == null)
                                        {
                                            return;
                                        }

                                        box.MateType = mate.MateType;
                                        box.HoldingVNum = mate.NpcMonsterVNum;
                                        box.SpLevel = mate.Level;
                                        box.SpDamage = mate.Attack;
                                        box.SpDefence = mate.Defence;
                                        session.Character.Mates.Remove(mate);
                                        session.SendPacket(
                                            UserInterfaceHelper.Instance.GenerateInfo(
                                                Language.Instance.GetMessageFromKey("PET_STORED")));
                                        session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
                                        session.SendPackets(session.Character.GenerateScP());
                                        session.SendPackets(session.Character.GenerateScN());
                                        session.CurrentMapInstance?.Broadcast(mate.GenerateOut());
                                    }
                                }
                            }
                            else
                            {
                                NpcMonster heldMonster = ServerManager.Instance.GetNpc(box.HoldingVNum);

                                if (heldMonster != null)
                                {
                                    var mate = new Mate(session.Character, heldMonster,
                                        (byte)(box.SpLevel == 0 ? 1 : box.SpLevel), box.MateType)
                                    {
                                        Attack = box.SpDamage,
                                        Defence = box.SpDefence
                                    };
                                    if (session.Character.AddPet(mate))
                                    {
                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                        session.SendPacket(
                                            UserInterfaceHelper.Instance.GenerateInfo(
                                                Language.Instance.GetMessageFromKey("PET_LEAVE_BEAD")));
                                    }
                                }
                            }
                        }
                    }

                    break;

                case 1:
                    if (option == 0)
                    {
                        session.SendPacket(
                            $"qna #guri^300^8023^{inv.Slot} {Language.Instance.GetMessageFromKey("ASK_RELEASE_PET")}");
                    }
                    else
                    {
                        NpcMonster heldMonster = ServerManager.Instance.GetNpc((short)EffectValue);
                        if (session.CurrentMapInstance == session.Character.Miniland && heldMonster != null)
                        {
                            var mate = new Mate(session.Character, heldMonster, LevelMinimum,
                                ItemSubType == 1 ? MateType.Partner : MateType.Pet);
                            if (session.Character.AddPet(mate))
                            {
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                session.SendPacket(
                                    UserInterfaceHelper.Instance.GenerateInfo(
                                        Language.Instance.GetMessageFromKey("PET_LEAVE_BEAD")));
                            }
                        }
                    }

                    break;

                case 6969:
                    if (EffectValue == 1 || EffectValue == 2)
                    {
                        var box =
                            session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot,
                                InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"wopen 44 {inv.Slot} 1");
                            }
                            else
                            {
                                List<ItemInstance> newInv =
                                    session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Count > 0)
                                {
                                    ItemInstance itemInstance = newInv.First();
                                    var specialist =
                                        session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>(
                                            itemInstance.Slot, itemInstance.Type);
                                    if (specialist != null)
                                    {
                                        specialist.SkillRank1 = box.SkillRank1;
                                        specialist.SkillRank2 = box.SkillRank2;
                                        specialist.SkillRank3 = box.SkillRank3;
                                        specialist.PartnerSkill1 = box.PartnerSkill1;
                                        specialist.PartnerSkill2 = box.PartnerSkill2;
                                        specialist.PartnerSkill3 = box.PartnerSkill3;
                                    }

                                    short slot = inv.Slot;
                                    if (slot != -1)
                                    {
                                        if (specialist != null)
                                        {
                                            session.SendPacket(session.Character.GenerateSay(
                                                $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {specialist.Item.Name}",
                                                12));
                                            newInv.ForEach(s => session.SendPacket(specialist.GenerateInventoryAdd()));
                                        }

                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }

                    break;

                case 69:
                    if (EffectValue == 1 || EffectValue == 2)
                    {
                        var box =
                            session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot,
                                InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"wopen 44 {inv.Slot} 0");
                            }
                            else
                            {
                                List<ItemInstance> newInv =
                                    session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Count > 0)
                                {
                                    ItemInstance itemInstance = newInv.First();
                                    var specialist =
                                        session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>(
                                            itemInstance.Slot, itemInstance.Type);
                                    if (specialist != null)
                                    {
                                        specialist.SlDamage = box.SlDamage;
                                        specialist.SlDefence = box.SlDefence;
                                        specialist.SlElement = box.SlElement;
                                        specialist.SlHP = box.SlHP;
                                        specialist.SpDamage = box.SpDamage;
                                        specialist.SpDark = box.SpDark;
                                        specialist.SpDefence = box.SpDefence;
                                        specialist.SpElement = box.SpElement;
                                        specialist.SpFire = box.SpFire;
                                        specialist.SpHP = box.SpHP;
                                        specialist.SpLevel = box.SpLevel;
                                        specialist.SpLight = box.SpLight;
                                        specialist.SpStoneUpgrade = box.SpStoneUpgrade;
                                        specialist.SpWater = box.SpWater;
                                        specialist.Upgrade = box.Upgrade;
                                        specialist.XP = box.XP;
                                    }

                                    short slot = inv.Slot;
                                    if (slot != -1)
                                    {
                                        if (specialist != null)
                                        {
                                            session.SendPacket(session.Character.GenerateSay(
                                                $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {specialist.Item.Name} + {specialist.Upgrade}",
                                                12));
                                            newInv.ForEach(s => session.SendPacket(specialist.GenerateInventoryAdd()));
                                        }

                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }

                    if (EffectValue == 3)
                    {
                        var box =
                            session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot,
                                InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"guri 26 0 {inv.Slot}");
                            }
                            else
                            {
                                List<ItemInstance> newInv =
                                    session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Count > 0)
                                {
                                    ItemInstance itemInstance = newInv.First();
                                    var fairy =
                                        session.Character.Inventory.LoadBySlotAndType<WearableInstance>(
                                            itemInstance.Slot, itemInstance.Type);
                                    if (fairy != null)
                                    {
                                        fairy.ElementRate = box.ElementRate;
                                    }

                                    short slot = inv.Slot;
                                    if (slot != -1)
                                    {
                                        if (fairy != null)
                                        {
                                            session.SendPacket(session.Character.GenerateSay(
                                                $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {fairy.Item.Name} ({fairy.ElementRate}%)",
                                                12));
                                            newInv.ForEach(s => session.SendPacket(fairy.GenerateInventoryAdd()));
                                        }

                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }

                    if (EffectValue == 4)
                    {
                        var box =
                            session.Character.Inventory.LoadBySlotAndType<BoxInstance>(inv.Slot,
                                InventoryType.Equipment);
                        if (box != null)
                        {
                            if (box.HoldingVNum == 0)
                            {
                                session.SendPacket($"guri 24 0 {inv.Slot}");
                            }
                            else
                            {
                                List<ItemInstance> newInv =
                                    session.Character.Inventory.AddNewToInventory(box.HoldingVNum);
                                if (newInv.Count > 0)
                                {
                                    short slot = inv.Slot;
                                    if (slot != -1)
                                    {
                                        session.SendPacket(session.Character.GenerateSay(
                                            $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newInv.First().Item.Name} x 1)",
                                            12));
                                        newInv.ForEach(s => session.SendPacket(s.GenerateInventoryAdd()));
                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, box.Id);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                                }
                            }
                        }
                    }

                    break;

                default:
                    Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
                    break;
            }
        }

        #endregion
    }
}