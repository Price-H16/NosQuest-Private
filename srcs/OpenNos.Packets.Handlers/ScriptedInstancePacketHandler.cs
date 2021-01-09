﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CloneExtensions;
using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.GameObject;
using OpenNos.GameObject.Event.TalentArena;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Packets.Enums;
using WingsEmu.Packets.ServerPackets;

namespace WingsEmu.PacketHandlers
{
    internal class ScriptedInstancePacketHandler : IPacketHandler
    {
        #region Instantiation

        public ScriptedInstancePacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public void ShPacket(ShPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            switch (packet.TargetType)
            {
                case UserType.Player:
                    ClientSession target = ServerManager.Instance.GetSessionBySessionId(packet.TargetId);

                    if (target == null || !target.Character.CanAttack || target == Session)
                    {
                        return;
                    }

                    Session.Character.GenerateSheepScore(packet.TargetType);
                    target.Character.Speed = 0;
                    target.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("RESURRECT_IN_SECONDS"), 10)));
                    target.Character.CanAttack = false;
                    Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(s =>
                    {
                        if (target != null || target != Session) // Possible Crash , bcs u have a Timer <- , Need to check if is useless or not 
                        {
                            target.Character.SheepScore1 -= 10; // Need to verify on Official Nostale If you Lost Only 10 Pts
                            target.Character.CanAttack = true;
                            target.Character.Speed = 5;
                            ServerManager.Instance.TeleportOnRandomPlaceInMap(target, target.CurrentMapInstance.MapInstanceId);
                        }
                    });
                    break;
                case UserType.Monster:
                    Session.Character.GenerateSheepScore(packet.TargetType);
                    Session.CurrentMapInstance?.Broadcast(StaticPacketHelper.Out(UserType.Monster, packet.TargetId));
                    break;
            }
        }

        public void ButtonCancel(BscPacket packet)
        {
            switch (packet.Type)
            {
                case 2:
                    ArenaMember arenamember = ServerManager.Instance.ArenaMembers.FirstOrDefault(s => s.Session == Session);
                    if (arenamember?.GroupId != null)
                    {
                        if (packet.Option != 1)
                        {
                            Session.SendPacket($"qna #bsc^2^1 {Language.Instance.GetMessageFromKey("ARENA_PENALTY_NOTICE")}");
                            return;
                        }
                    }

                    Session.Character.LeaveTalentArena(true);
                    break;
            }
        }

        public void SearchName(TawPacket packet)
        {
            ConcurrentBag<ArenaTeamMember> at = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(o => o.Session.Character.Name == packet.Username));
            if (at != null)
            {
                ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, at.FirstOrDefault(s => s.Session != null).Session.CurrentMapInstance.MapInstanceId, 69, 100);

                ArenaTeamMember zenas = at.OrderBy(s => s.Order).FirstOrDefault(s => s.Session != null && !s.Dead && s.ArenaTeamType == ArenaTeamType.ZENAS);
                ArenaTeamMember erenia = at.OrderBy(s => s.Order).FirstOrDefault(s => s.Session != null && !s.Dead && s.ArenaTeamType == ArenaTeamType.ERENIA);
                Session.SendPacket(Session.Character.GenerateTaM(0));
                Session.SendPacket(Session.Character.GenerateTaM(3));
                Session.SendPacket("taw_sv 0");
                Session.SendPacket(zenas?.Session.Character.GenerateTaP(0, true));
                Session.SendPacket(erenia?.Session.Character.GenerateTaP(2, true));
                Session.SendPacket(zenas?.Session.Character.GenerateTaFc(0));
                Session.SendPacket(erenia?.Session.Character.GenerateTaFc(1));
            }
            else
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("USER_NOT_FOUND_IN_ARENA")));
            }
        }

        public void Call(TaCallPacket packet)
        {
            ConcurrentBag<ArenaTeamMember> arenateam = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(o => o.Session == Session));
            if (arenateam == null || Session.CurrentMapInstance.MapInstanceType != MapInstanceType.TalentArenaMapInstance)
            {
                return;
            }

            IEnumerable<ArenaTeamMember> ownteam = arenateam.Where(s => s.ArenaTeamType == arenateam?.FirstOrDefault(e => e.Session == Session)?.ArenaTeamType);
            ClientSession client = ownteam.Where(s => s.Session != Session).OrderBy(s => s.Order).Skip(packet.CalledIndex).FirstOrDefault()?.Session;
            ArenaTeamMember memb = arenateam.FirstOrDefault(s => s.Session == client);
            if (client == null || client.CurrentMapInstance != Session.CurrentMapInstance || memb == null || memb.LastSummoned != null || ownteam.Sum(s => s.SummonCount) >= 5)
            {
                return;
            }

            memb.SummonCount++;
            arenateam.ToList().ForEach(arenauser => { arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaP(2, true)); });
            ArenaTeamMember arenaTeamMember = arenateam.FirstOrDefault(s => s.Session == client);
            if (arenaTeamMember != null)
            {
                arenaTeamMember.LastSummoned = DateTime.Now;
            }

            Session.CurrentMapInstance.Broadcast(Session.Character.GenerateEff(4432));
            for (int i = 0; i < 3; i++)
            {
                Observable.Timer(TimeSpan.FromSeconds(i)).Subscribe(o =>
                {
                    client.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("ARENA_CALLED"), 3 - i), 0));
                    client.SendPacket(client.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("ARENA_CALLED"), 3 - i), 10));
                });
            }

            short x = Session.Character.PositionX;
            short y = Session.Character.PositionY;
            const byte timer = 30;
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(o =>
            {
                Session.CurrentMapInstance.Broadcast($"ta_t 0 {client.Character.CharacterId} {timer}");
                client.Character.PositionX = x;
                client.Character.PositionY = y;
                Session.CurrentMapInstance.Broadcast(client.Character.GenerateTp());

                client.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Nothing));
            });

            Observable.Timer(TimeSpan.FromSeconds(timer + 3)).Subscribe(o =>
            {
                DateTime? lastsummoned = arenateam.FirstOrDefault(s => s.Session == client)?.LastSummoned;
                if (lastsummoned == null || ((DateTime)lastsummoned).AddSeconds(timer) >= DateTime.Now)
                {
                    return;
                }

                ArenaTeamMember firstOrDefault = arenateam.FirstOrDefault(s => s.Session == client);
                if (firstOrDefault != null)
                {
                    firstOrDefault.LastSummoned = null;
                }

                client.Character.PositionX = memb.ArenaTeamType == ArenaTeamType.ERENIA ? (short)120 : (short)19;
                client.Character.PositionY = memb.ArenaTeamType == ArenaTeamType.ERENIA ? (short)39 : (short)40;
                Session?.CurrentMapInstance.Broadcast(client.Character.GenerateTp());
                client.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));
            });
        }

        /// <summary>
        ///     RSelPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void Escape(EscapePacket packet)
        {
            switch (Session.CurrentMapInstance.MapInstanceType)
            {
                case MapInstanceType.TimeSpaceInstance:
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                    break;
                case MapInstanceType.TalentArenaMapInstance:
                    ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, ServerManager.Instance.ArenaInstance.MapInstanceId);
                    break;
                case MapInstanceType.RaidInstance:
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                    Session.Character.Group?.Characters.ToList().ForEach(
                        session => { session.SendPacket(session.Character.Group.GenerateRdlst()); });
                    Session.SendPacket(Session.Character.GenerateRaid(1, true));
                    Session.SendPacket(Session.Character.GenerateRaid(2, true));
                    Session.Character.Group?.LeaveGroup(Session);
                    break;
                case MapInstanceType.SheepGameInstance:
                    int miniscore = 50; // your score
                    if (Session.Character.SheepScore1 > miniscore && Session.Character.IsWaitingForGift) // Anti Afk to get Reward
                    {
                        ushort[] random1 = { 1, 2, 3 };
                        ushort[] random = { 2, 4, 6 };
                        short[] acorn = { 5947, 5948, 5949, 5950 };
                        Session.Character.GiftAdd(5951, random1[ServerManager.Instance.RandomNumber(0, random1.Length)]);
                        int rnd = ServerManager.Instance.RandomNumber(0, 5);
                        switch (rnd)
                        {
                            case 2:
                                Session.Character.GiftAdd(acorn[ServerManager.Instance.RandomNumber(0, acorn.Length)], random[ServerManager.Instance.RandomNumber(0, random.Length)]);
                                break;
                        }

                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                        Session.Character.IsWaitingForGift = false;
                    }

                    break;
            }
        }

        /// <summary>
        ///     RSelPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void GetGift(RSelPacket packet)
        {
            if (Session.CurrentMapInstance.MapInstanceType != MapInstanceType.TimeSpaceInstance)
            {
                return;
            }

            Guid mapInstanceId = ServerManager.Instance.GetBaseMapInstanceIdByMapId(Session.Character.MapId);
            MapInstance map = ServerManager.Instance.GetMapInstance(mapInstanceId);
            ScriptedInstance si = map.ScriptedInstances.FirstOrDefault(s => s.PositionX == Session.Character.MapX && s.PositionY == Session.Character.MapY);
            if (si == null)
            {
                return;
            }

            Session.Character.GetReput(si.Reputation, true);

            Session.Character.Gold = Session.Character.Gold + si.Gold > ServerManager.Instance.MaxGold ? ServerManager.Instance.MaxGold : Session.Character.Gold + si.Gold;
            Session.SendPacket(Session.Character.GenerateGold());
            Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("GOLD_TS_END"), si.Gold), 10));

            int rand = new Random().Next(si.DrawItems.Count);
            string repay = "repay ";
            Session.Character.GiftAdd(si.DrawItems[rand].VNum, si.DrawItems[rand].Amount);

            for (int i = 0; i < 3; i++)
            {
                Gift gift = si.GiftItems.ElementAtOrDefault(i);
                repay += gift == null ? "-1.0.0 " : $"{gift.VNum}.0.{gift.Amount} ";
                if (gift != null)
                {
                    Session.Character.GiftAdd(gift.VNum, gift.Amount);
                }
            }

            // TODO: Add HasAlreadyDone
            for (int i = 0; i < 2; i++)
            {
                Gift gift = si.SpecialItems.ElementAtOrDefault(i);
                repay += gift == null ? "-1.0.0 " : $"{gift.VNum}.0.{gift.Amount} ";
                if (gift != null)
                {
                    Session.Character.GiftAdd(gift.VNum, gift.Amount);
                }
            }

            repay += $"{si.DrawItems[rand].VNum}.0.{si.DrawItems[rand].Amount}";
            Session.SendPacket(repay);
            map.InstanceBag.EndState = 6;
        }

        /// <summary>
        ///     treq packet
        /// </summary>
        /// <param name="treqPacket"></param>
        public void GetTreq(TreqPacket treqPacket)
        {
            ScriptedInstance timespace = Session.CurrentMapInstance.ScriptedInstances.FirstOrDefault(s => treqPacket.X == s.PositionX && treqPacket.Y == s.PositionY).GetClone();

            if (timespace == null)
            {
                return;
            }

            if (treqPacket.StartPress == 1 || treqPacket.RecordPress == 1)
            {
                timespace.LoadScript(MapInstanceType.TimeSpaceInstance);
                if (timespace.FirstMap == null)
                {
                    return;
                }

                foreach (Gift i in timespace.RequieredItems)
                {
                    if (Session.Character.Inventory.CountItem(i.VNum) < i.Amount)
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_REQUIERED_ITEM"), ServerManager.Instance.GetItem(i.VNum).Name), 0));
                        return;
                    }

                    Session.Character.Inventory.RemoveItemAmount(i.VNum, i.Amount);
                }

                if (timespace.LevelMinimum > Session.Character.Level)
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_REQUIERED_LEVEL"), 0));
                    return;
                }

                Session.Character.MapX = timespace.PositionX;
                Session.Character.MapY = timespace.PositionY;
                ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, timespace.FirstMap.MapInstanceId);
                timespace.FirstMap.InstanceBag.Creator = Session.Character.CharacterId;
                Session.SendPackets(timespace.GenerateMinimap());
                Session.SendPacket(timespace.GenerateMainInfo());
                Session.SendPacket(timespace.FirstMap.InstanceBag.GenerateScore());
            }
            else
            {
                Session.SendPacket(timespace.GenerateRbr());
            }
        }

        /// <summary>
        ///     mkraid packet
        /// </summary>
        /// <param name="packet"></param>
        public void GenerateRaid(MkraidPacket packet)
        {
            if (Session.Character.Group?.Raid == null || !Session.Character.Group.IsLeader(Session))
            {
                return;
            }

            if (Session.Character.MapId != Session.Character.Group?.Raid.MapId && !ServerManager.Instance.RaidPortalFromAnywhere)
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg("WRONG_PORTAL", 0));
                return;
            }

            if (Session.Character.Group.CharacterCount <= 4 && Session.Character.Authority < AuthorityType.GameMaster)
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg("RAID_TEAM_NOT_READY", 0));
                return;
            }

            if (Session.Character.Group.Raid.FirstMap == null)
            {
                Session.Character.Group.Raid.LoadScript(MapInstanceType.RaidInstance);
            }

            if (Session.Character.Group.Raid.FirstMap == null)
            {
                // no script is present, user should be warned about implementation laks
                return;
            }

            Session.Character.Group.Raid.FirstMap.InstanceBag.Lock = true;
            if (ServerManager.Instance.GroupList.Any(s => s.GroupId == Session.Character.Group.GroupId))
            {
                ServerManager.Instance.GroupList.Remove(Session.Character.Group);
            }

            if (Session?.Character?.Group?.Characters == null)
            {
                return;
            }

            Session?.Character?.Group?.Characters.Where(s => s.CurrentMapInstance?.MapInstanceId != Session.CurrentMapInstance?.MapInstanceId).ToList().ForEach(session =>
            {
                Session.Character.Group.LeaveGroup(session);
                session.SendPacket(session.Character.GenerateRaid(1, true));
                session.SendPacket(session.Character.GenerateRaid(2, true));
            });

            Session.Character.Group.Raid.FirstMap.InstanceBag.Lives = (short)Session.Character.Group.CharacterCount;
            Session.Character.Group.Characters.ToList().ForEach(session =>
            {
                ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId, session.Character.Group.Raid.FirstMap.MapInstanceId, session.Character.Group.Raid.StartX,
                    session.Character.Group.Raid.StartY);
                session.SendPacket("raidbf 0 0 25");
                session.SendPacket(session.Character.Group.GeneraterRaidmbf(session.CurrentMapInstance));
                session.SendPacket(session.Character.GenerateRaid(5, false));
                session.SendPacket(session.Character.GenerateRaid(4, false));
                session.SendPacket(session.Character.GenerateRaid(3, false));
            });
        }

        /// <summary>
        ///     wreq packet
        /// </summary>
        /// <param name="packet"></param>
        public void GetWreq(WreqPacket packet)
        {
            foreach (ScriptedInstance portal in Session.CurrentMapInstance.ScriptedInstances)
            {
                if (Session.Character.PositionY < portal.PositionY - 1 || Session.Character.PositionY > portal.PositionY + 1 || Session.Character.PositionX < portal.PositionX - 1 ||
                    Session.Character.PositionX > portal.PositionX + 1)
                {
                    continue;
                }

                switch (packet.Value)
                {
                    case 0:
                        if (Session.Character.Group != null && Session.Character.Group.Characters.Any(s =>
                            !s.CurrentMapInstance.InstanceBag.Lock && s.CurrentMapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance && s.Character.MapId == portal.MapId &&
                            s.Character.CharacterId != Session.Character.CharacterId && s.Character.MapX == portal.PositionX && s.Character.MapY == portal.PositionY))
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                                $"#wreq^3^{Session.Character.CharacterId} #wreq^0^1 {Language.Instance.GetMessageFromKey("ASK_JOIN_TEAM_TS")}"));
                        }
                        else
                        {
                            Session.SendPacket(portal.GenerateRbr());
                        }

                        break;

                    case 1:
                        byte.TryParse(packet.Param.ToString(), out byte record);
                        GetTreq(new TreqPacket
                        {
                            X = portal.PositionX,
                            Y = portal.PositionY,
                            RecordPress = 1,
                            StartPress = 1
                        });
                        break;

                    case 3:
                        ClientSession character = (Session.Character.Group?.Characters).FirstOrDefault(s => s.Character.CharacterId == packet.Param);
                        if (character != null)
                        {
                            if (portal.LevelMinimum > Session.Character.Level)
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_REQUIERED_LEVEL"), 0));
                                return;
                            }

                            Session.Character.MapX = portal.PositionX;
                            Session.Character.MapY = portal.PositionY;
                            ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, character.CurrentMapInstance.MapInstanceId);
                            Session.SendPacket(portal.GenerateMainInfo());
                            Session.SendPackets(portal.GenerateMinimap());
                            Session.SendPacket(portal.FirstMap.InstanceBag.GenerateScore());
                        }

                        break;
                }
            }
        }

        /// <summary>
        ///     GitPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void Git(GitPacket packet)
        {
            MapButton button = Session.CurrentMapInstance.Buttons.FirstOrDefault(s => s.MapButtonId == packet.ButtonId);
            if (button == null)
            {
                return;
            }

            Session.CurrentMapInstance.Broadcast(button.GenerateOut());
            button.RunAction();
            Session.CurrentMapInstance.Broadcast(button.GenerateIn());
        }

        /// <summary>
        ///     rxitPacket packet
        /// </summary>
        /// <param name="rxitPacket"></param>
        public void InstanceExit(RxitPacket rxitPacket)
        {
            if (rxitPacket?.State != 1)
            {
                return;
            }

            if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
            {
                ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, ServerManager.Instance.ArenaInstance.MapInstanceId);
            }
            else if (Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.TimeSpaceInstance)
            {
                if (Session.CurrentMapInstance.InstanceBag.Lock)
                {
                    //5seed
                    Session.CurrentMapInstance.InstanceBag.DeadList.Add(Session.Character.CharacterId);
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("DIGNITY_LOST"), 20), 11));
                    Session.Character.Dignity = Session.Character.Dignity < -980 ? -1000 : Session.Character.Dignity - 20;
                }

                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
            }
            else if (Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance && Session.Character.Group?.Characters != null)
            {
                foreach (ClientSession s in Session.Character.Group.Characters)
                {
                    s.SendPacket(Session.Character.Group.GenerateRdlst());
                }

                Session.SendPacket(Session.Character.Group.GenerateRdlst());
                Session.Character.Group?.LeaveGroup(Session);
                ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                Session.SendPacket(Session.Character.GenerateRaid(1, true));
                Session.SendPacket(Session.Character.GenerateRaid(2, true));
            }
        }

        #endregion
    }
}