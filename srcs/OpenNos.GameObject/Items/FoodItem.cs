﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Threading;
using OpenNos.Core;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items
{
    public class FoodItem : Item
    {
        #region Instantiation

        public FoodItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public void Regenerate(ClientSession session, Item item, string[] packetsplit = null)
        {
            session.SendPacket(session.Character.GenerateEff(6000));
            session.Character.FoodAmount++;
            session.Character.MaxFood = 0;
            session.Character.FoodHp += item.Hp / 5;
            session.Character.FoodMp += item.Mp / 5;
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1800);
            }

            session.Character.FoodHp = item.Hp / 5;
            session.Character.FoodMp = item.Mp / 5;
            session.Character.FoodAmount--;
        }

        // TODO REVIEW THAT
        public void Sync(ClientSession session, Item item)
        {
            for (session.Character.MaxFood = 0; session.Character.MaxFood < 5; session.Character.MaxFood++)
            {
                if (session.Character.Hp <= 0 || !session.Character.IsSitting)
                {
                    session.Character.FoodAmount = 0;
                    session.Character.FoodHp = 0;
                    session.Character.FoodMp = 0;
                    return;
                }

                session.Character.Hp += session.Character.FoodHp;
                session.Character.Mp += session.Character.FoodMp;
                if (session.Character.FoodHp > 0 && session.Character.FoodHp > 0 &&
                    (session.Character.Hp < session.Character.HpLoad() ||
                        session.Character.Mp < session.Character.MpLoad()))
                {
                    session.CurrentMapInstance?.Broadcast(session,
                        session.Character.GenerateRc(session.Character.FoodHp));
                }

                if (session.IsConnected)
                {
                    session.SendPacket(session.Character.GenerateStat());
                }
                else
                {
                    return;
                }

                Thread.Sleep(1800);
            }
        }

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null)
        {
            if ((DateTime.Now - session.Character.LastPotion).TotalMilliseconds < 750)
            {
                return;
            }

            session.Character.LastPotion = DateTime.Now;
            Item item = inv.Item;
            switch (Effect)
            {
                default:
                    if (session.Character.Hp <= 0)
                    {
                        return;
                    }

                    if (!session.Character.IsSitting)
                    {
                        session.Character.Rest();
                    }

                    int amount = session.Character.FoodAmount;
                    if (amount < 5)
                    {
                        if (!session.Character.IsSitting)
                        {
                            return;
                        }

                        var workerThread = new Thread(() => Regenerate(session, item));
                        workerThread.Start();
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }
                    else
                    {
                        session.SendPacket(session.Character.Gender == GenderType.Female
                            ? session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_HUNGRY_FEMALE"), 1)
                            : session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_HUNGRY_MALE"), 1));
                    }

                    if (amount == 0)
                    {
                        if (!session.Character.IsSitting)
                        {
                            return;
                        }

                        var workerThread2 = new Thread(() => Sync(session, item));
                        workerThread2.Start();
                    }

                    break;
            }
        }

        #endregion
    }
}