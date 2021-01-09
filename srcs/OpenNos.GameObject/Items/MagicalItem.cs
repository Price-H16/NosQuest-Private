﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Character;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Items
{
    public class MagicalItem : Item
    {
        #region Instantiation

        public MagicalItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0,
            string[] packetsplit = null)
        {
            switch (Effect)
            {
                // airwaves - eventitems
                case 0:
                    if (ItemType == ItemType.Magical)
                    {
                        switch (VNum)
                        {
                            // Bank Card
                            case 2539:
                            case 10066:
                                if (session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                                {
                                    return;
                                }

                                session.Character.OpenBank();
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                return;
                        }
                    }

                    if (ItemType == ItemType.Event)
                    {
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(EffectValue));
                        if (MappingHelper.Instance.GuriItemEffects.ContainsKey(EffectValue))
                        {
                            session.CurrentMapInstance?.Broadcast(
                                UserInterfaceHelper.Instance.GenerateGuri(19, 1, session.Character.CharacterId,
                                    MappingHelper.Instance.GuriItemEffects[EffectValue]), session.Character.MapX,
                                session.Character.MapY);
                        }

                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }

                    // APPLY SHELL ON EQUIPMENT
                    if (inv.Item.ItemType == ItemType.Shell)
                    {
                        if (((WearableInstance)inv).EquipmentOptions.Count == 0)
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                Language.Instance.GetMessageFromKey("SHELL_MUST_BE_IDENTIFIED"), 0));
                            return;
                        }

                        if (packetsplit == null)
                        {
                            return;
                        }

                        if (packetsplit.Length < 9)
                        {
                            // MODIFIED PACKET
                            return;
                        }

                        if (!short.TryParse(packetsplit[9], out short eqSlot) ||
                            !Enum.TryParse(packetsplit[8], out InventoryType eqType))
                        {
                            return;
                        }

                        if (!int.TryParse(packetsplit[6], out int requestType))
                        {
                            return;
                        }

                        var shell = (WearableInstance)inv;
                        var eq =
                            session.Character.Inventory.LoadBySlotAndType<WearableInstance>(eqSlot, eqType);

                        if (eq == null)
                        {
                            // PACKET MODIFIED
                            return;
                        }

                        if (eq.Item.ItemType != ItemType.Armor && shell.Item.ItemSubType == 1)
                        {
                            // ARMOR SHELL ONLY APPLY ON ARMORS
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("SHELL_FOR_ARMOR_ONLY"), 0));
                            return;
                        }

                        if (eq.Item.ItemType != ItemType.Weapon && shell.Item.ItemSubType == 0)
                        {
                            // WEAPON SHELL ONLY APPLY ON WEAPONS
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("SHELL_FOR_WEAPON_ONLY"), 0));
                            return;
                        }

                        switch (requestType)
                        {
                            case 0:
                                session.SendPacket(eq.EquipmentOptions.Count > 0
                                    ? $"qna #u_i^1^{session.Character.CharacterId}^{(short)inv.Type}^{inv.Slot}^1^1^{(short)eqType}^{eqSlot} {Language.Instance.GetMessageFromKey("ADD_OPTION_ON_STUFF_NOT_EMPTY")}"
                                    : $"qna #u_i^1^{session.Character.CharacterId}^{(short)inv.Type}^{inv.Slot}^1^1^{(short)eqType}^{eqSlot} {Language.Instance.GetMessageFromKey("ADD_OPTION_ON_STUFF")}");
                                break;
                            case 1:
                                if (shell.EquipmentOptions == null)
                                {
                                    // SHELL NOT IDENTIFIED
                                    return;
                                }

                                if (eq.BoundCharacterId != session.Character.CharacterId && eq.BoundCharacterId != null)
                                {
                                    // NEED TO PERFUME STUFF BEFORE CHANGING SHELL
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("NEED_PARFUM_TO_CHANGE_SHELL"), 0));
                                    return;
                                }

                                if (eq.Rare < shell.Rare)
                                {
                                    // RARITY TOO HIGH ON SHELL
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("SHELL_RARITY_TOO_HIGH"), 0));
                                    return;
                                }

                                if (eq.Item.LevelMinimum < shell.Upgrade)
                                {
                                    // SHELL LEVEL TOO HIGH
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("SHELL_LEVEL_TOO_HIGH"), 0));
                                    return;
                                }

                                if (eq.EquipmentOptions?.Any() == true)
                                {
                                    if (new Random().Next(100) >= 50)
                                    {
                                        // BREAK BECAUSE DIDN'T USE MAGIC ERASER
                                        session.SendPacket(
                                            UserInterfaceHelper.Instance.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("SHELL_BROKEN"), 0));
                                        session.Character.Inventory.RemoveItemAmountFromInventory(1, shell.Id);
                                        return;
                                    }
                                }

                                if (eq.EquipmentOptions == null)
                                {
                                    eq.EquipmentOptions = new List<EquipmentOptionDTO>();
                                }

                                eq.EquipmentOptions.Clear();
                                foreach (EquipmentOptionDTO i in shell.EquipmentOptions)
                                {
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateGuri(17, 1,
                                            session.Character.CharacterId));
                                    session.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateMsg(
                                            Language.Instance.GetMessageFromKey("SHELL_OPTION_SET"), 0));
                                    eq.EquipmentOptions.Add(i);
                                }

                                eq.BoundCharacterId = session.Character.CharacterId;
                                eq.ShellRarity = shell.Rare;
                                session.Character.Inventory.RemoveItemAmountFromInventory(1, shell.Id);
                                break;
                        }
                    }

                    break;

                //respawn objects
                case 1:
                    if (session.Character.MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance)
                    {
                        session.SendPacket(
                            session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_USE_THAT"), 10));
                        return;
                    }

                    if (packetsplit != null && int.TryParse(packetsplit[2], out int type) &&
                        int.TryParse(packetsplit[3], out int secondaryType) &&
                        int.TryParse(packetsplit[4], out int inventoryType) &&
                        int.TryParse(packetsplit[5], out int slot))
                    {
                        int packetType;
                        switch (EffectValue)
                        {
                            case 0:
                                if (option == 0)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                                        $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1 #u_i^{type}^{secondaryType}^{inventoryType}^{slot}^2 {Language.Instance.GetMessageFromKey("WANT_TO_SAVE_POSITION")}"));
                                }
                                else
                                {
                                    if (int.TryParse(packetsplit[6], out packetType))
                                    {
                                        switch (packetType)
                                        {
                                            case 1:
                                                session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7,
                                                    $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^3"));
                                                break;

                                            case 2:
                                                session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7,
                                                    $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^4"));
                                                break;

                                            case 3:
                                                session.Character.SetReturnPoint(session.Character.MapId,
                                                    session.Character.MapX, session.Character.MapY);
                                                RespawnMapTypeDTO respawn = session.Character.Respawn;
                                                if (respawn.DefaultX != 0 && respawn.DefaultY != 0 &&
                                                    respawn.DefaultMapId != 0)
                                                {
                                                    ServerManager.Instance.ChangeMap(session.Character.CharacterId,
                                                        respawn.DefaultMapId,
                                                        (short)(respawn.DefaultX +
                                                            ServerManager.Instance.RandomNumber(-5, 5)),
                                                        (short)(respawn.DefaultY +
                                                            ServerManager.Instance.RandomNumber(-5, 5)));
                                                }

                                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                                break;

                                            case 4:
                                                RespawnMapTypeDTO respawnObj = session.Character.Respawn;
                                                if (respawnObj.DefaultX != 0 && respawnObj.DefaultY != 0 &&
                                                    respawnObj.DefaultMapId != 0)
                                                {
                                                    ServerManager.Instance.ChangeMap(session.Character.CharacterId,
                                                        respawnObj.DefaultMapId,
                                                        (short)(respawnObj.DefaultX +
                                                            ServerManager.Instance.RandomNumber(-5, 5)),
                                                        (short)(respawnObj.DefaultY +
                                                            ServerManager.Instance.RandomNumber(-5, 5)));
                                                }

                                                session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                                break;
                                        }
                                    }
                                }

                                break;

                            case 1:
                                if (int.TryParse(packetsplit[6], out packetType))
                                {
                                    RespawnMapTypeDTO respawn = session.Character.Return;
                                    switch (packetType)
                                    {
                                        case 0:
                                            if (respawn.DefaultX != 0 && respawn.DefaultY != 0 &&
                                                respawn.DefaultMapId != 0)
                                            {
                                                session.SendPacket(UserInterfaceHelper.Instance.GenerateRp(
                                                    respawn.DefaultMapId, respawn.DefaultX, respawn.DefaultY,
                                                    $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                            }

                                            break;

                                        case 1:
                                            session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7,
                                                $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^2"));
                                            break;

                                        case 2:
                                            if (respawn.DefaultX != 0 && respawn.DefaultY != 0 &&
                                                respawn.DefaultMapId != 0)
                                            {
                                                ServerManager.Instance.ChangeMap(session.Character.CharacterId,
                                                    respawn.DefaultMapId, respawn.DefaultX, respawn.DefaultY);
                                            }

                                            session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                            break;
                                    }
                                }

                                break;

                            case 2:
                                if (option == 0)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7,
                                        $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                }
                                else
                                {
                                    ServerManager.Instance.JoinMiniland(session, session);
                                    session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                }

                                break;
                            case 4:
                                if (option == 0)
                                {
                                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDelay(5000, 7,
                                        $"#u_i^{type}^{secondaryType}^{inventoryType}^{slot}^1"));
                                }
                                else
                                {
                                    ServerManager.Instance.ChangeMap(session.Character.CharacterId, 98, 10, 30);
                                    session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                                }

                                break;
                        }
                    }

                    break;

                // dyes or waxes
                case 10:
                case 11:
                    if (!session.Character.IsVehicled)
                    {
                        if (Effect == 10)
                        {
                            if (EffectValue == 99)
                            {
                                byte nextValue = (byte)ServerManager.Instance.RandomNumber(0, 127);
                                session.Character.HairColor = Enum.IsDefined(typeof(HairColorType), nextValue)
                                    ? (HairColorType)nextValue
                                    : 0;
                            }
                            else
                            {
                                session.Character.HairColor = Enum.IsDefined(typeof(HairColorType), (byte)EffectValue)
                                    ? (HairColorType)EffectValue
                                    : 0;
                            }
                        }
                        else
                        {
                            if (session.Character.Class == (byte)ClassType.Adventurer && EffectValue > 1)
                            {
                                session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("ADVENTURERS_CANT_USE"), 10));
                                return;
                            }

                            session.Character.HairStyle = Enum.IsDefined(typeof(HairStyleType), (byte)EffectValue)
                                ? (HairStyleType)EffectValue
                                : 0;
                        }

                        session.SendPacket(session.Character.GenerateEq());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx());
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }

                    break;

                // dignity restoration
                case 14:
                    if ((EffectValue == 100 || EffectValue == 200) && session.Character.Dignity < 100 &&
                        !session.Character.IsVehicled)
                    {
                        session.Character.Dignity += EffectValue;
                        if (session.Character.Dignity > 100)
                        {
                            session.Character.Dignity = 100;
                        }

                        session.SendPacket(session.Character.GenerateFd());
                        session.SendPacket(session.Character.GenerateEff(49 - (byte)session.Character.Faction));
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(),
                            ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(),
                            ReceiverType.AllExceptMe);
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }
                    else if (EffectValue == 2000 && session.Character.Dignity < 100 && !session.Character.IsVehicled)
                    {
                        session.Character.Dignity = 100;
                        session.SendPacket(session.Character.GenerateFd());
                        session.SendPacket(session.Character.GenerateEff(49 - (byte)session.Character.Faction));
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(),
                            ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(),
                            ReceiverType.AllExceptMe);
                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    }

                    break;

                // speakers
                case 15:
                    if (!session.Character.IsVehicled)
                    {
                        if (option == 0)
                        {
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateGuri(10, 3, session.Character.CharacterId, 1));
                        }
                    }

                    break;

                // bubbles
                case 16:
                    if (!session.Character.IsVehicled)
                    {
                        if (option == 0)
                        {
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateGuri(10, 4, session.Character.CharacterId, 1));
                        }
                    }

                    break;

                // wigs
                case 30:
                    if (!session.Character.IsVehicled)
                    {
                        var wig =
                            session.Character.Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Hat,
                                InventoryType.Wear);
                        if (wig != null)
                        {
                            wig.Design = (byte)ServerManager.Instance.RandomNumber(0, 15);
                            session.SendPacket(session.Character.GenerateEq());
                            session.SendPacket(session.Character.GenerateEquipment());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn());
                            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx());
                            session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                        }
                        else
                        {
                            session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WIG"),
                                    0));
                        }
                    }

                    break;

                //Raid stone
                case 300:
                    if (session.Character.Group != null && session.Character.Group.GroupType != GroupType.Group &&
                        session.Character.Group.IsLeader(session) &&
                        session.CurrentMapInstance.Portals.Any(s => s.Type == (short)PortalType.Raid))
                    {
                        foreach (ClientSession sess in session.Character.Group.Characters)
                        {
                            ServerManager.Instance.ChangeMap(sess.Character.CharacterId, session.Character.MapId, session.Character.PositionX, session.Character.PositionY);
                        }

                        session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
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