﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Xml;
using OpenNos.Core;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject
{
    public class ScriptedInstance : ScriptedInstanceDTO
    {
        #region Members

        private readonly InstanceBag _instancebag = new InstanceBag();

        public Dictionary<int, MapInstance> MapInstanceDictionary = new Dictionary<int, MapInstance>();

        #endregion

        #region Properties

        public List<Gift> DrawItems { get; set; }

        public MapInstance FirstMap { get; set; }

        public List<Gift> GiftItems { get; set; }

        public List<MapInstance> Maps { get; set; }

        public int Fxp { get; set; }

        public long Gold { get; set; }

        public string Label { get; set; }

        public byte Id { get; set; }

        public byte LevelMaximum { get; set; }

        public byte LevelMinimum { get; set; }

        public byte Lives { get; set; }

        public int MonsterAmount { get; internal set; }

        public int NpcAmount { get; internal set; }

        public int Reputation { get; set; }

        public short StartX { get; set; }

        public short StartY { get; set; }

        public string Title { get; set; }

        public List<Gift> RequieredItems { get; set; }

        public int RoomAmount { get; internal set; }

        public List<Gift> SpecialItems { get; set; }

        #endregion

        #region Methods

        public void Dispose()
        {
            Thread.Sleep(10000);
            MapInstanceDictionary.Values.ToList().ForEach(m => m.Dispose());
        }

        public string GenerateMainInfo() => $"minfo 0 1 -1.0/0 -1.0/0 -1/0 -1.0/0 1 {FirstMap.InstanceBag.Lives + 1} 0";

        public List<string> GenerateMinimap()
        {
            List<string> lst = new List<string> { "rsfm 0 0 4 12" };
            MapInstanceDictionary.Values.ToList().ForEach(s => lst.Add(s.GenerateRsfn(true)));
            return lst;
        }

        public string GenerateRbr()
        {
            string drawgift = string.Empty;
            string requireditem = string.Empty;
            string bonusitems = string.Empty;
            string specialitems = string.Empty;

            for (int i = 0; i < 5; i++)
            {
                Gift gift = DrawItems.ElementAtOrDefault(i);
                drawgift += $" {(gift == null ? "-1.0" : $"{gift.VNum}.{gift.Amount}")}";
            }

            for (int i = 0; i < 2; i++)
            {
                Gift gift = SpecialItems.ElementAtOrDefault(i);
                specialitems += $" {(gift == null ? "-1.0" : $"{gift.VNum}.{gift.Amount}")}";
            }

            for (int i = 0; i < 3; i++)
            {
                Gift gift = GiftItems.ElementAtOrDefault(i);
                bonusitems += $"{(i == 0 ? "" : " ")}{(gift == null ? "-1.0" : $"{gift.VNum}.{gift.Amount}")}";
            }

            // TODO FINISH THIS
            const int winnerScore = 0;
            const string winner = "";
            return
                $"rbr 0.0.0 4 15 {LevelMinimum}.{LevelMaximum} {RequieredItems.Sum(s => s.Amount)} {drawgift} {specialitems} {bonusitems} {winnerScore}.{(winnerScore > 0 ? winner : "")} 0 0 {(Title != string.Empty ? Title : Language.Instance.GetMessageFromKey("TS_TUTORIAL"))}\n{Label}";
        }

        public string GenerateWp() => $"wp {PositionX} {PositionY} {ScriptedInstanceId} 0 {LevelMinimum} {LevelMaximum}";

        public void LoadGlobals()
        {
            RequieredItems = new List<Gift>();
            DrawItems = new List<Gift>();
            SpecialItems = new List<Gift>();
            GiftItems = new List<Gift>();
            Maps = new List<MapInstance>();

            var doc = new XmlDocument();
            if (Script != null)
            {
                doc.LoadXml(Script);

                XmlNode def = doc.SelectSingleNode("Definition").SelectSingleNode("Globals");
                LevelMinimum = byte.Parse(def.SelectSingleNode("LevelMinimum")?.Attributes["Value"].Value);
                LevelMaximum = byte.Parse(def.SelectSingleNode("LevelMaximum")?.Attributes["Value"].Value);
                Label = def.SelectSingleNode("Label")?.Attributes["Value"].Value;
                Title = def.SelectSingleNode("Title")?.Attributes["Value"].Value;
                byte.TryParse(def.SelectSingleNode("Id")?.Attributes["Value"].Value, out byte id);
                Id = id;
                long.TryParse(def.SelectSingleNode("Gold")?.Attributes["Value"].Value, out long gold);
                Gold = gold;
                int.TryParse(def.SelectSingleNode("Reputation")?.Attributes["Value"].Value, out int reputation);
                Reputation = reputation;

                int.TryParse(def.SelectSingleNode("Fxp")?.Attributes["Value"].Value, out int fxp);
                Fxp = fxp;

                short.TryParse(def.SelectSingleNode("StartX")?.Attributes["Value"].Value, out short startx);
                StartX = startx;

                short.TryParse(def.SelectSingleNode("StartY")?.Attributes["Value"].Value, out short starty);
                StartY = starty;

                byte.TryParse(def.SelectSingleNode("Lives")?.Attributes["Value"].Value, out byte lives);
                Lives = lives;
                if (def.SelectSingleNode("RequieredItems")?.ChildNodes != null)
                {
                    foreach (XmlNode node in def.SelectSingleNode("RequieredItems")?.ChildNodes)
                    {
                        RequieredItems.Add(new Gift(short.Parse(node.Attributes["VNum"].Value),
                            byte.Parse(node.Attributes["Amount"].Value)));
                    }
                }

                if (def.SelectSingleNode("DrawItems")?.ChildNodes != null)
                {
                    foreach (XmlNode node in def.SelectSingleNode("DrawItems")?.ChildNodes)
                    {
                        bool.TryParse(node.Attributes["IsRandomRare"]?.Value, out bool isRandomRare);
                        short.TryParse(node.Attributes["Design"]?.Value, out short design);
                        DrawItems.Add(new Gift(short.Parse(node.Attributes["VNum"].Value),
                            byte.Parse(node.Attributes["Amount"].Value), design, isRandomRare));
                    }
                }

                if (def.SelectSingleNode("SpecialItems")?.ChildNodes != null)
                {
                    foreach (XmlNode node in def.SelectSingleNode("SpecialItems")?.ChildNodes)
                    {
                        short.TryParse(node.Attributes["Design"]?.Value, out short design);
                        bool.TryParse(node.Attributes["IsRandomRare"]?.Value, out bool isRandomRare);
                        SpecialItems.Add(new Gift(short.Parse(node.Attributes["VNum"].Value),
                            byte.Parse(node.Attributes["Amount"].Value), design, isRandomRare));
                    }
                }

                if (def.SelectSingleNode("GiftItems")?.ChildNodes == null)
                {
                    return;
                }

                foreach (XmlNode node in def.SelectSingleNode("GiftItems")?.ChildNodes)
                {
                    bool.TryParse(node.Attributes["IsHeroic"]?.Value, out bool isHeroic);
                    bool.TryParse(node.Attributes["IsRandomRare"]?.Value, out bool isRandomRare);
                    short.TryParse(node.Attributes["Design"]?.Value, out short design);
                    GiftItems.Add(new Gift(short.Parse(node.Attributes["VNum"].Value),
                        byte.Parse(node.Attributes["Amount"].Value), design, isRandomRare, isHeroic));
                }
            }
        }

        public void LoadScript(MapInstanceType mapinstancetype)
        {
            var doc = new XmlDocument();
            if (Script == null)
            {
                return;
            }

            doc.LoadXml(Script);
            XmlNode instanceEvents = doc.SelectSingleNode("Definition");

            //CreateMaps
            XmlNodeList xmlNodeList = instanceEvents?.SelectSingleNode("InstanceEvents")?.ChildNodes;
            if (xmlNodeList != null)
            {
                foreach (XmlNode variable in xmlNodeList)
                {
                    if (variable.Name != "CreateMap")
                    {
                        continue;
                    }

                    _instancebag.Lives = Lives;
                    MapInstance newmap = ServerManager.Instance.GenerateMapInstance(
                        short.Parse(variable?.Attributes?["VNum"].Value), mapinstancetype, _instancebag);
                    Maps.Add(newmap);
                    byte.TryParse(variable?.Attributes["IndexX"]?.Value, out byte indexx);
                    newmap.MapIndexX = indexx;

                    byte.TryParse(variable?.Attributes["IndexY"]?.Value, out byte indexy);
                    newmap.MapIndexY = indexy;

                    if (!MapInstanceDictionary.ContainsKey(int.Parse(variable?.Attributes["Map"].Value)))
                    {
                        MapInstanceDictionary.Add(int.Parse(variable?.Attributes["Map"].Value), newmap);
                    }
                }
            }

            FirstMap = MapInstanceDictionary.Values.FirstOrDefault();
            Observable.Timer(TimeSpan.FromMinutes(3)).Subscribe(x =>
            {
                if (FirstMap.InstanceBag.Lock)
                {
                    return;
                }

                MapInstanceDictionary.Values.ToList().ForEach(m =>
                    EventHelper.Instance.RunEvent(new EventContainer(m, EventActionType.SCRIPTEND, (byte)1)));
                Dispose();
            });
            GenerateEvent(instanceEvents, FirstMap);
        }

        private ConcurrentBag<EventContainer> GenerateEvent(XmlNode node, MapInstance parentmapinstance)
        {
            ConcurrentBag<EventContainer> evts = new ConcurrentBag<EventContainer>();

            foreach (XmlNode mapevent in node.ChildNodes)
            {
                if (mapevent.Name == "#comment")
                {
                    continue;
                }

                short toY = -1;
                short toX = -1;
                Guid destmapInstanceId = default;
                if (!int.TryParse(mapevent.Attributes["Map"]?.Value, out int mapid))
                {
                    mapid = -1;
                }

                if (!short.TryParse(mapevent.Attributes["PositionX"]?.Value, out short positionX) ||
                    !short.TryParse(mapevent.Attributes["PositionY"]?.Value, out short positionY))
                {
                    positionX = -1;
                    positionY = -1;
                }

                if (int.TryParse(mapevent.Attributes["ToMap"]?.Value, out int toMap))
                {
                    MapInstance destmap = MapInstanceDictionary.First(s => s.Key == toMap).Value;
                    if (!short.TryParse(mapevent?.Attributes["ToY"]?.Value, out toY) ||
                        !short.TryParse(mapevent?.Attributes["ToX"]?.Value, out toX))
                    {
                        if (destmap != null)
                        {
                            MapCell cell2 = destmap.Map.GetRandomPosition();
                            toY = cell2.Y;
                            toX = cell2.X;
                            destmapInstanceId = destmap.MapInstanceId;
                        }
                        else
                        {
                            toY = -1;
                            toX = -1;
                        }
                    }
                    else
                    {
                        destmapInstanceId = destmap.MapInstanceId;
                    }
                }

                bool.TryParse(mapevent?.Attributes["IsTarget"]?.Value, out bool isTarget);
                bool.TryParse(mapevent?.Attributes["IsBonus"]?.Value, out bool isBonus);
                bool.TryParse(mapevent?.Attributes["IsBoss"]?.Value, out bool isBoss);
                bool.TryParse(mapevent?.Attributes["IsProtected"]?.Value, out bool isProtected);
                bool.TryParse(mapevent?.Attributes["IsMate"]?.Value, out bool isMate);
                if (!bool.TryParse(mapevent?.Attributes["Move"]?.Value, out bool move))
                {
                    move = true;
                }

                if (!bool.TryParse(mapevent?.Attributes["IsHostile"]?.Value, out bool isHostile))
                {
                    isHostile = true;
                }

                MapInstance mapinstance = MapInstanceDictionary.FirstOrDefault(s => s.Key == mapid).Value ??
                    parentmapinstance;
                MapCell cell;
                switch (mapevent.Name)
                {
                    //master events
                    case "CreateMap":
                    case "InstanceEvents":
                        foreach (EventContainer i in GenerateEvent(mapevent, mapinstance))
                        {
                            EventHelper.Instance.RunEvent(i);
                        }

                        break;

                    case "End":
                        MapInstanceDictionary.Values.ToList().ForEach(m => evts.Add(new EventContainer(m,
                            EventActionType.SCRIPTEND, byte.Parse(mapevent.Attributes?["Type"]?.Value ?? "0"))));
                        break;

                    //register events
                    case "OnCharacterDiscoveringMap":
                    case "OnMoveOnMap":
                    case "OnMapClean":
                    case "OnLockerOpen":
                        evts.Add(new EventContainer(mapinstance, EventActionType.REGISTEREVENT,
                            new Tuple<string, ConcurrentBag<EventContainer>>(mapevent.Name,
                                GenerateEvent(mapevent, mapinstance))));
                        break;

                    case "OnAreaEntry":
                        evts.Add(new EventContainer(mapinstance, EventActionType.SETAREAENTRY,
                            new ZoneEvent
                            {
                                X = positionX,
                                Y = positionY,
                                Range = byte.Parse(mapevent?.Attributes["Range"]?.Value),
                                Events = GenerateEvent(mapevent, mapinstance)
                            }));
                        break;

                    case "Wave":
                        byte.TryParse(mapevent?.Attributes["Offset"]?.Value, out byte offset);
                        evts.Add(new EventContainer(mapinstance, EventActionType.REGISTERWAVE,
                            new EventWave(byte.Parse(mapevent?.Attributes["Delay"]?.Value),
                                GenerateEvent(mapevent, mapinstance), offset)));
                        break;

                    case "SetMonsterLockers":
                        evts.Add(new EventContainer(mapinstance, EventActionType.SETMONSTERLOCKERS,
                            byte.Parse(mapevent?.Attributes["Value"]?.Value)));
                        break;

                    case "SetButtonLockers":
                        evts.Add(new EventContainer(mapinstance, EventActionType.SETBUTTONLOCKERS,
                            byte.Parse(mapevent?.Attributes["Value"]?.Value)));
                        break;

                    case "OnTimeElapsed":
                        evts.Add(new EventContainer(mapinstance, EventActionType.ONTIMEELAPSED,
                            new Tuple<int, ConcurrentBag<EventContainer>>(
                                int.Parse(mapevent?.Attributes["Value"]?.Value),
                                GenerateEvent(mapevent, mapinstance))));
                        break;

                    case "ControlMonsterInRange":
                        short.TryParse(mapevent?.Attributes["VNum"]?.Value, out short vnum);
                        evts.Add(new EventContainer(mapinstance, EventActionType.CONTROLEMONSTERINRANGE,
                            new Tuple<short, byte, ConcurrentBag<EventContainer>>(vnum,
                                byte.Parse(mapevent?.Attributes["Range"]?.Value),
                                GenerateEvent(mapevent, mapinstance))));
                        //Tuple<short, byte, List<EventContainer>>
                        break;
                    //child events
                    case "OnDeath":
                        GenerateEvent(mapevent, mapinstance).ToList().ForEach(s => evts.Add(s));
                        break;

                    case "OnTarget":
                        evts.Add(new EventContainer(mapinstance, EventActionType.ONTARGET,
                            GenerateEvent(mapevent, mapinstance)));
                        break;

                    case "Effect":
                        evts.Add(new EventContainer(mapinstance, EventActionType.EFFECT,
                            short.Parse(mapevent?.Attributes["Value"].Value)));
                        break;

                    case "SummonMonsters":
                        MonsterAmount += short.Parse(mapevent?.Attributes["Amount"].Value);
                        evts.Add(new EventContainer(mapinstance, EventActionType.SPAWNMONSTERS,
                            mapinstance.Map.GenerateSummons(short.Parse(mapevent?.Attributes["VNum"].Value),
                                short.Parse(mapevent?.Attributes["Amount"].Value), move,
                                new ConcurrentBag<EventContainer>(),
                                isBonus, isHostile, isBoss)));
                        break;

                    case "SummonMonster":
                        if (positionX == -1 || positionY == -1)
                        {
                            cell = mapinstance?.Map?.GetRandomPosition();
                            if (cell != null)
                            {
                                positionX = cell.X;
                                positionY = cell.Y;
                            }
                        }

                        MonsterAmount++;
                        ConcurrentBag<EventContainer> notice = new ConcurrentBag<EventContainer>();
                        ConcurrentBag<EventContainer> death = new ConcurrentBag<EventContainer>();
                        byte noticerange = 0;
                        foreach (XmlNode var in mapevent.ChildNodes)
                        {
                            switch (var.Name)
                            {
                                case "OnDeath":
                                    death = GenerateEvent(var, mapinstance);
                                    break;

                                case "OnNoticing":
                                    byte.TryParse(var?.Attributes["Range"]?.Value, out noticerange);
                                    notice = GenerateEvent(var, mapinstance);
                                    break;
                            }
                        }

                        ConcurrentBag<ToSummon> lst = new ConcurrentBag<ToSummon>
                        {
                            new ToSummon(short.Parse(mapevent?.Attributes["VNum"].Value),
                                new MapCell { X = positionX, Y = positionY }, null, move, 100, isTarget, isBonus,
                                isHostile, isBoss)
                            {
                                DeathEvents = death,
                                NoticingEvents = notice,
                                NoticeRange = noticerange
                            }
                        };
                        evts.Add(new EventContainer(mapinstance, EventActionType.SPAWNMONSTERS, lst.AsEnumerable()));
                        break;

                    case "SummonNps":
                        NpcAmount += short.Parse(mapevent?.Attributes["Amount"].Value);
                        ;
                        evts.Add(new EventContainer(mapinstance, EventActionType.SPAWNNPCS,
                            mapinstance.Map.GenerateSummons(short.Parse(mapevent?.Attributes["VNum"].Value),
                                short.Parse(mapevent?.Attributes["Amount"].Value), true,
                                new ConcurrentBag<EventContainer>(), isMate, isProtected)));
                        break;

                    case "RefreshRaidGoals":
                        evts.Add(new EventContainer(mapinstance, EventActionType.REFRESHRAIDGOAL, null));
                        break;

                    case "Move":
                        ConcurrentBag<EventContainer> moveevents = new ConcurrentBag<EventContainer>();
                        foreach (EventContainer eventContainer in GenerateEvent(mapevent, mapinstance))
                        {
                            moveevents.Add(eventContainer);
                        }

                        evts.Add(new EventContainer(mapinstance, EventActionType.MOVE,
                            new ZoneEvent { X = positionX, Y = positionY, Events = moveevents }));
                        break;

                    case "SummonNpc":
                        if (positionX == -1 || positionY == -1)
                        {
                            cell = mapinstance?.Map?.GetRandomPosition();
                            if (cell != null)
                            {
                                positionX = cell.X;
                                positionY = cell.Y;
                            }
                        }

                        NpcAmount++;
                        List<ToSummon> lstn = new List<ToSummon>
                        {
                            new ToSummon(short.Parse(mapevent?.Attributes["VNum"].Value),
                                new MapCell { X = positionX, Y = positionY }, null, move, 100, isBossOrMate: isMate,
                                isBonusOrProtected: isProtected)
                            {
                                DeathEvents = GenerateEvent(mapevent, mapinstance)
                            }
                        };
                        evts.Add(new EventContainer(mapinstance, EventActionType.SPAWNNPCS, lstn.AsEnumerable()));
                        break;

                    case "SpawnButton":
                        if (positionX == -1 || positionY == -1)
                        {
                            cell = mapinstance?.Map?.GetRandomPosition();
                            if (cell != null)
                            {
                                positionX = cell.X;
                                positionY = cell.Y;
                            }
                        }

                        var button = new MapButton(
                            int.Parse(mapevent?.Attributes["Id"].Value), positionX, positionY,
                            short.Parse(mapevent?.Attributes["VNumEnabled"].Value),
                            short.Parse(mapevent?.Attributes["VNumDisabled"].Value), new List<EventContainer>(),
                            new List<EventContainer>(), new List<EventContainer>());
                        foreach (XmlNode var in mapevent.ChildNodes)
                        {
                            switch (var.Name)
                            {
                                case "OnFirstEnable":
                                    button.FirstEnableEvents.AddRange(GenerateEvent(var, mapinstance));
                                    break;

                                case "OnEnable":
                                    button.EnableEvents.AddRange(GenerateEvent(var, mapinstance));
                                    break;

                                case "OnDisable":
                                    button.DisableEvents.AddRange(GenerateEvent(var, mapinstance));
                                    break;
                            }
                        }

                        evts.Add(new EventContainer(mapinstance, EventActionType.SPAWNBUTTON, button));
                        break;

                    case "StopClock":
                        evts.Add(new EventContainer(mapinstance, EventActionType.STOPCLOCK, null));
                        break;

                    case "StopMapClock":
                        evts.Add(new EventContainer(mapinstance, EventActionType.STOPMAPCLOCK, null));
                        break;

                    case "RefreshMapItems":
                        evts.Add(new EventContainer(mapinstance, EventActionType.REFRESHMAPITEMS, null));
                        break;

                    case "RemoveMonsterLocker":
                        evts.Add(new EventContainer(mapinstance, EventActionType.REMOVEMONSTERLOCKER, null));
                        break;

                    case "ThrowItem":
                        short.TryParse(mapevent?.Attributes["VNum"]?.Value, out short vnum2);
                        byte.TryParse(mapevent?.Attributes["PackAmount"]?.Value, out byte packAmount);
                        int.TryParse(mapevent?.Attributes["MinAmount"]?.Value, out int minAmount);
                        int.TryParse(mapevent?.Attributes["MaxAmount"]?.Value, out int maxAmount);
                        evts.Add(new EventContainer(mapinstance, EventActionType.THROWITEMS,
                            new Tuple<int, short, byte, int, int>(-1, vnum2, packAmount == 0 ? (byte)1 : packAmount,
                                minAmount == 0 ? 1 : minAmount, maxAmount == 0 ? 1 : maxAmount)));
                        break;

                    case "RemoveButtonLocker":
                        evts.Add(new EventContainer(mapinstance, EventActionType.REMOVEBUTTONLOCKER, null));
                        break;

                    case "ChangePortalType":
                        evts.Add(new EventContainer(mapinstance, EventActionType.CHANGEPORTALTYPE,
                            new Tuple<int, PortalType>(int.Parse(mapevent?.Attributes["IdOnMap"].Value),
                                (PortalType)sbyte.Parse(mapevent?.Attributes["Type"].Value))));
                        break;

                    case "SendPacket":
                        evts.Add(new EventContainer(mapinstance, EventActionType.SENDPACKET,
                            mapevent?.Attributes["Value"].Value));
                        break;

                    case "NpcDialog":
                        evts.Add(new EventContainer(mapinstance, EventActionType.NPCDIALOG,
                            int.Parse(mapevent?.Attributes["Value"].Value)));
                        break;

                    case "SendMessage":
                        evts.Add(new EventContainer(mapinstance, EventActionType.SENDPACKET,
                            UserInterfaceHelper.Instance.GenerateMsg(mapevent?.Attributes["Value"].Value,
                                byte.Parse(mapevent?.Attributes["Type"].Value))));
                        break;

                    case "GenerateClock":
                        evts.Add(new EventContainer(mapinstance, EventActionType.CLOCK,
                            int.Parse(mapevent?.Attributes["Value"].Value)));
                        break;

                    case "GenerateMapClock":
                        evts.Add(new EventContainer(mapinstance, EventActionType.MAPCLOCK,
                            int.Parse(mapevent?.Attributes["Value"].Value)));
                        break;

                    case "Teleport":
                        evts.Add(new EventContainer(mapinstance, EventActionType.TELEPORT,
                            new Tuple<short, short, short, short>(short.Parse(mapevent?.Attributes["PositionX"].Value),
                                short.Parse(mapevent?.Attributes["PositionY"].Value),
                                short.Parse(mapevent?.Attributes["DestinationX"].Value),
                                short.Parse(mapevent?.Attributes["DestinationY"].Value))));
                        break;

                    case "StartClock":
                        Tuple<ConcurrentBag<EventContainer>, ConcurrentBag<EventContainer>> eve =
                            new Tuple<ConcurrentBag<EventContainer>, ConcurrentBag<EventContainer>>(
                                new ConcurrentBag<EventContainer>(), new ConcurrentBag<EventContainer>());
                        foreach (XmlNode var in mapevent.ChildNodes)
                        {
                            switch (var.Name)
                            {
                                case "OnTimeout":
                                    foreach (EventContainer i in GenerateEvent(var, mapinstance))
                                    {
                                        eve.Item1.Add(i);
                                    }

                                    break;

                                case "OnStop":
                                    foreach (EventContainer i in GenerateEvent(var, mapinstance))
                                    {
                                        eve.Item2.Add(i);
                                    }

                                    break;
                            }
                        }

                        evts.Add(new EventContainer(mapinstance, EventActionType.STARTCLOCK, eve));
                        break;

                    case "StartMapClock":
                        eve = new Tuple<ConcurrentBag<EventContainer>, ConcurrentBag<EventContainer>>(
                            new ConcurrentBag<EventContainer>(), new ConcurrentBag<EventContainer>());
                        foreach (XmlNode var in mapevent.ChildNodes)
                        {
                            switch (var.Name)
                            {
                                case "OnTimeout":
                                    foreach (EventContainer i in GenerateEvent(var, mapinstance))
                                    {
                                        eve.Item1.Add(i);
                                    }

                                    break;

                                case "OnStop":
                                    foreach (EventContainer i in GenerateEvent(var, mapinstance))
                                    {
                                        eve.Item2.Add(i);
                                    }

                                    break;
                            }
                        }

                        evts.Add(new EventContainer(mapinstance, EventActionType.STARTMAPCLOCK, eve));
                        break;

                    case "SpawnPortal":
                        var portal = new Portal
                        {
                            PortalId = byte.Parse(mapevent?.Attributes["IdOnMap"].Value),
                            SourceX = positionX,
                            SourceY = positionY,
                            Type = short.Parse(mapevent?.Attributes["Type"].Value),
                            DestinationX = toX,
                            DestinationY = toY,
                            DestinationMapId = (short)(destmapInstanceId == default ? -1 : 0),
                            SourceMapInstanceId = mapinstance.MapInstanceId,
                            DestinationMapInstanceId = destmapInstanceId
                        };
                        foreach (XmlNode var in mapevent.ChildNodes)
                        {
                            if (var.Name == "OnTraversal")
                            {
                                portal.OnTraversalEvents.AddRange(GenerateEvent(var, mapinstance));
                            }
                        }

                        evts.Add(new EventContainer(mapinstance, EventActionType.SPAWNPORTAL, portal));
                        break;

                    case "ClearMapMonsters":
                        evts.Add(new EventContainer(mapinstance, EventActionType.CLEARMAPMONSTERS, null));
                        break;
                }
            }

            return evts;
        }

        public void End()
        {
            if (_instancebag.DeadList.Count > _instancebag.Lives)
            {
                foreach (MapInstance m in MapInstanceDictionary.Values)
                {
                    EventHelper.Instance.RunEvent(new EventContainer(m, EventActionType.SCRIPTEND, (byte)3));
                }

                Dispose();
            }

            if (_instancebag.Clock.DeciSecondRemaining > 0)
            {
                return;
            }

            foreach (MapInstance m in MapInstanceDictionary.Values)
            {
                EventHelper.Instance.RunEvent(new EventContainer(m, EventActionType.SCRIPTEND, (byte)1));
            }

            Dispose();
        }

        #endregion
    }
}