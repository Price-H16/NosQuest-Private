// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using OpenNos.GameObject.Bazaar;
using OpenNos.GameObject.Buff;
using OpenNos.GameObject.Character;
using OpenNos.GameObject.Configuration;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Event.ACT6;
using OpenNos.GameObject.Event.BattleRoyale;
using OpenNos.GameObject.Event.ICEBREAKER;
using OpenNos.GameObject.Event.TalentArena;
using OpenNos.GameObject.Families;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Npc;
using OpenNos.GameObject.Quests;
using OpenNos.GameObject.Shops;
using OpenNos.GameObject.Skills;
using WingsEmu.Communication;
using WingsEmu.Configuration;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Character;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Networking
{
    public class ServerManager : BroadcastableBase
    {
        #region Instantiation

        private ServerManager()
        {
            // do nothing
        }

        #endregion

        #region Members

        public GameRateConfiguration RateConfiguration;
        public GameMinMaxConfiguration GameMinMaxConfiguration;
        public GameTrueFalseConfiguration GameTrueFalseConfiguration;
        public GameScheduledEventsConfiguration GameScheduledEventsConfiguration;

        private static readonly List<Item> Items = new List<Item>();

        private static readonly ConcurrentDictionary<Guid, MapInstance> _mapInstances =
            new ConcurrentDictionary<Guid, MapInstance>();

        private static readonly List<Map> Maps = new List<Map>();

        private static readonly List<NpcMonster> Npcs = new List<NpcMonster>();

        private static readonly List<Skill> Skills = new List<Skill>();

        private static readonly ThreadLocal<Random> Random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        private static ServerManager _instance;

        private static int _seed = Environment.TickCount;

        private bool _disposed;

        private List<DropDTO> _generalDrops;

        private long _lastGroupId;

        public ConcurrentDictionary<long, Group> _groups;

        private Dictionary<short, List<MapNpc>> _mapNpcs;

        private Dictionary<short, List<DropDTO>> _monsterDrops;

        private Dictionary<short, List<NpcMonsterSkill>> _monsterSkills;

        private Dictionary<int, List<Recipe>> _recipes;

        private Dictionary<int, List<ShopItemDTO>> _shopItems;

        private Dictionary<int, Shop> _shops;

        private Dictionary<int, List<ShopSkillDTO>> _shopSkills;

        private Dictionary<int, List<TeleporterDTO>> _teleporters;

        private List<Recipe> _recipeLists;


        private bool _inRelationRefreshMode;

        #endregion

        #region Properties

        public static ServerManager Instance => _instance ?? (_instance = new ServerManager());

        public byte MaxCodeAttempts { get; set; }

        public TimeSpan TimeBeforeAutoKick { get; set; }

        public TimeSpan AutoKickInterval { get; set; }

        public bool AntiBotEnabled { get; set; }

        public int MaximumHomes { get; set; }

        public bool Easter { get; set; }

        public bool Winter { get; set; }

        public bool Estival { get; set; }

        public bool Halloween { get; set; }

        public bool Valentine { get; set; }

        public IEnumerable<CharacterHomeDTO> CharacterHomes { get; set; }

        public ConcurrentBag<ScriptedInstance> Act4Raids { get; set; }

        public ConcurrentBag<ScriptedInstance> Act6Raids { get; set; }

        public MapInstance ArenaInstance { get; private set; }

        public List<BazaarItemLink> BazaarList { get; set; }

        public List<ConcurrentBag<ArenaTeamMember>> ArenaTeams { get; set; } =
            new List<ConcurrentBag<ArenaTeamMember>>();

        public int ChannelId { get; set; }

        public List<CharacterRelationDTO> CharacterRelations { get; set; }

        public int DropRate { get; set; }

        public int FamilyExpRate { get; set; }

        public bool ReputOnMonsters { get; set; }

        public bool RaidPortalFromAnywhere { get; set; }

        public bool LodTimes { get; set; }

        public bool AutoLoot { get; set; }

        public short MinLodLevel { get; set; }
        public int CylloanPercentRate { get; set; }

        public int GlacernonPercentRatePvp { get; set; }

        public int GlacernonPercentRatePvm { get; set; }

        public int QuestDropRate { get; set; }

        public bool EventInWaiting { get; set; }

        public int FairyXpRate { get; set; }

        public MapInstance FamilyArenaInstance { get; private set; }

        public MapInstance CaligorMapInstance { get; set; }

        public ConcurrentDictionary<long, Family> FamilyList { get; set; }

        public int GoldDropRate { get; set; }

        public int GoldRate { get; set; }

        public int ReputRate { get; set; }

        public List<Group> Groups
        {
            get { return _groups.Select(s => s.Value).ToList(); }
        }

        public int HeroicStartLevel { get; set; }

        public int HeroXpRate { get; set; }

        public bool IceBreakerInWaiting { get; set; }

        public bool InBazaarRefreshMode { get; set; }

        public bool InFamilyRefreshMode { get; set; }

        public List<int> MateIds { get; internal set; } = new List<int>();

        public long MaxGold { get; set; }

        public long MaxBankGold { get; set; }

        public short MaxHeroLevel { get; set; }

        public short MaxJobLevel { get; set; }

        public short MaxLevel { get; set; }

        public short MaxSpLevel { get; set; }

        public int MateXpRate { get; set; }

        public short MaxMateLevel { get; set; }

        public List<PenaltyLogDTO> PenaltyLogs { get; set; }

        public List<Schedule> Schedules { get; set; }

        public string ServerGroup { get; set; }

        public List<EventType> StartedEvents { get; set; }

        public int? RaidType { get; set; }

        public List<CharacterDTO> TopComplimented { get; set; }

        public List<CharacterDTO> TopPoints { get; set; }

        public List<CharacterDTO> TopReputation { get; set; }

        public Guid WorldId { get; private set; }

        public int XpRate { get; set; }

        public List<Card> Cards { get; set; }

        public ConcurrentBag<ScriptedInstance> Raids { get; set; }

        public ConcurrentBag<ScriptedInstance> TimeSpaces { get; set; }

        public List<Group> GroupList { get; set; } = new List<Group>();

        public List<ArenaMember> ArenaMembers { get; set; } = new List<ArenaMember>();

        public MapInstance Act4ShipDemon { get; set; }

        public MapInstance Act4ShipAngel { get; set; }

        public List<MapInstance> Act4Maps { get; set; }

        public PercentBar Act4AngelStat { get; set; }

        public PercentBar Act4DemonStat { get; set; }

        public PercentBar Act6Zenas { get; set; }

        public PercentBar Act6Erenia { get; set; }

        public DateTime Act4RaidStart { get; set; }

        public int AccountLimit { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public bool InShutdown { get; set; }

        public List<Quest> Quests { get; set; }

        public long? FlowerQuestId { get; set; }

        public long? CalvinQuest { get; set; }

        public long? MimiQuest { get; set; }

        public long? SluggQuest { get; set; }

        public long? EvaQuest { get; set; }

        public long? MalcolmQuest { get; set; }

        public MapInstance LobbyMapInstance { get; set; }

        public byte LobbySpeed { get; set; }

        #endregion

        #region Methods

        public void RefreshHomes()
        {
            CharacterHomes.ToList().Clear();
            CharacterHomes = DaoFactory.Instance.CharacterHomeDao.LoadAll();
        }

        public List<MapNpc> GetMapNpcsPerVNum(short vnum) => _mapNpcs.ContainsKey(vnum) ? _mapNpcs[vnum] : null;

        public bool ItemHasRecipe(short itemVNum)
        {
            return _recipeLists.Any(r => r.ItemVNum == itemVNum);
        }

        public Recipe GetRecipeByItemVNum(short itemVNum)
        {
            return _recipeLists.Find(s => s.ItemVNum == itemVNum);
        }

        public Card GetCardByCardId(short? cardId)
        {
            return Cards.Find(s => s.CardId == cardId);
        }

        public List<Recipe> GetRecipesByItemVNum(short itemVNum)
        {
            List<Recipe> recipes = new List<Recipe>();
            foreach (Recipe recipe in _recipeLists.Where(s => s.ProduceItemVNum == itemVNum))
            {
                recipes.Add(recipe);
            }

            return recipes;
        }

        public void AddGroup(Group group)
        {
            _groups[group.GroupId] = group;
        }

        public void AskPvpRevive(ClientSession session, ClientSession killer)
        {
            if (session?.Character == null || !session.HasSelectedCharacter)
            {
                return;
            }

            if (session.Character.IsVehicled)
            {
                session.Character.RemoveVehicle();
            }

            List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad, BuffType.Good, BuffType.Neutral };
            session.Character.DisableBuffs(bufftodisable);
            session.SendPacket(session.Character.GenerateStat());
            session.SendPacket(session.Character.GenerateCond());
            session.SendPackets(UserInterfaceHelper.Instance.GenerateVb());
            session.SendPacket("eff_ob -1 -1 0 4269");
            switch (session.CurrentMapInstance?.MapInstanceType)
            {
                case MapInstanceType.CaligorInstance:
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateInfo(
                            Language.Instance.GetMessageFromKey("RESPAWN_CALIGOR_ENTRY")));
                    Observable.Timer(TimeSpan.FromMilliseconds(5000)).Subscribe(o =>
                    {
                        session.Character.Hp = (int)session.Character.HpLoad();
                        session.Character.Mp = (int)session.Character.MpLoad();
                        if (CaligorMapInstance != null)
                        {
                            Instance.ChangeMapInstance(session.Character.CharacterId, CaligorMapInstance.MapInstanceId,
                                session.Character.Faction == FactionType.Angel ? 72 : 109, 159);
                        }

                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateTp());
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                        session.SendPacket(session.Character.GenerateStat());
                    });
                    break;
                case MapInstanceType.Act4Instance:
                    if (Instance.Act4DemonStat.Mode == 0 && Instance.Act4AngelStat.Mode == 0)
                    {
                        switch (session.Character?.Faction)
                        {
                            case FactionType.Angel:
                                Instance.Act4AngelStat.Percentage += 10000 / (GlacernonPercentRatePvp * 100);
                                break;
                            case FactionType.Demon:
                                Instance.Act4DemonStat.Percentage += 10000 / (GlacernonPercentRatePvp * 100);
                                break;
                        }
                    }

                    if (session.IpAddress != killer.IpAddress)
                    {
                        killer.Character.Act4Kill += 1;
                        session.Character.Act4Dead += 1;
                        session.Character.GetAct4Points(-1);
                        if (session.Character.Level + 10 >= killer.Character.Level &&
                            session.Character.Level <= killer.Character.Level - 10)
                        {
                            killer.Character.GetAct4Points(2);
                        }

                        if (session.Character.Reput < 50000)
                        {
                            session.SendPacket(session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("LOSE_REP"), 0), 11));
                        }
                        else
                        {
                            session.Character.LoseReput(session.Character.Level * 50);
                            killer.Character.GetReput(session.Character.Level * 50, true);
                            killer.SendPacket(session.Character.GenerateLev());
                        }
                    }

                    foreach (ClientSession sess in Instance.Sessions.Where(s =>
                        s.HasSelectedCharacter &&
                        s.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Instance))
                    {
                        if (sess.Character.Faction == killer.Character.Faction)
                        {
                            sess.SendPacket(sess.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("ACT4_PVP_KILL"),
                                    session.Character.Faction, killer.Character.Name), 12));
                        }
                        else if (sess.Character.Faction == session.Character.Faction)
                        {
                            sess.SendPacket(sess.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("ACT4_PVP_DEATH"),
                                    session.Character.Faction, session.Character.Name), 11));
                        }
                    }

                    session.SendPacket(session.Character.GenerateFd());
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(),
                        ReceiverType.AllExceptMe);
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(),
                        ReceiverType.AllExceptMe);
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ACT4_PVP_DIE"), 11));
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("ACT4_PVP_DIE"),
                            0));
                    Observable.Timer(TimeSpan.FromMilliseconds(2000)).Subscribe(o =>
                    {
                        session.CurrentMapInstance?.Broadcast(session,
                            $"c_mode 1 {session.Character.CharacterId} 1564 0 0 0");
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                    });
                    Observable.Timer(TimeSpan.FromMilliseconds(30000)).Subscribe(o =>
                    {
                        session.Character.Hp = (int)session.Character.HpLoad();
                        session.Character.Mp = (int)session.Character.MpLoad();
                        short x = (short)(39 + Instance.RandomNumber(-2, 3));
                        short y = (short)(42 + Instance.RandomNumber(-2, 3));
                        MapInstance citadel = Instance.Act4Maps.Find(s =>
                            s.Map.MapId == (session.Character.Faction == FactionType.Angel ? 130 : 131));
                        if (citadel != null)
                        {
                            Instance.ChangeMapInstance(session.Character.CharacterId, citadel.MapInstanceId, x, y);
                        }

                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateTp());
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                        session.SendPacket(session.Character.GenerateStat());
                    });
                    break;

                case MapInstanceType.IceBreakerInstance:
                    if (IceBreaker.AlreadyFrozenPlayers.Contains(session))
                    {
                        IceBreaker.AlreadyFrozenPlayers.Remove(session);
                        Group targetGroup = IceBreaker.GetGroupByClientSession(session);
                        if (targetGroup?.Characters.Count - 1 < 1)
                        {
                            IceBreaker.RemoveGroup(targetGroup);
                        }

                        session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.Instance.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("ICEBREAKER_PLAYER_OUT"),
                                session.Character?.Name), 0));
                        session.Character.Hp = 1;
                        session.Character.Mp = 1;
                        RespawnMapTypeDTO respawn = session.Character?.Respawn;
                        Instance.ChangeMap(session.Character.CharacterId, respawn.DefaultMapId);
                        killer.SendPacket($"cancel 2 {session.Character?.CharacterId}");
                    }
                    else
                    {
                        IceBreaker.FrozenPlayers.Add(session);
                        session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.Instance.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("ICEBREAKER_PLAYER_FROZEN"),
                                session.Character?.Name), 0));
                        session.Character.Hp = (int)session.Character.HpLoad();
                        session.Character.Mp = (int)session.Character.MpLoad();
                        session.SendPacket(session.Character?.GenerateStat());
                        session.SendPacket(session.Character?.GenerateCond());
                        IDisposable obs = null;
                        obs = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(s =>
                        {
                            if (IceBreaker.FrozenPlayers.Contains(session))
                            {
                                session.CurrentMapInstance?.Broadcast(session.Character?.GenerateEff(35));
                            }
                            else
                            {
                                obs?.Dispose();
                            }
                        });
                    }

                    break;

                case MapInstanceType.ArenaInstance:
                    killer.Character.TalentWin += 1;
                    session.Character.TalentLose += 1;
                    session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.Instance.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("ARENA_KILL"), session.Character?.Name,
                            killer.Character?.Name), 0));
                    goto default;

                case MapInstanceType.TalentArenaMapInstance:
                    ConcurrentBag<ArenaTeamMember> team = Instance.ArenaTeams.Find(s => s.Any(o => o.Session == session));
                    ArenaTeamMember member = team?.FirstOrDefault(s => s.Session == session);
                    if (member != null)
                    {
                        if (member.LastSummoned == null)
                        {
                            session.CurrentMapInstance.InstanceBag.DeadList.Add(session.Character.CharacterId);
                            member.Dead = true;
                            team.ToList().Where(s => s.LastSummoned != null).ToList().ForEach(s =>
                            {
                                s.LastSummoned = null;
                                s.Session.Character.PositionX = s.ArenaTeamType == ArenaTeamType.ERENIA ? (short)120 : (short)19;
                                s.Session.Character.PositionY = s.ArenaTeamType == ArenaTeamType.ERENIA ? (short)39 : (short)40;
                                session.CurrentMapInstance.Broadcast(s.Session.Character.GenerateTp());
                                s.Session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));
                            });
                            ArenaTeamMember arenaKiller = team.OrderBy(s => s.Order).FirstOrDefault(s => !s.Dead && s.ArenaTeamType != member.ArenaTeamType);
                            session.CurrentMapInstance.Broadcast(
                                session.Character.GenerateSay(string.Format("TEAM_WINNER_ARENA_ROUND", arenaKiller?.Session.Character.Name, arenaKiller?.ArenaTeamType), 10));
                            session.CurrentMapInstance.Broadcast(
                                UserInterfaceHelper.Instance.GenerateMsg(string.Format("TEAM_WINNER_ARENA_ROUND", arenaKiller?.Session.Character.Name, arenaKiller?.ArenaTeamType), 0));
                            session.CurrentMapInstance.Sessions.Except(team.Where(s => s.ArenaTeamType == arenaKiller?.ArenaTeamType).Select(s => s.Session)).ToList().ForEach(o =>
                            {
                                if (arenaKiller?.ArenaTeamType == ArenaTeamType.ERENIA)
                                {
                                    o.SendPacket(arenaKiller.Session.Character.GenerateTaM(2));
                                    o.SendPacket(arenaKiller.Session.Character.GenerateTaP(2, true));
                                }
                                else
                                {
                                    o.SendPacket(member.Session.Character.GenerateTaM(2));
                                    o.SendPacket(member.Session.Character.GenerateTaP(2, true));
                                }

                                o.SendPacket($"taw_d {member.Session.Character.CharacterId}");
                                o.SendPacket(member.Session.Character.GenerateSay(
                                    string.Format("WINNER_ARENA_ROUND", arenaKiller?.Session.Character.Name, arenaKiller?.ArenaTeamType, member.Session.Character.Name), 10));
                                o.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                    string.Format("WINNER_ARENA_ROUND", arenaKiller?.Session.Character.Name, arenaKiller?.ArenaTeamType, member.Session.Character.Name), 0));
                            });
                        }

                        member.Session.Character.PositionX = member.ArenaTeamType == ArenaTeamType.ERENIA ? (short)120 : (short)19;
                        member.Session.Character.PositionY = member.ArenaTeamType == ArenaTeamType.ERENIA ? (short)39 : (short)40;
                        session.CurrentMapInstance.Broadcast(member.Session, member.Session.Character.GenerateTp());
                        session.SendPacket(UserInterfaceHelper.Instance.GenerateTaSt(TalentArenaOptionType.Watch));
                        team.Where(friends => friends.ArenaTeamType == member.ArenaTeamType).ToList().ForEach(friends => { friends.Session.SendPacket(friends.Session.Character.GenerateTaFc(0)); });
                        team.ToList().ForEach(arenauser =>
                        {
                            arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaP(2, true));
                            arenauser.Session.SendPacket(arenauser.Session.Character.GenerateTaM(2));
                        });

                        session.Character.Hp = (int)session.Character.HpLoad();
                        session.Character.Mp = (int)session.Character.MpLoad();
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateRevive());
                        session.SendPacket(session.Character.GenerateStat());
                    }

                    break;

                default:
                    session.Character.LeaveTalentArena(true);
                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                        $"#revival^2 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_PVP")}"));
                    Task.Factory.StartNew(async () =>
                    {
                        bool revive = true;
                        for (int i = 1; i <= 30; i++)
                        {
                            await Task.Delay(1000);
                            if (session.Character.Hp <= 0)
                            {
                                continue;
                            }

                            revive = false;
                            break;
                        }

                        if (revive)
                        {
                            Instance.ReviveFirstPosition(session.Character.CharacterId);
                        }
                    });
                    break;
            }
        }

        // PacketHandler -> with Callback?
        public void AskRevive(long characterId, ClientSession killer = null)
        {
            ClientSession session = GetSessionByCharacterId(characterId);
            if (session?.HasSelectedCharacter != true || session.CurrentMapInstance == null ||
                session.Character.LastDeath.AddSeconds(1) > DateTime.Now)
            {
                return;
            }

            if (killer?.Character != null)
            {
                AskPvpRevive(session, killer);
                return;
            }

            if (session.Character.IsVehicled)
            {
                session.Character.RemoveVehicle();
            }

            List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad, BuffType.Good, BuffType.Neutral };
            session.Character.DisableBuffs(bufftodisable);
            session.SendPacket(session.Character.GenerateStat());
            session.SendPacket(session.Character.GenerateCond());
            session.SendPackets(UserInterfaceHelper.Instance.GenerateVb());
            session.Character.LastDeath = DateTime.Now;
            switch (session.CurrentMapInstance.MapInstanceType)
            {
                case MapInstanceType.Act4Instance:
                    if (session.Character.Level > 20)
                    {
                        session.Character.Dignity -=
                            (short)(session.Character.Level < 50 ? session.Character.Level : 50);
                        if (session.Character.Dignity < -1000)
                        {
                            session.Character.Dignity = -1000;
                        }

                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("LOSE_DIGNITY"),
                                (short)(session.Character.Level < 50 ? session.Character.Level : 50)), 11));
                        session.SendPacket(session.Character.GenerateFd());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(),
                            ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(),
                            ReceiverType.AllExceptMe);
                    }

                    session.SendPacket("eff_ob -1 -1 0 4269");

                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                        $"#revival^0 #revival^1 {(session.Character.Level > 20 ? Language.Instance.GetMessageFromKey("ASK_REVIVE") : Language.Instance.GetMessageFromKey("ASK_REVIVE_FREE"))}"));
                    RespawnMapTypeDTO a4respawn = session.Character.Respawn;
                    session.Character.MapX = (short)(a4respawn.DefaultX + RandomNumber(-3, 3));
                    session.Character.MapY = (short)(a4respawn.DefaultY + RandomNumber(-3, 3));
                    Task.Factory.StartNew(async () =>
                    {
                        bool revive = true;
                        for (int i = 1; i <= 30; i++)
                        {
                            await Task.Delay(1000);
                            if (session.Character.Hp <= 0)
                            {
                                continue;
                            }

                            revive = false;
                            break;
                        }

                        if (revive)
                        {
                            Instance.ReviveFirstPosition(session.Character.CharacterId);
                        }
                    });
                    break;
                case MapInstanceType.CaligorInstance:
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateInfo(
                            Language.Instance.GetMessageFromKey("RESPAWN_CALIGOR_ENTRY")));
                    Observable.Timer(TimeSpan.FromMilliseconds(5000)).Subscribe(o =>
                    {
                        session.Character.Hp = (int)session.Character.HpLoad();
                        session.Character.Mp = (int)session.Character.MpLoad();
                        if (CaligorMapInstance != null)
                        {
                            Instance.ChangeMapInstance(session.Character.CharacterId, CaligorMapInstance.MapInstanceId,
                                session.Character.Faction == FactionType.Angel ? 72 : 109, 159);
                        }

                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateTp());
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                        session.SendPacket(session.Character.GenerateStat());
                    });
                    break;
                case MapInstanceType.BaseMapInstance:
                    if (session.Character.Level > 20)
                    {
                        session.Character.Dignity -=
                            (short)(session.Character.Level < 50 ? session.Character.Level : 50);
                        if (session.Character.Dignity < -1000)
                        {
                            session.Character.Dignity = -1000;
                        }

                        session.SendPacket(session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("LOSE_DIGNITY"),
                                (short)(session.Character.Level < 50 ? session.Character.Level : 50)), 11));
                        session.SendPacket(session.Character.GenerateFd());
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateIn(),
                            ReceiverType.AllExceptMe);
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateGidx(),
                            ReceiverType.AllExceptMe);
                    }

                    session.SendPacket("eff_ob -1 -1 0 4269");

                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                        $"#revival^0 #revival^1 {(session.Character.Level > 20 ? Language.Instance.GetMessageFromKey("ASK_REVIVE") : Language.Instance.GetMessageFromKey("ASK_REVIVE_FREE"))}"));
                    RespawnMapTypeDTO resp = session.Character.Respawn;
                    session.Character.MapX = (short)(resp.DefaultX + RandomNumber(-3, 3));
                    session.Character.MapY = (short)(resp.DefaultY + RandomNumber(-3, 3));
                    Task.Factory.StartNew(async () =>
                    {
                        bool revive = true;
                        for (int i = 1; i <= 30; i++)
                        {
                            await Task.Delay(1000);
                            if (session.Character.Hp <= 0)
                            {
                                continue;
                            }

                            revive = false;
                            break;
                        }

                        if (revive)
                        {
                            Instance.ReviveFirstPosition(session.Character.CharacterId);
                        }
                    });
                    break;

                case MapInstanceType.TimeSpaceInstance:
                    if (!(session.CurrentMapInstance.InstanceBag.Lives -
                        session.CurrentMapInstance.InstanceBag.DeadList.Count <= 1))
                    {
                        session.Character.Hp = 1;
                        session.Character.Mp = 1;
                        return;
                    }

                    session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("YOU_HAVE_LIFE"),
                            session.CurrentMapInstance.InstanceBag.Lives -
                            session.CurrentMapInstance.InstanceBag.DeadList.Count + 1),
                        0));
                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                        $"#revival^1 #revival^1 {(session.Character.Level > 10 ? Language.Instance.GetMessageFromKey("ASK_REVIVE_TS_LOW_LEVEL") : Language.Instance.GetMessageFromKey("ASK_REVIVE_TS"))}"));
                    session.CurrentMapInstance.InstanceBag.DeadList.Add(session.Character.CharacterId);
                    Task.Factory.StartNew(async () =>
                    {
                        bool revive = true;
                        for (int i = 1; i <= 30; i++)
                        {
                            await Task.Delay(1000);
                            if (session.Character.Hp <= 0)
                            {
                                continue;
                            }

                            revive = false;
                            break;
                        }

                        if (revive)
                        {
                            Instance.ReviveFirstPosition(session.Character.CharacterId);
                        }
                    });

                    break;

                case MapInstanceType.RaidInstance:
                    if (session.Character.Family?.Act4Raid?.Maps?.Any(m => m == session.CurrentMapInstance) ?? false)
                    {
                        session.Character.LoseReput(session.Character.Level * 2);
                        Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(5000);
                            Instance.ReviveFirstPosition(session.Character.CharacterId);
                        });
                    }
                    else
                    {
                        List<long> deadList = session.CurrentMapInstance.InstanceBag.DeadList.ToList();
                        if (session.CurrentMapInstance.InstanceBag.DeadList.Count >
                            session.CurrentMapInstance.InstanceBag.Lives)
                        {
                            session.Character.Hp = 1;
                            session.Character.Mp = 1;
                            session.Character.Group?.Raid?.End();
                            return;
                        }

                        if (deadList.Count(s => s == session.Character.CharacterId) < 2)
                        {
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(
                                string.Format(Language.Instance.GetMessageFromKey("YOU_HAVE_LIFE_RAID"),
                                    2 - session.CurrentMapInstance.InstanceBag.DeadList.Count(s =>
                                        s == session.Character.CharacterId))));
                            session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(
                                string.Format(Language.Instance.GetMessageFromKey("RAID_MEMBER_DEAD"),
                                    session.Character.Name)));
                            session.CurrentMapInstance.InstanceBag.DeadList.Add(session.Character.CharacterId);
                            if (session.Character?.Group?.Characters != null)
                            {
                                foreach (ClientSession player in session.Character.Group?.Characters)
                                {
                                    player?.SendPacket(
                                        player?.Character?.Group?.GeneraterRaidmbf(player?.CurrentMapInstance));
                                    player?.SendPacket(player?.Character?.Group?.GenerateRdlst());
                                }
                            }

                            Task.Factory.StartNew(async () =>
                            {
                                await Task.Delay(20000);
                                Instance.ReviveFirstPosition(session.Character.CharacterId);
                            });
                        }
                        else
                        {
                            Group grp = session.Character.Group;
                            if (grp != null)
                            {
                                session.CurrentMapInstance.InstanceBag.DeadList.Add(session.Character.CharacterId);
                                if (session.Character.Hp <= 0)
                                {
                                    session.Character.Hp = 1;
                                    session.Character.Mp = 1;
                                }

                                grp.Characters.Where(s => s != null).ToList().ForEach(s =>
                                {
                                    s.SendPacket(s.Character?.Group?.GeneraterRaidmbf(s.CurrentMapInstance));
                                    s.SendPacket(s.Character?.Group?.GenerateRdlst());
                                });
                                session.SendPacket(session.Character.GenerateRaid(1, true));
                                session.SendPacket(session.Character.GenerateRaid(2, true));
                                grp.LeaveGroup(session);
                                session.SendPacket(
                                    UserInterfaceHelper.Instance.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("KICKED_FROM_RAID"), 0));
                                ChangeMap(session.Character.CharacterId, 1, 78, 111);
                            }
                        }
                    }

                    break;

                case MapInstanceType.LodInstance:
                    session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                        $"#revival^0 #revival^1 {Language.Instance.GetMessageFromKey("ASK_REVIVE_LOD")}"));
                    Task.Factory.StartNew(async () =>
                    {
                        bool revive = true;
                        for (int i = 1; i <= 30; i++)
                        {
                            await Task.Delay(1000);
                            if (session.Character.Hp <= 0)
                            {
                                continue;
                            }

                            revive = false;
                            break;
                        }

                        if (revive)
                        {
                            Instance.ReviveFirstPosition(session.Character.CharacterId);
                        }
                    });
                    break;

                case MapInstanceType.BattleRoyaleMapInstance:
                    BattleRoyaleManager.Instance.Kick(session, killer);
                    Instance.ReviveFirstPosition(session.Character.CharacterId);
                    break;

                default:
                    Instance.ReviveFirstPosition(session.Character.CharacterId);
                    break;
            }
        }

        public void BazaarRefresh(long bazaarItemId)
        {
            InBazaarRefreshMode = true;
            CommunicationServiceClient.Instance.UpdateBazaar(ServerGroup, bazaarItemId);
            SpinWait.SpinUntil(() => !InBazaarRefreshMode);
        }

        public void ChangeMap(long id, short? mapId = null, short? mapX = null, short? mapY = null)
        {
            ClientSession session = GetSessionByCharacterId(id);
            if (session?.Character == null)
            {
                return;
            }

            if (mapId != null)
            {
                session.Character.MapInstanceId = GetBaseMapInstanceIdByMapId((short)mapId);
            }

            try
            {
                KeyValuePair<Guid, MapInstance> unused =
                    _mapInstances.First(x => x.Key == session.Character.MapInstanceId);
            }
            catch
            {
                return;
            }

            if (mapId == (short)SpecialMapIdType.Lobby)
            {
                TeleportToLobby(session);
                return;
            }

            ChangeMapInstance(id, session.Character.MapInstanceId, mapX, mapY);
        }

        // Both partly
        public void ChangeMapInstance(long id, Guid mapInstanceId, int? mapX = null, int? mapY = null)
        {
            ClientSession session = GetSessionByCharacterId(id);
            if (session?.Character == null || session.Character.IsChangingMapInstance)
            {
                return;
            }

            try
            {
                if (session.Character.Authority >= AuthorityType.VipPlus &&
                    session.Character.StaticBonusList.All(s => s.StaticBonusType != StaticBonusType.PetBasket))
                {
                    session.Character.StaticBonusList.Add(new StaticBonusDTO
                    {
                        CharacterId = session.Character.CharacterId,
                        DateEnd = DateTime.Now.AddDays(60),
                        StaticBonusType = StaticBonusType.PetBasket
                    });
                }

                if (session.Character.IsExchanging || session.Character.InExchangeOrTrade)
                {
                    session.Character.CloseExchangeOrTrade();
                }

                if (session.Character.HasShopOpened)
                {
                    session.Character.CloseShop();
                }

                session.Character.LeaveTalentArena();
                session.CurrentMapInstance.RemoveMonstersTarget(session.Character);
                session.Character.Mates.Where(m => m.IsTeamMember).ToList()
                    .ForEach(mate => session.CurrentMapInstance.RemoveMonstersTarget(mate));
                session.CurrentMapInstance.UnregisterSession(session.Character.CharacterId);
                LeaveMap(session.Character.CharacterId);
                session.Character.IsChangingMapInstance = true;
                if (session.Character.IsSitting)
                {
                    session.Character.IsSitting = false;
                }

                // cleanup sending queue to avoid sending uneccessary packets to it
                session.ClearLowPriorityQueue();
                bool isLeavingLobby = session.Character.MapInstanceId == LobbyMapInstance.MapInstanceId;
                session.Character.MapInstanceId = mapInstanceId;
                if (session.Character.MapInstance.Map.MapId == (short)SpecialMapIdType.Lobby &&
                    session.Character.MapInstance != LobbyMapInstance)
                {
                    session.Character.MapInstanceId = LobbyMapInstance.MapInstanceId;
                }

                if (session.Character.MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance ||
                    session.Character.MapInstance.MapInstanceType == MapInstanceType.LobbyMapInstance)
                {
                    session.Character.MapId = session.Character.MapInstance.Map.MapId;
                    if (mapX != null && mapY != null)
                    {
                        session.Character.MapX = (short)mapX;
                        session.Character.MapY = (short)mapY;
                    }
                }

                if (mapX != null && mapY != null)
                {
                    session.Character.PositionX = (short)mapX;
                    session.Character.PositionY = (short)mapY;
                }

                session.CurrentMapInstance = session.Character.MapInstance;
                session.CurrentMapInstance.RegisterSession(session);

                session.SendPacket(session.Character.GenerateCInfo());
                session.SendPacket(session.Character.GenerateCMode());
                session.SendPacket(session.Character.GenerateEq());
                session.SendPacket(session.Character.GenerateEquipment());
                session.SendPacket(session.Character.GenerateLev());
                session.SendPacket(session.Character.GenerateStat());
                session.SendPacket(session.Character.GenerateAt());
                session.SendPacket(session.Character.GenerateCond());
                session.SendPacket(session.Character.GenerateCMap());
                session.SendPacket(session.Character.GenerateStatChar());
                session.SendPacket(session.Character.GeneratePairy());
                session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember)
                    .OrderBy(s => s.MateType)
                    .Select(s => s.GeneratePst()));
                session.Character.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                {
                    if (!session.Character.IsVehicled)
                    {
                        s.PositionX = (short)(session.Character.PositionX + (s.MateType == MateType.Partner ? -1 : 1));
                        s.PositionY = (short)(session.Character.PositionY + 1);
                        bool isBlocked = session.Character.MapInstance.Map.IsBlockedZone(s.PositionX, s.PositionY);
                        if (isBlocked)
                        {
                            s.PositionX = session.Character.PositionX;
                            s.PositionY = session.Character.PositionY;
                        }

                        session.SendPacket(s.GenerateIn());
                    }
                });
                session.SendPacket(
                    session.Character.MapInstance.Map.MapId >= 228 && session.Character.MapInstance.Map.MapId <= 238 ||
                    session.Character.MapInstance.Map.MapId == 2604
                        ? session.Character.GenerateAct6()
                        : session.Character.GenerateAct());
                session.SendPacket(session.Character.GeneratePinit());
                session.Character.Mates.ForEach(s => session.SendPacket(s.GenerateScPacket()));
                session.SendPacket(session.Character.GenerateScpStc());
                if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4Instance)
                {
                    session.SendPacket(session.Character.GenerateFc());
                }
                else if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                {
                    if (session.Character.Family?.Act4Raid?.Maps?.Any(m => m.MapInstanceId == mapInstanceId) ?? false)
                    {
                        session.SendPacket(session.Character.GenerateDg());
                    }
                    else
                    {
                        session.SendPacket(session.Character?.Group?.GeneraterRaidmbf(session.CurrentMapInstance));
                    }
                }

                session.SendPacket(session.CurrentMapInstance.GenerateMapDesignObjects());
                session.SendPackets(session.CurrentMapInstance.GetMapDesignObjectEffects());
                session.SendPackets(session.CurrentMapInstance.GetMapItems());
                MapInstancePortalHandler
                    .GenerateMinilandEntryPortals(session.CurrentMapInstance.Map.MapId,
                        session.Character.Miniland.MapInstanceId).ForEach(p => session.SendPacket(p.GenerateGp()));
                if (session.CurrentMapInstance.InstanceBag.Clock.Enabled)
                {
                    session.SendPacket(session.CurrentMapInstance.InstanceBag.Clock.GetClock());
                }

                if (session.CurrentMapInstance.Clock.Enabled)
                {
                    session.SendPacket(session.CurrentMapInstance.InstanceBag.Clock.GetClock());
                }

                // TODO: fix this
                if (session.Character.MapInstance.Map.MapTypes.Any(m =>
                    m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness))
                {
                    session.SendPacket("bc 0 0 0");
                }

                if (session.Character.Size != 10)
                {
                    session.SendPacket(session.Character.GenerateScal());
                }

                switch (session.CurrentMapInstance?.IsDancing)
                {
                    case true when !session.Character.IsDancing:
                        session.CurrentMapInstance?.Broadcast("dance 2");
                        break;
                    case false when session.Character.IsDancing:
                        session.Character.IsDancing = false;
                        session.CurrentMapInstance?.Broadcast("dance");
                        break;
                }

                if (Groups != null)
                {
                    Parallel.ForEach(Groups, group =>
                    {
                        foreach (ClientSession groupSession in group.Characters)
                        {
                            ClientSession chara = Sessions.FirstOrDefault(s =>
                                s.Character != null && s.Character.CharacterId == groupSession.Character.CharacterId &&
                                s.CurrentMapInstance == groupSession.CurrentMapInstance);
                            if (chara == null)
                            {
                                continue;
                            }

                            groupSession.SendPacket(groupSession.Character.GeneratePinit());
                            groupSession.SendPackets(groupSession.Character.Mates.Where(s => s.IsTeamMember)
                                .OrderBy(s => s.MateType)
                                .Select(s => s.GeneratePst()));
                        }
                    });
                }

                if (session.Character.Group?.GroupType == GroupType.Group)
                {
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GeneratePidx(),
                        ReceiverType.AllExceptMe);
                }

                if (session.CurrentMapInstance?.Map.MapTypes.All(s => s.MapTypeId != (short)MapTypeEnum.Act52) ==
                    true && session.Character.Buff.Any(s => s.Card.CardId == 339)) //Act5.2 debuff
                {
                    session.Character.RemoveBuff(339, true);
                    session.Character.DotDebuff?.Dispose();
                }
                else if (session.CurrentMapInstance?.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act52) ==
                    true && session.Character.Buff.All(s => s.Card.CardId != 339 && s.Card.CardId != 340))
                {
                    session.Character.AddStaticBuff(new StaticBuffDTO
                    {
                        CardId = 339,
                        CharacterId = session.Character.CharacterId,
                        RemainingTime = -1
                    }, true);
                }

                if (!session.Character.InvisibleGm && session.CurrentMapInstance != null)
                {
                    Parallel.ForEach(
                        session.CurrentMapInstance.Sessions.Where(s => s.Character != null && s != session), s =>
                        {
                            if (session.CurrentMapInstance.MapInstanceType != MapInstanceType.Act4Instance &&
                                session.CurrentMapInstance.MapInstanceType != MapInstanceType.CaligorInstance ||
                                session.Character.Faction == s.Character.Faction)
                            {
                                s.SendPacket(session.Character.GenerateIn());
                                s.SendPacket(session.Character.GenerateGidx());
                                if (!session.Character.IsVehicled)
                                {
                                    session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m => { s.SendPacket(m.GenerateIn()); });
                                }
                            }
                            else
                            {
                                s.SendPacket(session.Character.GenerateIn(true));
                                if (!session.Character.IsVehicled)
                                {
                                    session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m => { s.SendPacket(m.GenerateIn(true)); });
                                }
                            }
                        });
                }

                if (session.CurrentMapInstance != null)
                {
                    Parallel.ForEach(
                        session.CurrentMapInstance.Sessions.Where(
                            s => s.Character?.InvisibleGm == false && s != session), visibleSession =>
                        {
                            if (session.CurrentMapInstance.MapInstanceType != MapInstanceType.Act4Instance &&
                                session.CurrentMapInstance.MapInstanceType != MapInstanceType.CaligorInstance ||
                                session.Character.Faction == visibleSession.Character.Faction)
                            {
                                session.SendPacket(visibleSession.Character.GenerateIn());
                                session.SendPacket(visibleSession.Character.GenerateGidx());

                                if (visibleSession.Character.HasShopOpened && visibleSession.HasCurrentMapInstance)
                                {
                                    KeyValuePair<long, MapShop> shop =
                                        visibleSession.CurrentMapInstance.UserShops.FirstOrDefault(mapshop =>
                                            mapshop.Value.OwnerId.Equals(visibleSession.Character.GetId()));

                                    session.SendPacket(visibleSession.Character.GeneratePlayerFlag(shop.Key + 1));
                                    session.SendPacket(visibleSession.Character.GenerateShop(shop.Value.Name));
                                }

                                if (!visibleSession.Character.IsVehicled)
                                {
                                    visibleSession.Character.Mates
                                        .Where(m => m.IsTeamMember && m.CharacterId != session.Character.CharacterId)
                                        .ToList().ForEach(mate =>
                                        {
                                            session.SendPacket(mate.GenerateIn(false,
                                                session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType
                                                    .Act4Instance)));
                                        });
                                }
                            }
                            else
                            {
                                session.SendPacket(visibleSession.Character.GenerateIn(true));

                                if (visibleSession.Character.HasShopOpened && visibleSession.HasCurrentMapInstance)
                                {
                                    KeyValuePair<long, MapShop> shop =
                                        visibleSession.CurrentMapInstance.UserShops.FirstOrDefault(mapshop =>
                                            mapshop.Value.OwnerId.Equals(visibleSession.Character.GetId()));

                                    session.SendPacket(visibleSession.Character.GeneratePlayerFlag(shop.Key + 1));
                                    session.SendPacket(visibleSession.Character.GenerateShop(shop.Value.Name));
                                }

                                if (!visibleSession.Character.IsVehicled)
                                {
                                    visibleSession.Character.Mates.Where(m =>
                                            m.IsTeamMember && m.CharacterId != session.Character.CharacterId).ToList()
                                        .ForEach(m => { session.SendPacket(m.GenerateIn(true, true)); });
                                }
                            }
                        });
                }

                if (session.Character.MapInstance == LobbyMapInstance) // Zoom
                {
                    session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(15, 1, session.Character.CharacterId));
                }
                else if (isLeavingLobby)
                {
                    session.SendPacket(UserInterfaceHelper.Instance.GenerateGuri(15, 0, session.Character.CharacterId));
                }

                session.Character.LoadSpeed();
                session.SendPacket(session.Character.GenerateCond());

                session.Character.IsChangingMapInstance = false;
                session.SendPacket(session.Character.GenerateMinimapPosition());
                session.CurrentMapInstance.OnCharacterDiscoveringMapEvents.ToList().ForEach(e =>
                {
                    (EventContainer eventContainer, List<long> eventIds) = e;
                    if (eventIds.Contains(session.Character.CharacterId))
                    {
                        return;
                    }

                    eventIds.Add(session.Character.CharacterId);
                    EventHelper.Instance.RunEvent(eventContainer, session);
                });
            }
            catch (Exception)
            {
                Logger.Log.Warn("Character changed while changing map. Do not abuse Commands.");
                session.Character.IsChangingMapInstance = false;
            }
        }

        public void DisconnectAll()
        {
            foreach (ClientSession session in Sessions)
            {
                session?.Destroy();
            }
        }

        public sealed override void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            GC.SuppressFinalize(this);
            _disposed = true;
        }

        public void FamilyRefresh(long familyId, bool changeFaction = false)
        {
            CommunicationServiceClient.Instance.UpdateFamily(ServerGroup, familyId, changeFaction);
        }

        public MapInstance GenerateMapInstance(short mapId, MapInstanceType type, InstanceBag mapclock)
        {
            Map map = Maps.Find(m => m.MapId.Equals(mapId));
            if (map == null)
            {
                return null;
            }

            Guid guid = Guid.NewGuid();
            var mapInstance = new MapInstance(map, guid, false, type, mapclock);
            mapInstance.LoadMonsters();
            mapInstance.LoadNpcs();
            mapInstance.LoadPortals();
            Parallel.ForEach(mapInstance.Monsters, mapMonster =>
            {
                mapMonster.MapInstance = mapInstance;
                mapInstance.AddMonster(mapMonster);
            });
            Parallel.ForEach(mapInstance.Npcs, mapNpc =>
            {
                mapNpc.MapInstance = mapInstance;
                mapInstance.AddNpc(mapNpc);
            });
            _mapInstances.TryAdd(guid, mapInstance);
            return mapInstance;
        }

        public IEnumerable<Skill> GetAllSkill() => Skills;

        public Guid GetBaseMapInstanceIdByMapId(short mapId)
        {
            return _mapInstances.FirstOrDefault(s =>
                s.Value?.Map.MapId == mapId && s.Value.MapInstanceType == MapInstanceType.BaseMapInstance).Key;
        }

        public List<DropDTO> GetDropsByMonsterVNum(short monsterVNum) => _monsterDrops.ContainsKey(monsterVNum)
            ? _generalDrops.Concat(_monsterDrops[monsterVNum]).ToList()
            : new List<DropDTO>();

        public Group GetGroupByCharacterId(long characterId)
        {
            return Groups?.SingleOrDefault(g => g.IsMemberOfGroup(characterId));
        }

        public Item GetItem(short vnum)
        {
            return Items.Find(m => m.VNum.Equals(vnum));
        }

        public MapInstance GetMapInstance(Guid id) => _mapInstances.ContainsKey(id) ? _mapInstances[id] : null;

        public IEnumerable<MapInstance> GetMapInstancesByMapInstanceType(MapInstanceType type)
        {
            return _mapInstances.Values.Where(s => s.MapInstanceType == type);
        }

        public long GetNextGroupId()
        {
            _lastGroupId++;
            return _lastGroupId;
        }


        public NpcMonster GetNpc(short npcVNum)
        {
            return Npcs.Find(m => m.NpcMonsterVNum.Equals(npcVNum));
        }

        public T GetProperty<T>(string charName, string property)
        {
            ClientSession session =
                Sessions.FirstOrDefault(s => s.Character?.Name.Equals(charName) == true);
            if (session == null)
            {
                return default;
            }

            return (T)session.Character.GetType().GetProperties().Single(pi => pi.Name == property)
                .GetValue(session.Character, null);
        }

        public T GetProperty<T>(long charId, string property)
        {
            ClientSession session = GetSessionByCharacterId(charId);
            if (session == null)
            {
                return default;
            }

            return (T)session.Character.GetType().GetProperties().Single(pi => pi.Name == property)
                .GetValue(session.Character, null);
        }

        public List<Recipe> GetReceipesByMapNpcId(int mapNpcId) => _recipes.ContainsKey(mapNpcId) ? _recipes[mapNpcId] : new List<Recipe>();

        public ClientSession GetSessionByCharacterName(string name)
        {
            return Sessions.SingleOrDefault(s => s.Character.Name == name);
        }

        public ClientSession GetSessionBySessionId(int sessionId)
        {
            return Sessions.SingleOrDefault(s => s.SessionId == sessionId);
        }

        public Skill GetSkill(short skillVNum)
        {
            return Skills.Find(m => m.SkillVNum.Equals(skillVNum));
        }

        public Quest GetQuest(long questId)
        {
            return Quests.Find(m => m.QuestId.Equals(questId));
        }

        public T GetUserMethod<T>(long characterId, string methodName)
        {
            ClientSession session = GetSessionByCharacterId(characterId);
            if (session == null)
            {
                return default;
            }

            MethodInfo method = session.Character.GetType().GetMethod(methodName);

            return (T)method?.Invoke(session.Character, null);
        }

        public void GroupLeave(ClientSession session)
        {
            if (Groups == null)
            {
                return;
            }

            Group grp = Instance.Groups.Find(s => s.IsMemberOfGroup(session.Character.CharacterId));
            if (grp == null)
            {
                return;
            }

            if (grp.CharacterCount >= 3 && grp.GroupType == GroupType.Group ||
                grp.CharacterCount >= 2 && grp.GroupType != GroupType.Group)
            {
                if (grp.IsLeader(session))
                {
                    Broadcast(session,
                        UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("NEW_LEADER")),
                        ReceiverType.OnlySomeone, string.Empty,
                        grp.Characters.ElementAt(1).Character.CharacterId);
                }

                grp.LeaveGroup(session);

                if (grp.GroupType == GroupType.Group)
                {
                    foreach (ClientSession groupSession in grp.Characters)
                    {
                        groupSession.SendPacket(groupSession.Character.GeneratePinit());
                        groupSession.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember)
                            .OrderBy(s => s.MateType)
                            .Select(s => s.GeneratePst()));
                        groupSession.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("LEAVE_GROUP"), session.Character.Name),
                            0));
                    }

                    session.SendPacket(session.Character.GeneratePinit());
                    session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember)
                        .OrderBy(s => s.MateType)
                        .Select(s => s.GeneratePst()));
                    Broadcast(session.Character.GeneratePidx(true));
                    session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_LEFT"), 0));
                }
                else
                {
                    foreach (ClientSession groupSession in grp.Characters)
                    {
                        session.SendPacket(session.Character.GenerateRaid(1, true));
                        session.SendPacket(session.Character.GenerateRaid(2, true));
                        groupSession.SendPacket(grp.GenerateRdlst());
                        groupSession.SendPacket(groupSession.Character.GenerateRaid(0, false));
                    }

                    if (session?.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                    {
                        Instance.ChangeMap(session.Character.CharacterId, session.Character.MapId,
                            session.Character.MapX, session.Character.MapY);
                    }

                    session?.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_LEFT"), 0));
                }
            }
            else
            {
                ClientSession[] grpmembers = new ClientSession[40];
                grp.Characters.ToList().CopyTo(grpmembers);
                foreach (ClientSession targetSession in grpmembers)
                {
                    if (targetSession == null)
                    {
                        continue;
                    }

                    targetSession.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("GROUP_CLOSED"),
                            0));
                    Broadcast(targetSession.Character.GeneratePidx(true));
                    grp.LeaveGroup(targetSession);
                    targetSession.SendPacket(targetSession.Character.GeneratePinit());
                    targetSession.SendPackets(targetSession.Character.Mates.Where(s => s.IsTeamMember)
                        .OrderBy(s => s.MateType)
                        .Select(s => s.GeneratePst()));
                }

                GroupList.RemoveAll(s => s.GroupId == grp.GroupId);
                _groups.TryRemove(grp.GroupId, out Group value);
            }

            if (session != null)
            {
                session.Character.Group = null;
            }
        }

        private void InitializeConfigurations()
        {
            XpRate = RateConfiguration.XpRate;
            JobXpRate = RateConfiguration.JobXpRate;
            HeroXpRate = RateConfiguration.HeroXpRate;
            FairyXpRate = RateConfiguration.FairyXpRate;
            MateXpRate = RateConfiguration.MateXpRate;
            ReputRate = RateConfiguration.ReputRate;
            DropRate = RateConfiguration.DropRate;
            QuestDropRate = RateConfiguration.QuestDropRate;
            FamilyExpRate = RateConfiguration.FamilyXpRate;
            MaxGold = RateConfiguration.MaxGold;
            MaxBankGold = RateConfiguration.MaxBankGold;
            GoldDropRate = RateConfiguration.GoldDropRate;
            GoldRate = RateConfiguration.GoldRate;
            GlacernonPercentRatePvm = RateConfiguration.GlacernonPercentRatePvm;
            GlacernonPercentRatePvp = RateConfiguration.GlacernonPercentRatePvp;
            CylloanPercentRate = RateConfiguration.CylloanPercentRatePvm;

            /*
             * Min Max Configurations
             */
            MaxLevel = GameMinMaxConfiguration.MaxLevel;
            MaxMateLevel = GameMinMaxConfiguration.MaxMateLevel;
            MaxJobLevel = GameMinMaxConfiguration.MaxJobLevel;
            MaxSpLevel = GameMinMaxConfiguration.MaxSpLevel;
            MaxHeroLevel = GameMinMaxConfiguration.MaxHeroLevel;
            HeroicStartLevel = GameMinMaxConfiguration.HeroMinLevel;
            LobbySpeed = GameMinMaxConfiguration.LobbySpeed;
            MinLodLevel = GameMinMaxConfiguration.MinLodLevel;
            MaximumHomes = GameMinMaxConfiguration.MaximumHomes;

            /*
             * Events
             */
            ReputOnMonsters = RateConfiguration.ReputOnMonster;
            RaidPortalFromAnywhere = GameTrueFalseConfiguration.RaidPortalFromAnywhere;
            LodTimes = GameTrueFalseConfiguration.LodTimes;
            AutoLoot = GameTrueFalseConfiguration.AutoLoot;
            Easter = GameTrueFalseConfiguration.Easter;
            Winter = GameTrueFalseConfiguration.Winter;
            Estival = GameTrueFalseConfiguration.Estival;
            Halloween = GameTrueFalseConfiguration.Halloween;
            Valentine = GameTrueFalseConfiguration.Valentine;
            TimeBeforeAutoKick = GameScheduledEventsConfiguration.TimeBeforeAutoKick;
            AutoKickInterval = GameScheduledEventsConfiguration.TimeAutoKickInterval;
            MaxCodeAttempts = GameMinMaxConfiguration.MaxBotCodeAttempts;
            AntiBotEnabled = GameTrueFalseConfiguration.AntiBot;
            Maintenance = GameTrueFalseConfiguration.Maintenance;
            MessageOfTheDay = GameTrueFalseConfiguration.MessageOfTheDay;
            Schedules = GameScheduledEventsConfiguration.ScheduledEvents;
            Act4RaidStart = DateTime.Now;
            Act4AngelStat = new PercentBar();
            Act4DemonStat = new PercentBar();
            Act6Erenia = new PercentBar();
            Act6Zenas = new PercentBar();
        }

        public int JobXpRate { get; set; }

        public void LoadItems()
        {
            Stopwatch sw = Stopwatch.StartNew();
            IEnumerable<ItemDTO> items = DaoFactory.Instance.ItemDao.LoadAll();
            Dictionary<short?, BCardDTO[]> bcards = DaoFactory.Instance.BCardDao.LoadAll().Where(s => s.ItemVNum.HasValue).GroupBy(s => s.ItemVNum).ToDictionary(s => s.Key, s => s.ToArray());
            Dictionary<short, RollGeneratedItemDTO[]> rollItems = DaoFactory.Instance.RollGeneratedItemDao.LoadAll().GroupBy(s => s.OriginalItemVNum).ToDictionary(s => s.Key, s => s.ToArray());
            Dictionary<short, Item> item = new Dictionary<short, Item>();
            foreach (ItemDTO itemDto in items)
            {
                Item newItem;
                switch (itemDto.ItemType)
                {
                    case ItemType.Ammo:
                        newItem = new NoFunctionItem(itemDto);
                        break;

                    case ItemType.Armor:
                        newItem = new WearableItem(itemDto);
                        break;

                    case ItemType.Box:
                        newItem = new BoxItem(itemDto);
                        break;

                    case ItemType.Event:
                        newItem = new MagicalItem(itemDto);
                        break;

                    case ItemType.Fashion:
                        newItem = new WearableItem(itemDto);
                        break;

                    case ItemType.Food:
                        newItem = new FoodItem(itemDto);
                        break;

                    case ItemType.Jewelery:
                        newItem = new WearableItem(itemDto);
                        break;

                    case ItemType.Magical:
                        newItem = new MagicalItem(itemDto);
                        break;


                    case ItemType.Potion:
                        newItem = new PotionItem(itemDto);
                        break;

                    case ItemType.Production:
                        newItem = new ProduceItem(itemDto);
                        break;


                    case ItemType.Shell:
                        newItem = new MagicalItem(itemDto);
                        break;

                    case ItemType.Snack:
                        newItem = new SnackItem(itemDto);
                        break;

                    case ItemType.Special:
                        newItem = new SpecialItem(itemDto);
                        break;

                    case ItemType.Specialist:
                        newItem = new WearableItem(itemDto);
                        break;

                    case ItemType.Teacher:
                        newItem = new TeacherItem(itemDto);
                        break;

                    case ItemType.Upgrade:
                        newItem = new UpgradeItem(itemDto);
                        break;

                    case ItemType.Weapon:
                        newItem = new WearableItem(itemDto);
                        break;

                    case ItemType.Main:
                    case ItemType.Map:
                    case ItemType.Part:
                    case ItemType.Quest1:
                    case ItemType.Quest2:
                    case ItemType.Sell:
                    default:
                        newItem = new NoFunctionItem(itemDto);
                        break;
                }

                if (bcards.TryGetValue(newItem.VNum, out BCardDTO[] bcardDtos))
                {
                    newItem.BCards.AddRange(bcardDtos.Cast<BCard>());
                }
                if(rollItems.TryGetValue(newItem.VNum, out RollGeneratedItemDTO[] rolls))
                {
                    newItem.RollGeneratedItems.AddRange(rolls);
                }

                item[itemDto.VNum] = newItem;
            }

            Items.AddRange(item.Values);
            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("ITEMS_LOADED"), Items.Count));
        }

        public void Initialize(GameRateConfiguration rateConf, GameMinMaxConfiguration levelConf, GameTrueFalseConfiguration eventsConf, GameScheduledEventsConfiguration gameScheduledConf)
        {
            RateConfiguration = rateConf;
            GameMinMaxConfiguration = levelConf;
            GameTrueFalseConfiguration = eventsConf;
            GameScheduledEventsConfiguration = gameScheduledConf;
            /*
             * Rates
             */
            InitializeConfigurations();
            CharacterHomes = DaoFactory.Instance.CharacterHomeDao.LoadAll();

            CommunicationServiceClient.Instance.SetMaintenanceState(Maintenance);
            LoadItems();
            LoadMonsterDrops();
            LoadMonsterSkills();
            LoadBazaar();
            LoadNpcMonsters();
            LoadRecipes();
            LoadShopItems();
            LoadShopSkills();
            LoadShops();
            LoadTeleporters();
            LoadSkills();
            LoadCards();
            LoadQuests();
            LoadMapNpcs();
            LoadMapsAndContent();
            LoadFamilies();
            LaunchEvents();
            LoadAct4ShipMaps();
            RefreshRanking();
            CharacterRelations = DaoFactory.Instance.CharacterRelationDao.LoadAll().ToList();
            PenaltyLogs = DaoFactory.Instance.PenaltyLogDao.LoadAll().ToList();

            if (DaoFactory.Instance.MapDao.LoadById((short)SpecialMapIdType.Lobby) != null)
            {
                Logger.Log.Info("[LOBBY] Lobby Map Loaded");
                LobbyMapInstance = GenerateMapInstance((short)SpecialMapIdType.Lobby,
                    MapInstanceType.LobbyMapInstance, new InstanceBag());
            }

            LoadArenaMap();
            LoadAct4Maps();
            LoadAct4();
            LoadScriptedInstances();

            //Register the new created TCPIP server to the api
            WorldId = Guid.NewGuid();
        }

        private void LoadAct4Maps()
        {
            if (DaoFactory.Instance.MapDao.LoadById(154) != null)
            {
                CaligorMapInstance = GenerateMapInstance(154, MapInstanceType.CaligorInstance, new InstanceBag());
                CaligorMapInstance.IsPvp = true;
                Logger.Log.Info("[ACT4] Caligor Map Loaded");
            }

            if (Act4Maps == null)
            {
                Act4Maps = new List<MapInstance>();
            }

            foreach (Map m in Maps.Where(s => s.MapTypes.Any(o =>
                o.MapTypeId == (short)MapTypeEnum.Act4 || o.MapTypeId == (short)MapTypeEnum.Act42)))
            {
                MapInstance act4Map = GenerateMapInstance(m.MapId, MapInstanceType.Act4Instance, new InstanceBag());
                if (act4Map.Map.MapId == 153)
                {
                    act4Map.Portals.Clear();
                    // ANGEL
                    act4Map.Portals.Add(new Portal
                    {
                        DestinationMapId = 134,
                        DestinationX = 140,
                        DestinationY = 4,
                        SourceX = 46,
                        SourceY = 171,
                        SourceMapId = 153,
                        IsDisabled = false,
                        Type = (short)PortalType.MapPortal
                    });
                    // DEMON
                    act4Map.Portals.Add(new Portal
                    {
                        DestinationMapId = 134,
                        DestinationX = 140,
                        DestinationY = 4,
                        SourceX = 140,
                        SourceY = 171,
                        SourceMapId = 153,
                        IsDisabled = false,
                        Type = (short)PortalType.MapPortal
                    });
                }

                // TODO REMOVE THAT FOR RELEASE
                if (act4Map.Map.MapId == 134)
                {
                    Portal portal = act4Map.Portals.Find(s => s.DestinationMapId == 153);
                    if (portal != null)
                    {
                        portal.SourceX = 140;
                        portal.SourceY = 11;
                    }
                }

                act4Map.IsPvp = true;
                Act4Maps.Add(act4Map);
            }
        }

        private void LoadAct4()
        {
            foreach (MapInstance m in Act4Maps)
            {
                foreach (Portal portal in m.Portals)
                {
                    MapInstance mapInstance = Act4Maps.Find(s => s.Map.MapId == portal.DestinationMapId);
                    if (mapInstance != null)
                    {
                        portal.DestinationMapInstanceId = mapInstance.MapInstanceId;
                    }
                    else
                    {
                        m.Portals.RemoveAll(s => s.DestinationMapId == portal.DestinationMapId);
                        Logger.Log.Warn($"Could not find Act4Map with Id {portal.DestinationMapId}");
                    }
                }
            }

            Act4Maps.Add(CaligorMapInstance);
            Logger.Log.Info("[ACT4] Initialized");
        }

        private void LoadAct4ShipMaps()
        {
            if (DaoFactory.Instance.MapDao.LoadById(149) != null)
            {
                Logger.Log.Info("[ACT4] Demon Ship Loaded");
                Act4ShipDemon = GenerateMapInstance(149, MapInstanceType.ArenaInstance, new InstanceBag());
                Logger.Log.Info("[ACT4] Angel Ship Loaded");
                Act4ShipAngel = GenerateMapInstance(149, MapInstanceType.NormalInstance, new InstanceBag());
            }
        }

        private void LoadArenaMap()
        {
            if (DaoFactory.Instance.MapDao.LoadById(2006) != null)
            {
                Logger.Log.Info("[ARENA] Arena Map Loaded");
                ArenaInstance = GenerateMapInstance(2006, MapInstanceType.ArenaInstance, new InstanceBag());
                ArenaInstance.IsPvp = true;
                ArenaInstance.Portals.Add(new Portal
                {
                    DestinationMapId = 1,
                    DestinationX = 1,
                    DestinationY = 1,
                    SourceMapId = 2006,
                    SourceX = 37,
                    SourceY = 15
                });
            }

            if (DaoFactory.Instance.MapDao.LoadById(2106) != null)
            {
                Logger.Log.Info("[ARENA] Family Arena Map Loaded");
                FamilyArenaInstance = GenerateMapInstance(2106, MapInstanceType.ArenaInstance, new InstanceBag());
                FamilyArenaInstance.IsPvp = true;
                FamilyArenaInstance.Portals.Add(new Portal
                {
                    DestinationMapId = 1,
                    DestinationX = 1,
                    DestinationY = 1,
                    SourceMapId = 2106,
                    SourceX = 38,
                    SourceY = 3
                });
            }
        }

        private static void LoadMapsAndContent()
        {
            try
            {
                int i = 0;
                int monstercount = 0;

                var monsters = DaoFactory.Instance.MapMonsterDao.LoadAll().GroupBy(s => s.MapId).ToDictionary(s => s.Key, s => s.ToArray());
                var npcs = DaoFactory.Instance.MapNpcDao.LoadAll().GroupBy(s => s.MapId).ToDictionary(s => s.Key, s => s.ToArray());
                var portals = DaoFactory.Instance.PortalDao.LoadAll().GroupBy(s => s.SourceMapId).ToDictionary(s => s.Key, s => s.ToArray());
                MapTypeMapDTO[] mapTypes = DaoFactory.Instance.MapTypeMapDao.LoadAll().ToArray();
                MapTypeDTO[] mapTypeMap = DaoFactory.Instance.MapTypeDao.LoadAll().ToArray();
                IEnumerable<RespawnMapTypeDTO> respawns = DaoFactory.Instance.RespawnMapTypeDao.LoadAll();

                foreach (MapDTO map in DaoFactory.Instance.MapDao.LoadAll().ToArray())
                {
                    Guid guid = Guid.NewGuid();
                    IEnumerable<MapTypeMapDTO> mapType = mapTypes.Where(s => map.MapId == s.MapId);
                    var mapObject = new Map(map.MapId, map.Data,
                        mapTypeMap.Where(s => mapType.Any(p => p.MapTypeId == s.MapTypeId)), respawns)
                    {
                        Music = map.Music
                    };
                    var newMap = new MapInstance(mapObject, guid, map.ShopAllowed, MapInstanceType.BaseMapInstance, new InstanceBag());
                    _mapInstances.TryAdd(guid, newMap);

                    if (portals.TryGetValue(map.MapId, out PortalDTO[] port))
                    {
                        newMap.LoadPortals(port);
                    }

                    if (npcs.TryGetValue(map.MapId, out MapNpcDTO[] np))
                    {
                        newMap.LoadNpcs(np);
                    }

                    if (monsters.TryGetValue(map.MapId, out MapMonsterDTO[] monst))
                    {
                        newMap.LoadMonsters(monst);
                    }

                    foreach (MapNpc mapNpc in newMap.Npcs)
                    {
                        mapNpc.MapInstance = newMap;
                        newMap.AddNpc(mapNpc);
                    }

                    foreach (MapMonster mapMonster in newMap.Monsters)
                    {
                        mapMonster.MapInstance = newMap;
                        newMap.AddMonster(mapMonster);
                    }


                    monstercount += newMap.Monsters.Count;
                    Maps.Add(mapObject);
                    i++;
                }

                Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("MAPS_LOADED"), i));
                Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("MAPMONSTERS_LOADED"), monstercount));
            }
            catch (Exception e)
            {
                Logger.Log.Error("General Error", e);
            }
        }

        private void LoadMapNpcs()
        {
            _mapNpcs = new Dictionary<short, List<MapNpc>>();
            foreach (IGrouping<short, MapNpcDTO> mapNpcGrouping in DaoFactory.Instance.MapNpcDao.LoadAll().ToArray().GroupBy(t => t.MapId))
            {
                _mapNpcs[mapNpcGrouping.Key] = mapNpcGrouping.Select(t => t as MapNpc).ToList();
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("MAPNPCS_LOADED"),
                _mapNpcs.Sum(i => i.Value.Count)));
        }

        private void LoadQuests()
        {
            // initialize quests
            Quests = new List<Quest>();
            var questRewards = DaoFactory.Instance.QuestRewardDao.LoadAll();
            var questObjectives = DaoFactory.Instance.QuestObjectiveDao.LoadAll();
            foreach (QuestDTO questdto in DaoFactory.Instance.QuestDao.LoadAll().ToArray())
            {
                var quest = (Quest)questdto;
                quest.QuestRewards = questRewards.Where(s => s.QuestId == quest.QuestId).ToList();
                quest.QuestObjectives = questObjectives.Where(s => s.QuestId == quest.QuestId).ToList();
                Quests.Add(quest);
            }

            FlowerQuestId = Quests.Find(q => q.QuestType == (byte)QuestType.FlowerQuest)?.QuestId;

            if (Easter)
            {
                CalvinQuest = Quests.Find(q => q.QuestId == 5950)?.QuestId;
                MimiQuest = Quests.Find(q => q.QuestId == 5946)?.QuestId;
                SluggQuest = Quests.Find(q => q.QuestId == 5948)?.QuestId;
                EvaQuest = Quests.Find(q => q.QuestId == 5953)?.QuestId;
                MalcolmQuest = Quests.Find(q => q.QuestId == 5945)?.QuestId;
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("QUESTS_LOADED"), Quests.Count));
        }

        private static void LoadSkills()
        {
            IEnumerable<ComboDTO> combos = DaoFactory.Instance.ComboDao.LoadAll().ToArray()
                ;
            IEnumerable<BCardDTO> bcards = DaoFactory.Instance.BCardDao.LoadAll().ToArray().Where(s => s.SkillVNum.HasValue);
            foreach (SkillDTO skillItem in DaoFactory.Instance.SkillDao.LoadAll().ToArray())
            {
                if (!(skillItem is Skill skillObj))
                {
                    return;
                }

                skillObj.Combos.AddRange(combos.Where(s => s.SkillVNum == skillObj.SkillVNum).ToList());
                skillObj.BCards = new ConcurrentBag<BCard>();

                foreach (BCardDTO o in bcards.Where(s => s.SkillVNum == skillObj.SkillVNum))
                {
                    skillObj.BCards.Add((BCard)o);
                }

                Skills.Add(skillObj);
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("SKILLS_LOADED"), Skills.Count));
        }

        private void LoadCards()
        {
            Cards = new List<Card>();

            IEnumerable<BCardDTO> bcards = DaoFactory.Instance.BCardDao.LoadAll().ToArray().Where(s => s.CardId.HasValue);
            IEnumerable<Card> cards = DaoFactory.Instance.CardDao.LoadAll().Cast<Card>();
            foreach (Card card in cards)
            {
                card.BCards = new List<BCard>();


                card.BCards.AddRange(bcards.Where(s => s.CardId == card.CardId).Cast<BCard>());
                Cards.Add(card);
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("CARDS_LOADED"), Cards.Count));
        }

        private void LoadTeleporters()
        {
            _teleporters = new Dictionary<int, List<TeleporterDTO>>();
            foreach (IGrouping<int, TeleporterDTO> teleporterGrouping in DaoFactory.Instance.TeleporterDao.LoadAll().GroupBy(t => t.MapNpcId))
            {
                _teleporters[teleporterGrouping.Key] = teleporterGrouping.Select(t => t).ToList();
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("TELEPORTERS_LOADED"),
                _teleporters.Sum(i => i.Value.Count)));
        }

        private void LoadShops()
        {
            _shops = new Dictionary<int, Shop>();
            foreach (ShopDTO shopGrouping in DaoFactory.Instance.ShopDao.LoadAll())
            {
                _shops[shopGrouping.MapNpcId] = (Shop)shopGrouping;
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("SHOPS_LOADED"), _shops.Count));
        }

        private void LoadShopSkills()
        {
            _shopSkills = new Dictionary<int, List<ShopSkillDTO>>();
            foreach (IGrouping<int, ShopSkillDTO> shopSkillGrouping in DaoFactory.Instance.ShopSkillDao.LoadAll().GroupBy(s => s.ShopId))
            {
                _shopSkills[shopSkillGrouping.Key] = shopSkillGrouping.ToList();
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("SHOPSKILLS_LOADED"),
                _shopSkills.Sum(i => i.Value.Count)));
        }

        private void LoadShopItems()
        {
            _shopItems = new Dictionary<int, List<ShopItemDTO>>();
            foreach (IGrouping<int, ShopItemDTO> shopItemGrouping in DaoFactory.Instance.ShopItemDao.LoadAll().GroupBy(s => s.ShopId))
            {
                _shopItems[shopItemGrouping.Key] = shopItemGrouping.ToList();
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("SHOPITEMS_LOADED"),
                _shopItems.Sum(i => i.Value.Count)));
        }

        private void LoadRecipes()
        {
            _recipes = new Dictionary<int, List<Recipe>>();
            foreach (IGrouping<int, RecipeDTO> recipeGrouping in DaoFactory.Instance.RecipeDao.LoadAll().GroupBy(r => r.MapNpcId))
            {
                _recipes[recipeGrouping.Key] = recipeGrouping.Select(r => r as Recipe).ToList();
            }


            _recipeLists = new List<Recipe>();
            foreach (RecipeDTO recipe in DaoFactory.Instance.RecipeDao.LoadAll())
            {
                _recipeLists.Add((Recipe)recipe);
            }

            foreach (IGrouping<short, RecipeItemDTO> recipeItems in DaoFactory.Instance.RecipeItemDao.LoadAll().GroupBy(s => s.RecipeId).ToList())
            {
                _recipeLists.Find(s => s.RecipeId == recipeItems.Key).Items = recipeItems.ToList();
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("RECIPES_LOADED"),
                _recipes.Sum(i => i.Value.Count)));
        }

        private static void LoadNpcMonsters()
        {
            IEnumerable<BCardDTO> bcards = DaoFactory.Instance.BCardDao.LoadAll().ToArray().Where(s => s.NpcMonsterVNum.HasValue);
            foreach (NpcMonsterDTO npcMonster in DaoFactory.Instance.NpcMonsterDao.LoadAll().ToArray())
            {
                if (!(npcMonster is NpcMonster monster))
                {
                    continue;
                }

                monster.BCards = new List<BCard>();

                foreach (BCardDTO s in bcards.Where(s => s.NpcMonsterVNum == monster.NpcMonsterVNum))
                {
                    monster.BCards.Add((BCard)s);
                }

                Npcs.Add(monster);
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("NPCMONSTERS_LOADED"), Npcs.Count));
        }

        private void LoadMonsterSkills()
        {
            // initialize monsterskills
            _monsterSkills = new Dictionary<short, List<NpcMonsterSkill>>();
            foreach (IGrouping<short, NpcMonsterSkillDTO> monsterSkillGrouping in DaoFactory.Instance.NpcMonsterSkillDao.LoadAll().ToArray().GroupBy(n => n.NpcMonsterVNum))
            {
                _monsterSkills[monsterSkillGrouping.Key] =
                    monsterSkillGrouping.Select(n => n as NpcMonsterSkill).ToList();
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("MONSTERSKILLS_LOADED"),
                _monsterSkills.Sum(i => i.Value.Count)));
        }

        private void LoadMonsterDrops()
        {
            // intialize monsterdrops
            _monsterDrops = new Dictionary<short, List<DropDTO>>();
            foreach (IGrouping<short?, DropDTO> monsterDropGrouping in DaoFactory.Instance.DropDao.LoadAll().ToArray().GroupBy(d => d.MonsterVNum))
            {
                if (monsterDropGrouping.Key.HasValue)
                {
                    _monsterDrops[monsterDropGrouping.Key.Value] =
                        monsterDropGrouping.OrderBy(d => d.DropChance).ToList();
                }
                else
                {
                    _generalDrops = monsterDropGrouping.ToList();
                }
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("DROPS_LOADED"),
                _monsterDrops.Sum(i => i.Value.Count)));
        }

        public bool Maintenance { get; set; }
        public bool MessageOfTheDay { get; set; }

        public bool IsCharacterMemberOfGroup(long characterId)
        {
            return Groups?.Any(g => g.IsMemberOfGroup(characterId)) == true;
        }

        public bool IsCharactersGroupFull(long characterId)
        {
            return Groups?.Any(g => g.IsMemberOfGroup(characterId) && g.CharacterCount == (byte)g.GroupType) == true;
        }

        public void JoinMiniland(ClientSession session, ClientSession minilandOwner)
        {
            ChangeMapInstance(session.Character.CharacterId, minilandOwner.Character.Miniland.MapInstanceId, 5, 8);
            if (session.Character.Miniland.MapInstanceId != minilandOwner.Character.Miniland.MapInstanceId)
            {
                session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(session.Character.MinilandMessage.Replace(' ', '^'), 0));
                session.SendPacket(session.Character.GenerateMlinfobr());
                minilandOwner.Character.GeneralLogs.Add(new GeneralLogDTO
                {
                    AccountId = session.Account.AccountId,
                    CharacterId = session.Character.CharacterId,
                    IpAddress = session.IpAddress,
                    LogData = "Miniland",
                    LogType = "World",
                    Timestamp = DateTime.Now
                });
            }
            else
            {
                session.SendPacket(session.Character.GenerateMlinfo());
            }

            minilandOwner.Character.Mates.Where(s => !s.IsTeamMember).ToList()
                .ForEach(s => session.SendPacket(s.GenerateIn()));
            session.SendPacket(session.Character.GenerateSay(
                string.Format(Language.Instance.GetMessageFromKey("MINILAND_VISITOR"),
                    session.Character.GeneralLogs.Count(s =>
                        s.LogData == "Miniland" && s.Timestamp.Day == DateTime.Now.Day),
                    session.Character.GeneralLogs.Count(s => s.LogData == "Miniland")), 10));
        }

        // Server
        public void Kick(string characterName)
        {
            ClientSession session =
                Sessions.FirstOrDefault(s => s.Character?.Name.Equals(characterName) == true);
            session?.Disconnect();
        }

        // Map
        public void LeaveMap(long id)
        {
            ClientSession session = GetSessionByCharacterId(id);
            if (session == null)
            {
                return;
            }

            session.SendPacket(UserInterfaceHelper.Instance.GenerateMapOut());
            session.Character.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                session.CurrentMapInstance?.Broadcast(session, s.GenerateOut(), ReceiverType.AllExceptMe));
            session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateOut(), ReceiverType.AllExceptMe);
        }


        public int RandomNumber(int min = 0, int max = 100) => Random.Value.Next(min, max);

        public MapCell MinilandRandomPos() => new MapCell { X = (short)RandomNumber(5, 16), Y = (short)RandomNumber(3, 14) };

        public void RefreshRanking()
        {
            TopComplimented = DaoFactory.Instance.CharacterDao.GetTopCompliment();
            TopPoints = DaoFactory.Instance.CharacterDao.GetTopPoints();
            TopReputation = DaoFactory.Instance.CharacterDao.GetTopReputation();
        }

        public void RelationRefresh(long relationId)
        {
            _inRelationRefreshMode = true;
            CommunicationServiceClient.Instance.UpdateRelation(ServerGroup, relationId);
            SpinWait.SpinUntil(() => !_inRelationRefreshMode);
        }

        public void RemoveMapInstance(Guid mapId)
        {
            KeyValuePair<Guid, MapInstance> map = _mapInstances.FirstOrDefault(s => s.Key == mapId);
            if (map.Equals(default(KeyValuePair<Guid, MapInstance>)))
            {
                return;
            }

            map.Value.Dispose();
            ((IDictionary)_mapInstances).Remove(map.Key);
        }

        // Map
        public void ReviveFirstPosition(long characterId)
        {
            ClientSession session = GetSessionByCharacterId(characterId);
            if (session == null || session.Character.Hp > 0)
            {
                return;
            }

            short x;
            short y;
            switch (session.CurrentMapInstance.MapInstanceType)
            {
                case MapInstanceType.TimeSpaceInstance:
                case MapInstanceType.RaidInstance:
                    session.Character.Hp = (int)session.Character.HpLoad();
                    session.Character.Mp = (int)session.Character.MpLoad();
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateRevive());
                    session.SendPacket(session.Character.GenerateStat());
                    break;
                case MapInstanceType.Act4Instance:
                    x = (short)(39 + Instance.RandomNumber(-2, 3));
                    y = (short)(42 + Instance.RandomNumber(-2, 3));
                    MapInstance citadel = Instance.Act4Maps.Find(s =>
                        s.Map.MapId == (session.Character.Faction == FactionType.Angel ? 130 : 131));
                    if (citadel != null)
                    {
                        Instance.ChangeMapInstance(session.Character.CharacterId, citadel.MapInstanceId, x, y);
                    }

                    session.Character.Hp = 1;
                    session.Character.Mp = 1;
                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateTp());
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                    session.SendPacket(session.Character.GenerateStat());
                    break;
                default:
                    session.Character.Hp = 1;
                    session.Character.Mp = 1;
                    if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.BaseMapInstance)
                    {
                        RespawnMapTypeDTO resp = session.Character.Respawn;
                        x = (short)(resp.DefaultX + RandomNumber(-3, 3));
                        y = (short)(resp.DefaultY + RandomNumber(-3, 3));
                        ChangeMap(session.Character.CharacterId, resp.DefaultMapId, x, y);
                    }
                    else
                    {
                        Instance.ChangeMap(session.Character.CharacterId, session.Character.MapId,
                            session.Character.MapX, session.Character.MapY);
                    }

                    session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateTp());
                    session.CurrentMapInstance?.Broadcast(session.Character.GenerateRevive());
                    session.SendPacket(session.Character.GenerateStat());
                    break;
            }
        }

        public void SaveAll()
        {
            if (!Sessions.Any())
            {
                return;
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (ClientSession session in Sessions.Where(s =>
                s?.HasCurrentMapInstance == true && s.HasSelectedCharacter && s.Character != null))
            {
                session.Character.Save();
            }

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Logger.Log.Info("[SaveAll] SaveAll need " + elapsedTime + " to Save " + Sessions.Count() + " Players");
        }

        public void SetProperty(long charId, string property, object value)
        {
            ClientSession session = GetSessionByCharacterId(charId);
            if (session == null)
            {
                return;
            }

            PropertyInfo propertyinfo = session.Character.GetType().GetProperty(property);
            propertyinfo?.SetValue(session.Character, value, null);
        }

        public void Shout(string message)
        {
            Broadcast($"say 1 0 10 ({Language.Instance.GetMessageFromKey("ADMINISTRATOR")}){message}");
            Broadcast($"msg 2 {message}");
        }

        public void Shutdown()
        {
            CommunicationServiceClient.Instance.SetWorldServerAsInvisible(WorldId);
            string message = string.Format(Language.Instance.GetMessageFromKey("SHUTDOWN_SEC"), 15);
            Instance.Broadcast($"say 1 0 10 ({Language.Instance.GetMessageFromKey("ADMINISTRATOR")}){message}");
            Instance.Broadcast(UserInterfaceHelper.Instance.GenerateMsg(message, 2));

            Observable.Timer(TimeSpan.FromSeconds(15)).Subscribe(c =>
            {
                Instance.SaveAll();
                Instance.DisconnectAll();
                CommunicationServiceClient.Instance.UnregisterWorldServer(WorldId);
            });
        }

        public void TeleportToLobby(ClientSession session)
        {
            if (session?.Character == null)
            {
                return;
            }

            ChangeMapInstance(session.Character.CharacterId, LobbyMapInstance.MapInstanceId, RandomNumber(141, 147),
                RandomNumber(87, 94));
            session.CurrentMapInstance?.Broadcast(session.Character.GenerateEff(23));
        }

        public void TeleportForward(ClientSession session, Guid guid, short x, short y)
        {
            MapInstance map = GetMapInstance(guid);
            if (guid == default)
            {
                return;
            }

            bool pos = map.Map.GetDefinedPosition(x, y);
            if (!pos)
            {
                return;
            }

            session.Character.TeleportOnMap(x, y);
        }

        public void TeleportOnRandomPlaceInMap(ClientSession session, Guid guid, bool isSameMap = false)
        {
            MapInstance map = GetMapInstance(guid);
            if (guid == default)
            {
                return;
            }

            MapCell pos = map.Map.GetRandomPosition();
            if (pos == null)
            {
                return;
            }

            switch (isSameMap)
            {
                case false:
                    ChangeMapInstance(session.Character.CharacterId, guid, pos.X, pos.Y);
                    break;
                case true:
                    session.Character.TeleportOnMap(pos.X, pos.Y);
                    break;
            }
        }

        // Server
        public void UpdateGroup(long charId)
        {
            try
            {
                Group myGroup = Groups?.FirstOrDefault(s => s.IsMemberOfGroup(charId));
                if (myGroup == null)
                {
                    return;
                }

                ConcurrentBag<ClientSession> groupMembers =
                    Groups.Find(s => s.IsMemberOfGroup(charId))?.Characters;
                if (groupMembers == null)
                {
                    return;
                }

                foreach (ClientSession session in groupMembers)
                {
                    session.SendPacket(session.Character.GeneratePinit());
                    session.SendPackets(session.Character.Mates.Where(s => s.IsTeamMember)
                        .OrderBy(s => s.MateType)
                        .Select(s => s.GeneratePst()));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal List<NpcMonsterSkill> GetNpcMonsterSkillsByMonsterVNum(short npcMonsterVNum) => _monsterSkills.ContainsKey(npcMonsterVNum)
            ? _monsterSkills[npcMonsterVNum]
            : new List<NpcMonsterSkill>();

        internal Shop GetShopByMapNpcId(int mapNpcId) => _shops.ContainsKey(mapNpcId) ? _shops[mapNpcId] : null;

        internal List<ShopItemDTO> GetShopItemsByShopId(int shopId) => _shopItems.ContainsKey(shopId) ? _shopItems[shopId] : new List<ShopItemDTO>();

        internal List<ShopSkillDTO> GetShopSkillsByShopId(int shopId) => _shopSkills.ContainsKey(shopId) ? _shopSkills[shopId] : new List<ShopSkillDTO>();

        internal List<TeleporterDTO> GetTeleportersByNpcVNum(short npcMonsterVNum)
        {
            if (_teleporters?.ContainsKey(npcMonsterVNum) == true)
            {
                return _teleporters[npcMonsterVNum];
            }

            return new List<TeleporterDTO>();
        }

        internal void StopServer()
        {
            Instance.Shutdown();
        }

        // Server
        private void BotProcess()
        {
            try
            {
                Shout(Language.Instance.GetMessageFromKey($"BOT_MESSAGE_{RandomNumber(0, 5)}"));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void GroupProcess()
        {
            try
            {
                if (Groups != null)
                {
                    Parallel.ForEach(Groups, grp =>
                    {
                        foreach (ClientSession session in grp.Characters)
                        {
                            foreach (string str in grp.GeneratePst(session))
                            {
                                session.SendPacket(str);
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void LaunchEvents()
        {
            StartedEvents = new List<EventType>();
            _groups = new ConcurrentDictionary<long, Group>();

            Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(x => { Act6Process(); });

            Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(x => { Act4Process(); });

            Observable.Interval(TimeSpan.FromMinutes(5)).Subscribe(x => { DaoFactory.Instance.BazaarItemDao.RemoveOutDated(); });

            Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(x => { GroupProcess(); });

            Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(x => { Act4FlowerProcess(); });

            Observable.Interval(TimeSpan.FromHours(3)).Subscribe(x => { BotProcess(); });

            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(x => { RemoveItemProcess(); });

            //Observable.Interval(TimeSpan.FromMinutes(5)).Subscribe(x => { SaveAll(); });

            foreach (Schedule schedule in Schedules)
            {
                Observable
                    .Timer(
                        TimeSpan.FromSeconds(EventHelper.Instance.GetMilisecondsBeforeTime(schedule.Time).TotalSeconds),
                        TimeSpan.FromDays(1)).Subscribe(e => { EventHelper.Instance.GenerateEvent(schedule.Event); });
            }

            EventHelper.Instance.GenerateEvent(EventType.ACT4SHIP);

            CommunicationServiceEvents.Instance.SessionKickedEvent += OnSessionKicked;
            CommunicationServiceEvents.Instance.MessageSentToCharacter += OnMessageSentToCharacter;
            CommunicationServiceEvents.Instance.MailSent += OnMailSent;
            CommunicationServiceEvents.Instance.AuthorityChange += OnAuthorityChange;
            CommunicationServiceEvents.Instance.FamilyRefresh += OnFamilyRefresh;
            CommunicationServiceEvents.Instance.RelationRefresh += OnRelationRefresh;
            CommunicationServiceEvents.Instance.BazaarRefresh += OnBazaarRefresh;
            CommunicationServiceEvents.Instance.PenaltyLogRefresh += OnPenaltyLogRefresh;
            CommunicationServiceEvents.Instance.ShutdownEvent += OnShutdown;
            _lastGroupId = 1;
        }

        private void Act4FlowerProcess()
        {
            // FIND THE REAL VALUES
            foreach (MapInstance map in Act4Maps.Where(s =>
                s.Npcs.Count(o => o.NpcVNum == 2004 && o.IsOut) < s.Npcs.Count(n => n.NpcVNum == 2004)))
            {
                // TODO PROPERTY
                foreach (MapNpc i in map.Npcs.Where(s => s.IsOut && s.NpcVNum == 2004))
                {
                    MapCell randomPos = map.Map.GetRandomPosition();
                    i.MapX = randomPos.X;
                    i.MapY = randomPos.Y;
                    i.MapInstance.Broadcast(i.GenerateIn());
                }
            }
        }

        public void Act4Process()
        {
            MapInstance angelMapInstance = Act4Maps.Find(s => s.Map.MapId == 132);
            MapInstance demonMapInstance = Act4Maps.Find(s => s.Map.MapId == 133);

            if (angelMapInstance == null || demonMapInstance == null)
            {
                return;
            }

            void SummonMukraju(MapInstance instance, byte faction)
            {
                var monster = new MapMonster
                {
                    MonsterVNum = 556,
                    MapY = faction == 1 ? (short)92 : (short)95,
                    MapX = faction == 1 ? (short)114 : (short)20,
                    MapId = (short)(131 + faction),
                    IsMoving = true,
                    MapMonsterId = instance.GetNextId(),
                    ShouldRespawn = false
                };
                monster.Initialize(instance);
                monster.BattleEntity.OnDeathEvents.Add(new EventContainer(instance, EventActionType.STARTACT4RAID,
                    new Tuple<byte, byte>((byte)RandomNumber(0, 4), faction)));
                instance.AddMonster(monster);
                instance.Broadcast(monster.GenerateIn());
                Observable.Timer(TimeSpan.FromSeconds(300)).Subscribe(o =>
                {
                    instance.RemoveMonster(monster);
                    instance.Broadcast(monster.GenerateOut());
                });
            }

            if (Act4AngelStat.Percentage > 10000)
            {
                Act4AngelStat.Mode = 1;
                Act4AngelStat.Percentage = 0;
                Act4AngelStat.TotalTime = 300;
                SummonMukraju(angelMapInstance, 1);
            }

            if (Act4DemonStat.Percentage > 10000)
            {
                Act4DemonStat.Mode = 1;
                Act4DemonStat.Percentage = 0;
                Act4DemonStat.TotalTime = 300;
                SummonMukraju(demonMapInstance, 2);
            }

            if (Act4AngelStat.CurrentTime <= 0 && Act4AngelStat.Mode != 0)
            {
                Act4AngelStat.Mode = 0;
                Act4AngelStat.TotalTime = 0;
            }
            else if (Act4DemonStat.CurrentTime <= 0 && Act4DemonStat.Mode != 0)
            {
                Act4DemonStat.Mode = 0;
                Act4DemonStat.TotalTime = 0;
            }

            Parallel.ForEach(
                Sessions.Where(s =>
                    s?.Character != null && s.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Instance),
                sess => sess.SendPacket(sess.Character.GenerateFc()));
        }

        public void Act6Process()
        {
            if (Act6Zenas.Percentage >= 1000 && Act6Zenas.Mode == 0)
            {
                Act6Raid.GenerateRaid(FactionType.Angel);
                Act6Zenas.TotalTime = 3600;
                Act6Zenas.Mode = 1;
            }
            else if (Act6Erenia.Percentage >= 1000 && Act6Erenia.Mode == 0)
            {
                Act6Raid.GenerateRaid(FactionType.Demon);
                Act6Erenia.TotalTime = 3600;
                Act6Erenia.Mode = 1;
            }

            if (Act6Erenia.CurrentTime <= 0 && Act6Erenia.Mode != 0)
            {
                Act6Erenia.KilledMonsters = 0;
                Act6Erenia.Percentage = 0;
                Act6Erenia.Mode = 0;
            }

            if (Act6Zenas.CurrentTime <= 0 && Act6Zenas.Mode != 0)
            {
                Act6Zenas.KilledMonsters = 0;
                Act6Zenas.Percentage = 0;
                Act6Zenas.Mode = 0;
            }

            Parallel.ForEach(
                Sessions.Where(s =>
                    s?.Character != null && s.CurrentMapInstance?.Map.MapId >= 228 &&
                    s.CurrentMapInstance?.Map.MapId < 238 || s?.CurrentMapInstance?.Map.MapId == 2604),
                sess => sess.SendPacket(sess.Character.GenerateAct6()));
        }

        private void LoadBazaar()
        {
            BazaarList = new List<BazaarItemLink>();
            foreach (BazaarItemDTO bz in DaoFactory.Instance.BazaarItemDao.LoadAll().ToArray())
            {
                var item = new BazaarItemLink
                {
                    BazaarItem = bz
                };
                CharacterDTO chara = DaoFactory.Instance.CharacterDao.LoadById(bz.SellerId);
                if (chara != null)
                {
                    item.Owner = chara.Name;
                    item.Item = (ItemInstance)DaoFactory.Instance.IteminstanceDao.LoadById(bz.ItemInstanceId);
                }

                BazaarList.Add(item);
            }

            Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("BAZAR_LOADED"), BazaarList.Count));
        }

        private void LoadFamilies()
        {
            FamilyList = new ConcurrentDictionary<long, Family>();
            foreach (FamilyDTO familyDto in DaoFactory.Instance.FamilyDao.LoadAll())
            {
                var family = new Family(familyDto)
                {
                    FamilyCharacters = new List<FamilyCharacter>()
                };
                foreach (FamilyCharacterDTO famchar in DaoFactory.Instance.FamilyCharacterDao.LoadByFamilyId(family.FamilyId).ToList())
                {
                    family.FamilyCharacters.Add((FamilyCharacter)famchar);
                }

                FamilyCharacter familyCharacter = family.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                if (familyCharacter != null)
                {
                    family.Warehouse = new Inventory((Character.Character)familyCharacter.Character);
                    foreach (ItemInstanceDTO inventory in DaoFactory.Instance.IteminstanceDao.LoadByCharacterId(familyCharacter.CharacterId).Where(s => s.Type == InventoryType.FamilyWareHouse)
                        .ToList())
                    {
                        inventory.CharacterId = familyCharacter.CharacterId;
                        family.Warehouse[inventory.Id] = (ItemInstance)inventory;
                    }
                }

                family.FamilyLogs = DaoFactory.Instance.FamilyLogDao.LoadByFamilyId(family.FamilyId).ToList();
                FamilyList[family.FamilyId] = family;
            }
        }

        private void LoadScriptedInstances()
        {
            Raids = new ConcurrentBag<ScriptedInstance>();
            TimeSpaces = new ConcurrentBag<ScriptedInstance>();
            Act4Raids = new ConcurrentBag<ScriptedInstance>();
            Act6Raids = new ConcurrentBag<ScriptedInstance>();
            Parallel.ForEach(_mapInstances, map =>
            {
                foreach (ScriptedInstanceDTO scriptedInstanceDto in DaoFactory.Instance.ScriptedInstanceDao.LoadByMap(map.Value.Map.MapId).ToList())
                {
                    var si = (ScriptedInstance)scriptedInstanceDto;
                    switch (si.Type)
                    {
                        case ScriptedInstanceType.TimeSpace:
                            si.LoadGlobals();
                            TimeSpaces.Add(si);
                            map.Value.ScriptedInstances.Add(si);
                            break;
                        case ScriptedInstanceType.Raid:
                            si.LoadGlobals();
                            Raids.Add(si);
                            var port = new Portal
                            {
                                Type = (byte)PortalType.Raid,
                                SourceMapId = si.MapId,
                                SourceX = si.PositionX,
                                SourceY = si.PositionY
                            };
                            map.Value.Portals.Add(port);
                            break;
                        case ScriptedInstanceType.RaidAct4:
                            si.LoadGlobals();
                            Act4Raids.Add(si);
                            break;
                        case ScriptedInstanceType.RaidAct6:
                            si.LoadGlobals();
                            Raids.Add(si);
                            Act6Raids.Add(si);
                            break;
                    }
                }
            });
        }


        private void OnBazaarRefresh(object sender, EventArgs e)
        {
            // TODO: Parallelization of bazaar.
            long bazaarId = (long)sender;
            BazaarItemDTO bzdto = DaoFactory.Instance.BazaarItemDao.LoadById(bazaarId);
            BazaarItemLink bzlink = BazaarList.Find(s => s.BazaarItem.BazaarItemId == bazaarId);
            lock(BazaarList)
            {
                if (bzdto != null)
                {
                    CharacterDTO chara = DaoFactory.Instance.CharacterDao.LoadById(bzdto.SellerId);
                    if (bzlink != null)
                    {
                        BazaarList.Remove(bzlink);
                        bzlink.BazaarItem = bzdto;
                        bzlink.Owner = chara.Name;
                        bzlink.Item = (ItemInstance)DaoFactory.Instance.IteminstanceDao.LoadById(bzdto.ItemInstanceId);
                        BazaarList.Add(bzlink);
                    }
                    else
                    {
                        var item = new BazaarItemLink
                        {
                            BazaarItem = bzdto
                        };
                        if (chara != null)
                        {
                            item.Owner = chara.Name;
                            item.Item = (ItemInstance)DaoFactory.Instance.IteminstanceDao.LoadById(bzdto.ItemInstanceId);
                        }

                        BazaarList.Add(item);
                    }
                }
                else if (bzlink != null)
                {
                    BazaarList.Remove(bzlink);
                }
            }

            InBazaarRefreshMode = false;
        }

        private void OnFamilyRefresh(object sender, EventArgs e)
        {
            // TODO: Parallelization of family.
            Tuple<long, bool> tuple = (Tuple<long, bool>)sender;
            long familyId = tuple.Item1;
            FamilyDTO famdto = DaoFactory.Instance.FamilyDao.LoadById(familyId);
            Family fam = FamilyList[familyId];
            lock(FamilyList)
            {
                if (famdto != null)
                {
                    if (fam != null)
                    {
                        MapInstance lod = fam.LandOfDeath;
                        lock(FamilyList)
                        {
                            FamilyList.TryRemove(fam.FamilyId, out Family familye);
                        }

                        fam = (Family)famdto;
                        fam.FamilyCharacters = new List<FamilyCharacter>();
                        foreach (FamilyCharacterDTO famchar in DaoFactory.Instance.FamilyCharacterDao.LoadByFamilyId(
                            fam.FamilyId))
                        {
                            fam.FamilyCharacters.Add((FamilyCharacter)famchar);
                        }

                        FamilyCharacter familyLeader =
                            fam.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                        if (familyLeader != null)
                        {
                            fam.Warehouse = new Inventory((Character.Character)familyLeader.Character);
                            foreach (ItemInstanceDTO inventory in DaoFactory.Instance.IteminstanceDao.LoadByCharacterId(familyLeader.CharacterId)
                                .Where(s => s.Type == InventoryType.FamilyWareHouse))
                            {
                                inventory.CharacterId = familyLeader.CharacterId;
                                fam.Warehouse[inventory.Id] = (ItemInstance)inventory;
                            }
                        }

                        fam.FamilyLogs = DaoFactory.Instance.FamilyLogDao.LoadByFamilyId(fam.FamilyId).ToList();
                        fam.LandOfDeath = lod;
                        FamilyList[familyId] = fam;
                        Parallel.ForEach(
                            Sessions.Where(s =>
                                fam.FamilyCharacters.Any(m => m.CharacterId == s.Character.CharacterId)), session =>
                            {
                                session.Character.Family = fam;
                                if (tuple.Item2)
                                {
                                    session.Character.ChangeFaction((FactionType)fam.FamilyFaction);
                                }

                                session.CurrentMapInstance.Broadcast(session.Character.GenerateGidx());
                            });
                    }
                    else
                    {
                        var fami = (Family)famdto;
                        fami.FamilyCharacters = new List<FamilyCharacter>();
                        foreach (FamilyCharacterDTO famchar in DaoFactory.Instance.FamilyCharacterDao.LoadByFamilyId(
                            fami.FamilyId))
                        {
                            fami.FamilyCharacters.Add((FamilyCharacter)famchar);
                        }

                        FamilyCharacter familyCharacter =
                            fami.FamilyCharacters.Find(s => s.Authority == FamilyAuthority.Head);
                        if (familyCharacter != null)
                        {
                            fami.Warehouse = new Inventory((Character.Character)familyCharacter.Character);
                            foreach (ItemInstanceDTO inventory in DaoFactory.Instance.IteminstanceDao.LoadByCharacterId(familyCharacter.CharacterId)
                                .Where(s => s.Type == InventoryType.FamilyWareHouse))
                            {
                                inventory.CharacterId = familyCharacter.CharacterId;
                                fami.Warehouse[inventory.Id] = (ItemInstance)inventory;
                            }
                        }

                        fami.FamilyLogs = DaoFactory.Instance.FamilyLogDao.LoadByFamilyId(fami.FamilyId).ToList();
                        FamilyList[familyId] = fami;
                        Parallel.ForEach(
                            Sessions.Where(
                                s => fami.FamilyCharacters.Any(m => m.CharacterId == s.Character.CharacterId)),
                            session =>
                            {
                                session.Character.Family = fami;
                                if (tuple.Item2)
                                {
                                    session.Character.ChangeFaction((FactionType)fami.FamilyFaction);
                                }

                                session.CurrentMapInstance.Broadcast(session.Character.GenerateGidx());
                            });
                    }
                }
                else if (fam != null)
                {
                    lock(FamilyList)
                    {
                        FamilyList.TryRemove(fam.FamilyId, out Family _);
                    }
                }
            }

            InFamilyRefreshMode = false;
        }

        private void OnAuthorityChange(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            Tuple<long, AuthorityType> args = (Tuple<long, AuthorityType>)sender;
            ClientSession account = Sessions.FirstOrDefault(s => s.Account.AccountId == args.Item1);
            if (account == null)
            {
                return;
            }

            account.Account.Authority = args.Item2;
            account.SendPacket(
                $"say 1 0 10 ({Language.Instance.GetMessageFromKey("ADMINISTRATOR")}) You are now {account.Account.Authority.ToString()}");
        }

        private void OnMailSent(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var message = (MailDTO)sender;
            ClientSession targetSession = Sessions.SingleOrDefault(s => s.Character.CharacterId == message.ReceiverId);
            targetSession?.Character?.GenerateMail(message);
        }

        private void OnMessageSentToCharacter(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var message = (SCSCharacterMessage)sender;

            ClientSession targetSession =
                Sessions.SingleOrDefault(s => s.Character.CharacterId == message.DestinationCharacterId);
            switch (message.Type)
            {
                case MessageType.WhisperGM:
                case MessageType.Whisper:
                    if (targetSession == null || message.Type == MessageType.WhisperGM &&
                        targetSession.Account.Authority != AuthorityType.GameMaster)
                    {
                        return;
                    }

                    if (targetSession.Character.GmPvtBlock)
                    {
                        if (message.DestinationCharacterId != null)
                        {
                            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                            {
                                DestinationCharacterId = message.SourceCharacterId,
                                SourceCharacterId = message.DestinationCharacterId.Value,
                                SourceWorldId = WorldId,
                                Message = targetSession.Character.GenerateSay(
                                    Language.Instance.GetMessageFromKey("GM_CHAT_BLOCKED"), 10),
                                Type = MessageType.PrivateChat
                            });
                        }
                    }
                    else if (targetSession.Character.WhisperBlocked)
                    {
                        if (message.DestinationCharacterId != null)
                        {
                            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                            {
                                DestinationCharacterId = message.SourceCharacterId,
                                SourceCharacterId = message.DestinationCharacterId.Value,
                                SourceWorldId = WorldId,
                                Message = UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("USER_WHISPER_BLOCKED"), 0),
                                Type = MessageType.PrivateChat
                            });
                        }
                    }
                    else
                    {
                        if (message.SourceWorldId != WorldId)
                        {
                            if (message.DestinationCharacterId != null)
                            {
                                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                                {
                                    DestinationCharacterId = message.SourceCharacterId,
                                    SourceCharacterId = message.DestinationCharacterId.Value,
                                    SourceWorldId = WorldId,
                                    Message = targetSession.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("MESSAGE_SENT_TO_CHARACTER"),
                                            targetSession.Character.Name, ChannelId), 11),
                                    Type = MessageType.PrivateChat
                                });
                            }

                            targetSession.SendPacket(
                                $"{message.Message} <{Language.Instance.GetMessageFromKey("CHANNEL")}: {CommunicationServiceClient.Instance.GetChannelIdByWorldId(message.SourceWorldId)}>");
                        }
                        else
                        {
                            targetSession.SendPacket(message.Message);
                        }
                    }

                    break;

                case MessageType.Shout:
                    Shout(message.Message);
                    break;

                case MessageType.PrivateChat:
                    targetSession?.SendPacket(message.Message);
                    break;

                case MessageType.FamilyChat:
                    if (message.DestinationCharacterId.HasValue)
                    {
                        if (message.SourceWorldId != WorldId)
                        {
                            Parallel.ForEach(Instance.Sessions, session =>
                            {
                                if (!session.HasSelectedCharacter || session.Character.Family == null)
                                {
                                    return;
                                }

                                if (session.Character.Family.FamilyId == message.DestinationCharacterId)
                                {
                                    session.SendPacket(
                                        $"say 1 0 6 <{Language.Instance.GetMessageFromKey("CHANNEL")}: {CommunicationServiceClient.Instance.GetChannelIdByWorldId(message.SourceWorldId)}>{message.Message}");
                                }
                            });
                        }
                    }

                    break;

                case MessageType.Family:
                    if (message.DestinationCharacterId.HasValue)
                    {
                        Parallel.ForEach(Instance.Sessions, session =>
                        {
                            if (!session.HasSelectedCharacter || session.Character.Family == null)
                            {
                                return;
                            }

                            if (session.Character.Family.FamilyId == message.DestinationCharacterId)
                            {
                                session.SendPacket(message.Message);
                            }
                        });
                    }

                    break;
            }
        }

        private void OnPenaltyLogRefresh(object sender, EventArgs e)
        {
            int relId = (int)sender;
            PenaltyLogDTO reldto = DaoFactory.Instance.PenaltyLogDao.LoadById(relId);
            PenaltyLogDTO rel = PenaltyLogs.Find(s => s.PenaltyLogId == relId);
            if (reldto != null)
            {
                if (rel == null)
                {
                    PenaltyLogs.Add(reldto);
                }
            }
            else if (rel != null)
            {
                PenaltyLogs.Remove(rel);
            }
        }

        private void OnRelationRefresh(object sender, EventArgs e)
        {
            _inRelationRefreshMode = true;
            long relId = (long)sender;
            lock(CharacterRelations)
            {
                CharacterRelationDTO reldto = DaoFactory.Instance.CharacterRelationDao.LoadById(relId);
                CharacterRelationDTO rel = CharacterRelations.Find(s => s.CharacterRelationId == relId);
                if (reldto != null)
                {
                    if (rel != null)
                    {
                    }
                    else
                    {
                        CharacterRelations.Add(reldto);
                    }
                }
                else if (rel != null)
                {
                    CharacterRelations.Remove(rel);
                }
            }

            _inRelationRefreshMode = false;
        }

        private void OnSessionKicked(object sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            Tuple<long?, long?> kickedSession = (Tuple<long?, long?>)sender;

            ClientSession targetSession = Sessions.FirstOrDefault(s =>
                (!kickedSession.Item1.HasValue || s.SessionId == kickedSession.Item1.Value)
                && (!kickedSession.Item1.HasValue || s.Account.AccountId == kickedSession.Item2));

            targetSession?.Disconnect();
        }

        private void OnShutdown(object sender, EventArgs e)
        {
            Instance.Shutdown();
        }

        private void RemoveItemProcess()
        {
            try
            {
                Parallel.ForEach(Sessions.Where(c => c.IsConnected), session => session.Character?.RefreshValidity());
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}