﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.DAL;
using OpenNos.GameObject.Bazaar;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Packets.Enums;

namespace WingsEmu.PacketHandlers
{
    public class BazaarPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BazaarPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     c_buy packet
        /// </summary>
        /// <param name="cBuyPacket"></param>
        public void BuyBazaar(CBuyPacket cBuyPacket)
        {
            if (ServerManager.Instance.InShutdown || Session.Character == null || Session.Character.InExchangeOrTrade)
            {
                return;
            }

            BazaarItemDTO bz = DaoFactory.Instance.BazaarItemDao.LoadAll().FirstOrDefault(s => s.BazaarItemId == cBuyPacket.BazaarId);
            if (bz != null && cBuyPacket.Amount > 0)
            {
                long price = cBuyPacket.Amount * bz.Price;

                if (Session.Character.Gold >= price)
                {
                    var bzcree = new BazaarItemLink { BazaarItem = bz };
                    if (DaoFactory.Instance.CharacterDao.LoadById(bz.SellerId) != null)
                    {
                        bzcree.Owner = DaoFactory.Instance.CharacterDao.LoadById(bz.SellerId)?.Name;
                        bzcree.Item = (ItemInstance)DaoFactory.Instance.IteminstanceDao.LoadById(bz.ItemInstanceId);
                    }

                    if (cBuyPacket.Amount <= bzcree.Item.Amount)
                    {
                        if (!Session.Character.Inventory.CanAddItem(bzcree.Item.ItemVNum))
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                            return;
                        }

                        if (bzcree.Item == null)
                        {
                            return;
                        }

                        if (bz.IsPackage && cBuyPacket.Amount != bz.Amount)
                        {
                            return;
                        }

                        ItemInstanceDTO bzitemdto = DaoFactory.Instance.IteminstanceDao.LoadById(bzcree.BazaarItem.ItemInstanceId);
                        if (bzitemdto.Amount < cBuyPacket.Amount)
                        {
                            return;
                        }

                        bzitemdto.Amount -= (ushort)cBuyPacket.Amount;
                        Session.Character.Gold -= price;
                        Session.SendPacket(Session.Character.GenerateGold());
                        DaoFactory.Instance.IteminstanceDao.InsertOrUpdate(bzitemdto);
                        ServerManager.Instance.BazaarRefresh(bzcree.BazaarItem.BazaarItemId);
                        Session.SendPacket($"rc_buy 1 {bzcree.Item.Item.VNum} {bzcree.Owner} {cBuyPacket.Amount} {cBuyPacket.Price} 0 0 0");
                        ItemInstance newBz = bzcree.Item.DeepCopy();
                        newBz.Id = Guid.NewGuid();
                        newBz.Amount = (ushort)cBuyPacket.Amount;
                        newBz.Type = newBz.Item.Type;
                        if (newBz is WearableInstance wear)
                        {
                            wear.EquipmentOptions.AddRange(DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(bzcree.Item.Id));
                            wear.EquipmentOptions.ForEach(s => s.WearableInstanceId = newBz.Id);
                            wear.EquipmentOptions.ForEach(s => DaoFactory.Instance.EquipmentOptionDao.InsertOrUpdate(s));
                        }


                        List<ItemInstance> newInv = Session.Character.Inventory.AddToInventory(newBz);
                        if (newInv.Any())
                        {
                            Session.SendPacket(Session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {bzcree.Item.Item.Name} x {cBuyPacket.Amount}", 10));
                        }
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateModal(Language.Instance.GetMessageFromKey("STATE_CHANGED"), 1));
                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateModal(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 1));
                }
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateModal(Language.Instance.GetMessageFromKey("STATE_CHANGED"), 1));
            }
        }

        /// <summary>
        ///     c_scalc packet
        /// </summary>
        /// <param name="cScalcPacket"></param>
        public void GetBazaar(CScalcPacket cScalcPacket)
        {
            if (ServerManager.Instance.InShutdown || Session.Character == null || Session.Character.InExchangeOrTrade)
            {
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            BazaarItemDTO bz = DaoFactory.Instance.BazaarItemDao.LoadAll().FirstOrDefault(s => s.BazaarItemId == cScalcPacket.BazaarId);
            if (bz != null)
            {
                var item = (ItemInstance)DaoFactory.Instance.IteminstanceDao.LoadById(bz.ItemInstanceId);
                if (item == null || bz.SellerId != Session.Character.CharacterId)
                {
                    return;
                }

                int soldedamount = bz.Amount - item.Amount;
                long taxes = bz.MedalUsed ? 0 : (long)(bz.Price * 0.10 * soldedamount);
                long price = bz.Price * soldedamount - taxes;
                if (Session.Character.Inventory.CanAddItem(item.ItemVNum))
                {
                    if (Session.Character.Gold + price <= ServerManager.Instance.MaxGold)
                    {
                        Session.Character.Gold += price;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REMOVE_FROM_BAZAAR"), price), 10));
                        if (item.Amount != 0)
                        {
                            ItemInstance newBz = item.DeepCopy();
                            newBz.Id = Guid.NewGuid();
                            newBz.Type = newBz.Item.Type;
                            if (newBz is WearableInstance wear)
                            {
                                wear.EquipmentOptions.AddRange(DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(item.Id));
                                wear.EquipmentOptions.ForEach(s => s.WearableInstanceId = newBz.Id);
                                wear.EquipmentOptions.ForEach(s => DaoFactory.Instance.EquipmentOptionDao.InsertOrUpdate(s));
                            }

                            Session.Character.Inventory.AddToInventory(newBz);
                        }

                        Session.SendPacket($"rc_scalc 1 {bz.Price} {bz.Amount - item.Amount} {bz.Amount} {taxes} {price + taxes}");

                        if (DaoFactory.Instance.BazaarItemDao.LoadById(bz.BazaarItemId) != null)
                        {
                            DaoFactory.Instance.BazaarItemDao.Delete(bz.BazaarItemId);
                        }

                        DaoFactory.Instance.IteminstanceDao.Delete(item.Id);

                        ServerManager.Instance.BazaarRefresh(bz.BazaarItemId);
                    }
                    else
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0));
                        Session.SendPacket($"rc_scalc 1 {bz.Price} 0 {bz.Amount} 0 0");
                    }
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE")));
                    Session.SendPacket($"rc_scalc 1 {bz.Price} 0 {bz.Amount} 0 0");
                }
            }
            else
            {
                Session.SendPacket("rc_scalc 1 0 0 0 0 0");
            }
        }

        /// <summary>
        ///     c_skill packet
        /// </summary>
        /// <param name="cSkillPacket"></param>
        public void OpenBazaar(CSkillPacket cSkillPacket)
        {
            if (ServerManager.Instance.InShutdown || Session.Character == null || Session.Character.InExchangeOrTrade)
            {
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            if (Session.Character.Authority >= AuthorityType.Vip && Session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.BazaarMedalGold))
            {
                Session.Character.StaticBonusList.Add(new StaticBonusDTO
                {
                    CharacterId = Session.Character.CharacterId,
                    DateEnd = DateTime.Now.AddDays(30),
                    StaticBonusType = StaticBonusType.BazaarMedalGold
                });
            }

            StaticBonusDTO medalBonus =
                Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);
            if (medalBonus != null)
            {
                byte medal = medalBonus.StaticBonusType == StaticBonusType.BazaarMedalGold || (int)Session.Character.Authority >= 1 ? (byte)MedalType.Gold : (byte)MedalType.Silver;
                int time = (int)Session.Character.Authority >= 1 ? 720 : (int)(medalBonus.DateEnd - DateTime.Now).TotalHours;
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOTICE_BAZAAR"), 0));
                Session.SendPacket($"wopen 32 {medal} {time}");
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
            }
        }

        /// <summary>
        ///     c_blist packet
        /// </summary>
        /// <param name="cbListPacket"></param>
        public void RefreshBazarList(CbListPacket cbListPacket)
        {
            if (ServerManager.Instance.InShutdown)
            {
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateRcbList(cbListPacket));
        }

        /// <summary>
        ///     c_slist packet
        /// </summary>
        /// <param name="csListPacket"></param>
        public void RefreshPersonalBazarList(CsListPacket csListPacket)
        {
            if (ServerManager.Instance.InShutdown)
            {
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            Session.SendPacket(Session.Character.GenerateRcsList(csListPacket));
        }

        /// <summary>
        ///     c_reg packet
        /// </summary>
        /// <param name="cRegPacket"></param>
        public void SellBazaar(CRegPacket cRegPacket)
        {
            if (ServerManager.Instance.InShutdown || Session.Character == null || Session.Character.InExchangeOrTrade)
            {
                return;
            }

            SpinWait.SpinUntil(() => !ServerManager.Instance.InBazaarRefreshMode);
            StaticBonusDTO medal =
                Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

            long price = cRegPacket.Price * cRegPacket.Amount;
            long taxmax = price > 100000 ? price / 200 : 500;
            long taxmin = price >= 4000 ? 60 + (price - 4000) / 2000 * 30 > 10000 ? 10000 : 60 + (price - 4000) / 2000 * 30 : 50;
            long tax = medal == null ? taxmax : taxmin;
            long maxGold = ServerManager.Instance.MaxGold;
            if (Session.Character.Gold < tax && Session.Account.Authority < AuthorityType.VipPlus || cRegPacket.Amount <= 0 ||
                Session.Character.ExchangeInfo != null && Session.Character.ExchangeInfo.ExchangeList.Any() || Session.Character.IsShopping)
            {
                return;
            }

            ItemInstance it = Session.Character.Inventory.LoadBySlotAndType(cRegPacket.Slot, cRegPacket.Inventory == 4 ? 0 : (InventoryType)cRegPacket.Inventory);
            if (it == null || !it.Item.IsSoldable || it.IsBound)
            {
                return;
            }

            if (Session.Character.Authority == AuthorityType.Vip && Session.Character.Inventory.CountItemInAnInventory(InventoryType.Bazaar) > 200)
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("LIMIT_EXCEEDED"), 0));
                return;
            }

            if (Session.Character.Authority == AuthorityType.VipPlus && Session.Character.Inventory.CountItemInAnInventory(InventoryType.Bazaar) > 300)
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("LIMIT_EXCEEDED"), 0));
                return;
            }

            if (Session.Character.Authority == AuthorityType.VipPlusPlus && Session.Character.Inventory.CountItemInAnInventory(InventoryType.Bazaar) > 500)
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("LIMIT_EXCEEDED"), 0));
                return;
            }

            if (Session.Character.Authority == AuthorityType.User && Session.Character.Inventory.CountItemInAnInventory(InventoryType.Bazaar) > 10 * (medal == null ? 1 : 10))
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("LIMIT_EXCEEDED"), 0));
                return;
            }

            if (price > (medal != null || Session.Character.Authority >= AuthorityType.Vip ? maxGold : 1000000 * it.Amount))
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("PRICE_EXCEEDED"), 0));
                return;
            }

            if (cRegPacket.Price < 0)
            {
                return;
            }

            ItemInstance bazar = Session.Character.Inventory.AddIntoBazaarInventory(cRegPacket.Inventory == 4 ? 0 : (InventoryType)cRegPacket.Inventory, cRegPacket.Slot, (ushort)cRegPacket.Amount);
            if (bazar == null)
            {
                return;
            }

            short duration;
            switch (cRegPacket.Durability)
            {
                case 1:
                    duration = 24;
                    break;

                case 2:
                    duration = 168;
                    break;

                case 3:
                    duration = 360;
                    break;

                case 4:
                    duration = 720;
                    break;

                default:
                    return;
            }

            DaoFactory.Instance.IteminstanceDao.InsertOrUpdate(bazar);

            var bazaarItem = new BazaarItemDTO
            {
                Amount = bazar.Amount,
                DateStart = DateTime.Now,
                Duration = duration,
                IsPackage = cRegPacket.IsPackage != 0,
                MedalUsed = medal != null,
                Price = cRegPacket.Price,
                SellerId = Session.Character.CharacterId,
                ItemInstanceId = bazar.Id
            };

            DaoFactory.Instance.BazaarItemDao.InsertOrUpdate(ref bazaarItem);
            if (bazar is WearableInstance wear)
            {
                wear.EquipmentOptions.ForEach(s => s.WearableInstanceId = bazar.Id);
                DaoFactory.Instance.EquipmentOptionDao.InsertOrUpdate(wear.EquipmentOptions);
            }

            ServerManager.Instance.BazaarRefresh(bazaarItem.BazaarItemId);

            if (Session.Account.Authority < AuthorityType.VipPlus)
            {
                Session.Character.Gold -= tax;
                Session.SendPacket(Session.Character.GenerateGold());
            }

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("OBJECT_IN_BAZAAR"), 10));
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("OBJECT_IN_BAZAAR"), 0));

            Session.SendPacket("rc_reg 1");
        }

        #endregion
    }
}