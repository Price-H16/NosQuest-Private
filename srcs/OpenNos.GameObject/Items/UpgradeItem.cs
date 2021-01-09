﻿// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items
{
    public class UpgradeItem : Item
    {
        public UpgradeItem(ItemDTO item) : base(item)
        {
        }

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null)
        {
            if (Effect == 0)
            {
                if (EffectValue != 0)
                {
                    if (session.Character.IsSitting)
                    {
                        session.Character.IsSitting = false;
                        session.SendPacket(session.Character.GenerateRest());
                    }

                    session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(12, 1, session.Character.CharacterId,
                        EffectValue));
                }
                else if (EffectValue == 0)
                {
                    if (packetsplit == null || packetsplit.Length <= 9)
                    {
                        return;
                    }

                    if (!byte.TryParse(packetsplit[8], out byte typeEquip) ||
                        !short.TryParse(packetsplit[9], out short slotEquip))
                    {
                        return;
                    }

                    if (session.Character.IsSitting)
                    {
                        session.Character.IsSitting = false;
                        session.SendPacket(session.Character.GenerateRest());
                    }

                    if (option != 0)
                    {
                        bool isUsed = false;
                        switch (inv.ItemVNum)
                        {
                            case 1219:
                                var equip =
                                    session.Character.Inventory.LoadBySlotAndType<WearableInstance>(slotEquip,
                                        (InventoryType)typeEquip);
                                if (equip != null && equip.IsFixed)
                                {
                                    equip.IsFixed = false;
                                    session.SendPacket(session.Character.GenerateEff(3003));
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(17, 1,
                                        session.Character.CharacterId, slotEquip));
                                    session.SendPacket(
                                        session.Character.GenerateSay(
                                            Language.Instance.GetMessageFromKey("ITEM_UNFIXED"), 12));
                                    isUsed = true;
                                }

                                break;

                            case 1365:
                            case 9039:
                                var specialist =
                                    session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>(slotEquip,
                                        (InventoryType)typeEquip);
                                if (specialist != null && specialist.Rare == -2)
                                {
                                    specialist.Rare = 0;
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("SP_RESURRECTED"), 0));
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateGuri(13, 1, session.Character.CharacterId,
                                            1));
                                    session.Character.SpPoint = 10000;
                                    if (session.Character.SpPoint > 10000)
                                    {
                                        session.Character.SpPoint = 10000;
                                    }

                                    session.SendPacket(session.Character.GenerateSpPoint());
                                    session.SendPacket(specialist.GenerateInventoryAdd());
                                    isUsed = true;
                                }

                                break;
                        }

                        if (!isUsed)
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ITEM_IS_NOT_FIXED"),
                                    11));
                        }
                        else
                        {
                            session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                        }
                    }
                    else
                    {
                        session.SendPacket(
                            $"qna #u_i^1^{session.Character.CharacterId}^{(byte)inv.Type}^{inv.Slot}^0^1^{typeEquip}^{slotEquip} {Language.Instance.GetMessageFromKey("QNA_ITEM")}");
                    }
                }
            }
            else
            {
                Logger.Log.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType()));
            }
        }
    }

    #endregion
}