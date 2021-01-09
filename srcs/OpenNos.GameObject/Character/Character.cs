﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Battle.Args;
using OpenNos.GameObject.Bazaar;
using OpenNos.GameObject.Buff;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Event.TalentArena;
using OpenNos.GameObject.Families;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Miniland;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Npc;
using OpenNos.GameObject.Quests;
using OpenNos.GameObject.Skills;
using OpenNos.GameObject.Trade;
using WingsEmu.Communication;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Character;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Packets.Enums;
using WingsEmu.Packets.ServerPackets;
using WingsEmu.Pathfinder.PathFinder;

namespace OpenNos.GameObject.Character
{
    public class Character : CharacterDTO, IBattleEntity
    {
        #region Instantiation

        public override void Initialize()
        {
            GroupSentRequestCharacterIds = new List<long>();
            FamilyInviteCharacters = new List<long>();
            TradeRequests = new List<long>();
            FriendRequestCharacters = new List<long>();
            StaticBonusList = new List<StaticBonusDTO>();
            Mates = new List<Mate>();
            MeditationDictionary = new Dictionary<short, DateTime>();
            Quests = new ConcurrentBag<CharacterQuest>();
            MTListTargetQueue = new ConcurrentStack<MTListHitTarget>();
            ReflectiveBuffs = new ConcurrentDictionary<short, int?>();
            CanTriggerMegaTitan = true;
            BrawlerMorphType = BrawlerMorphType.Normal;
            ExchangeInfo = null;
            SpCooldown = 30;
            SaveX = 0;
            SaveY = 0;
            LastDefence = DateTime.Now.AddSeconds(-21);
            LastDelay = DateTime.Now.AddSeconds(-5);
            LastHealth = DateTime.Now;
            LastSkillUse = DateTime.Now;
            LastGroupJoin = DateTime.Now;
            LastEffect = DateTime.Now;
            LastMonsterAggro = DateTime.Now;
            LastSpGaugeRemove = DateTime.Now;
            Session = null;
            MailList = new Dictionary<int, MailDTO>();
            BattleEntity = new BattleEntity(this);
            Group = null;
            GmPvtBlock = false;
        }

        #endregion

        #region Members

        private byte _speed;
        private readonly object _syncObj = new object();

        #endregion

        #region Properties

        #region BattleEntityProperties

        public bool CanTriggerMegaTitan { get; set; }

        public BattleEntity BattleEntity { get; set; }

        public void AddBuff(Buff.Buff indicator) => BattleEntity.AddBuff(indicator);

        public void RemoveBuff(short cardId, bool removePermaBuff = false) =>
            BattleEntity.RemoveBuff(cardId, removePermaBuff);

        public int[] GetBuff(BCardType.CardType type, byte subtype) => BattleEntity.GetBuff(type, subtype);

        public bool HasBuff(BCardType.CardType type, byte subtype, bool removeWeaponEffects = false) =>
            BattleEntity.HasBuff(type, subtype, removeWeaponEffects);

        public bool HasBuff(BuffType type) => BattleEntity.HasBuff(type);

        public ConcurrentBag<Buff.Buff> Buff => BattleEntity.Buffs;

        public void DisableBuffs(List<BuffType> types, int level = 100) => BattleEntity.DisableBuffs(types, level);

        public ConcurrentBag<EquipmentOptionDTO> ShellOptionsMain => BattleEntity.ShellOptionsMain;

        public ConcurrentBag<EquipmentOptionDTO> ShellOptionsSecondary => BattleEntity.ShellOptionsSecondary;

        public ConcurrentBag<EquipmentOptionDTO> ShellOptionArmor => BattleEntity.ShellOptionArmor;

        public int MaxHp => (int)HpLoad();

        #endregion

        public IDisposable SaveObs { get; set; }

        public IDisposable AntiBotObservable { get; set; }

        public IDisposable AntiBotMessageInterval { get; set; }

        public short AntiBotIdentificator { get; set; }

        public byte AntiBotCount { get; set; }

        public IDisposable DragonModeObservable { get; set; }

        public BrawlerMorphType BrawlerMorphType { get; set; }

        public IEnumerable<CharacterHomeDTO> Homes
        {
            get
            {
                lock(ServerManager.Instance.CharacterHomes)
                {
                    return ServerManager.Instance.CharacterHomes == null ||
                        !ServerManager.Instance.CharacterHomes.Any()
                            ? new List<CharacterHomeDTO>()
                            : ServerManager.Instance.CharacterHomes.Where(s => s.CharacterId == CharacterId);
                }
            }
        }

        public int RetainedHp { get; set; }

        public int AccumulatedDamage { get; set; }

        public UseSkillPacket FocusedMonster { get; set; }

        public IDisposable WeddingEffect { get; set; }

        public bool IsWaitingForWedding { get; set; }

        public IDisposable DotDebuff { get; set; }

        // <BuffId, BuffReflexion>
        public ConcurrentDictionary<short, int?> ReflectiveBuffs { get; set; }

        public DateTime LastMegaTitanBuff { get; set; }

        public ConcurrentStack<MTListHitTarget> MTListTargetQueue { get; set; }

        public bool TriggerAmbush { get; set; }

        public byte SkillComboCount { get; set; }

        public DateTime LastSkillCombo { get; set; }

        public DateTime LastQuest { get; set; }

        public DateTime LastUnregister { get; set; }

        public AuthorityType Authority { get; set; }

        public Node[][] BrushFire { get; set; }

        public bool CanFight => !IsSitting && ExchangeInfo == null;

        public List<CharacterRelationDTO> CharacterRelations
        {
            get
            {
                lock(ServerManager.Instance.CharacterRelations)
                {
                    return ServerManager.Instance.CharacterRelations == null
                        ? new List<CharacterRelationDTO>()
                        : ServerManager.Instance.CharacterRelations.Where(s =>
                            s.CharacterId == CharacterId || s.RelatedCharacterId == CharacterId).ToList();
                }
            }
        }

        public int CurrentHp
        {
            get => Hp;
            set => Hp = value;
        }

        public short CurrentMinigame { get; set; }

        public int DarkResistance
        {
            get => BattleEntity.DarkResistance;
            set => BattleEntity.DarkResistance = value;
        }

        public int DealtDamage { get; set; }

        public int Defence
        {
            get => BattleEntity.CloseDefence;
            set => BattleEntity.CloseDefence = value;
        }

        public int DefenceRate
        {
            get => BattleEntity.DefenceRate;
            set => BattleEntity.DefenceRate = value;
        }

        public int Direction { get; set; }

        public int DistanceCritical { get; set; }

        public int DistanceCriticalRate { get; set; }

        public int DistanceDefence
        {
            get => BattleEntity.RangedDefence;
            set => BattleEntity.RangedDefence = value;
        }

        public int DistanceDefenceRate
        {
            get => BattleEntity.DistanceDefenceRate;
            set => BattleEntity.DistanceDefenceRate = value;
        }

        public int DistanceRate { get; set; }

        public int ChargeValue { get; set; }

        public byte Element
        {
            get => BattleEntity.Element;
            set => BattleEntity.Element = value;
        }

        public int ElementRate
        {
            get => BattleEntity.ElementRate;
            set => BattleEntity.ElementRate = value;
        }

        public int ElementRateSp
        {
            get => BattleEntity.ElementRateSp;
            set => BattleEntity.ElementRateSp = value;
        }

        public ExchangeInfo ExchangeInfo { get; set; }

        public Family Family { get; set; }

        public FamilyCharacterDTO FamilyCharacter
        {
            get { return Family?.FamilyCharacters.FirstOrDefault(s => s.CharacterId == CharacterId); }
        }

        public List<long> FamilyInviteCharacters { get; set; }

        public int FireResistance
        {
            get => BattleEntity.FireResistance;
            set => BattleEntity.FireResistance = value;
        }

        public int FoodAmount { get; set; }

        public int FoodHp { get; set; }

        public int FoodMp { get; set; }

        public List<long> FriendRequestCharacters { get; set; }

        public List<GeneralLogDTO> GeneralLogs { get; set; }

        public bool GmPvtBlock { get; set; }

        public Group Group { get; set; }

        public List<long> GroupSentRequestCharacterIds { get; set; }

        public bool HasGodMode { get; set; }

        public bool HasShopOpened { get; set; }

        public int HitCritical { get; set; }

        public int HitCriticalRate { get; set; }

        public int HitRate
        {
            get => BattleEntity.HitRate;
            set => BattleEntity.HitRate = value;
        }

        public bool InExchangeOrTrade => ExchangeInfo != null || Speed == 0;

        public Inventory Inventory { get; set; }

        public bool Invisible { get; set; }

        public bool InvisibleGm { get; set; }

        public bool IsChangingMapInstance { get; set; }

        public bool IsCustomSpeed { get; set; }

        public bool IsDancing { get; set; }

        public bool IsDead { get; set; }

        /// <summary>
        ///     Defines if the Character Is currently sending or getting items thru exchange.
        /// </summary>
        public bool IsExchanging { get; set; }

        public bool IsShopping { get; set; }

        public bool IsSitting { get; set; }

        public bool IsVehicled { get; set; }

        public bool IsWaitingForEvent { get; set; }

        public DateTime LastDefence { get; set; }

        public DateTime LastDelay { get; set; }

        public DateTime LastDeath { get; set; }

        public DateTime LastEffect { get; set; }

        public DateTime LastHealth { get; set; }

        public DateTime LastGroupJoin { get; set; }

        public DateTime LastMapObject { get; set; }

        public DateTime LastMonsterAggro { get; set; }

        public int LastMonsterId { get; set; }

        public short LastUsedItem { get; set; }

        public Dictionary<short, DateTime> MeditationDictionary { get; set; }

        public ConcurrentBag<IDisposable> BuffObservables { get; internal set; }

        public DateTime LastMove { get; set; }

        public int LastNRunId { get; set; }

        public double LastPortal { get; set; }

        public DateTime LastPotion { get; set; }

        public int LastPulse { get; set; }

        public DateTime LastPvpRevive { get; set; }

        public DateTime LastQuestSummon { get; set; }

        public DateTime LastSkillUse { get; set; }

        public double LastSp { get; set; }

        public DateTime LastSpeedChange { get; set; }

        public DateTime LastSpGaugeRemove { get; set; }

        public DateTime LastTransform { get; set; }

        public int LightResistance
        {
            get => BattleEntity.LightResistance;
            set => BattleEntity.LightResistance = value;
        }

        public int MagicalDefence
        {
            get => BattleEntity.MagicDefence;
            set => BattleEntity.MagicDefence = value;
        }

        public IDictionary<int, MailDTO> MailList { get; set; }

        public MapInstance MapInstance => ServerManager.Instance.GetMapInstance(MapInstanceId);

        public Guid MapInstanceId { get; set; }

        public List<Mate> Mates { get; set; }

        public int MaxDistance { get; set; }

        public int BuffRandomTime { get; set; }

        public int MaxFood { get; set; }

        public int MaxHit { get; set; }

        public int MaxSnack { get; set; }

        public int MinDistance { get; set; }

        public int MinHit { get; set; }

        public MapInstance Miniland { get; private set; }

        public int Morph { get; set; }

        public int MorphUpgrade { get; set; }

        public int MorphUpgrade2 { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public int TotalTime { get; set; }

        public ConcurrentBag<CharacterQuest> Quests { get; set; } = new ConcurrentBag<CharacterQuest>();

        public List<QuicklistEntryDTO> QuicklistEntries { get; private set; } = new List<QuicklistEntryDTO>();

        public RespawnMapTypeDTO Respawn
        {
            get
            {
                var respawn = new RespawnMapTypeDTO
                {
                    DefaultX = 78,
                    DefaultY = 118,
                    DefaultMapId = 1,
                    RespawnMapTypeId = -1
                };

                if (MapInstance?.MapInstanceType == MapInstanceType.Act4Instance)
                {
                    return new RespawnMapTypeDTO
                    {
                        DefaultX = 40,
                        DefaultY = 42,
                        DefaultMapId = (short)(Faction == FactionType.Angel ? 130 : 131),
                        RespawnMapTypeId = -1
                    };
                }

                if (!Session.HasCurrentMapInstance || Session.CurrentMapInstance.Map.MapTypes.Count == 0)
                {
                    return respawn;
                }

                long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes.ElementAt(0).RespawnMapTypeId;
                if (respawnmaptype == null)
                {
                    return respawn;
                }

                RespawnDTO resp = Respawns.FirstOrDefault(s => s.RespawnMapTypeId == respawnmaptype);
                if (resp == null)
                {
                    RespawnMapTypeDTO defaultresp = Session.CurrentMapInstance.Map.DefaultRespawn;
                    if (defaultresp == null)
                    {
                        return respawn;
                    }

                    respawn.DefaultX = defaultresp.DefaultX;
                    respawn.DefaultY = defaultresp.DefaultY;
                    respawn.DefaultMapId = defaultresp.DefaultMapId;
                    respawn.RespawnMapTypeId = (long)respawnmaptype;
                }
                else
                {
                    respawn.DefaultX = resp.X;
                    respawn.DefaultY = resp.Y;
                    respawn.DefaultMapId = resp.MapId;
                    respawn.RespawnMapTypeId = (long)respawnmaptype;
                }

                return respawn;
            }
        }

        public List<RespawnDTO> Respawns { private get; set; }

        public RespawnMapTypeDTO Return
        {
            get
            {
                var respawn = new RespawnMapTypeDTO();
                if (!Session.HasCurrentMapInstance || Session.CurrentMapInstance.Map.MapTypes.Count == 0)
                {
                    return respawn;
                }

                long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes.ElementAt(0).ReturnMapTypeId;
                if (respawnmaptype == null)
                {
                    return respawn;
                }

                RespawnDTO resp = Respawns.FirstOrDefault(s => s.RespawnMapTypeId == respawnmaptype);
                if (resp == null)
                {
                    RespawnMapTypeDTO defaultresp = Session.CurrentMapInstance.Map.DefaultReturn;
                    if (defaultresp == null)
                    {
                        return respawn;
                    }

                    respawn.DefaultX = defaultresp.DefaultX;
                    respawn.DefaultY = defaultresp.DefaultY;
                    respawn.DefaultMapId = defaultresp.DefaultMapId;
                    respawn.RespawnMapTypeId = (long)respawnmaptype;
                }
                else
                {
                    respawn.DefaultX = resp.X;
                    respawn.DefaultY = resp.Y;
                    respawn.DefaultMapId = resp.MapId;
                    respawn.RespawnMapTypeId = (long)respawnmaptype;
                }

                return respawn;
            }
        }

        public short SaveX { get; set; }

        public short SaveY { get; set; }

        public int ScPage { get; set; }

        public ClientSession Session { get; private set; }

        public int Size { get; set; } = 10;

        public ConcurrentDictionary<int, CharacterSkill> Skills { get; private set; } = new ConcurrentDictionary<int, CharacterSkill>();

        public ConcurrentDictionary<int, CharacterSkill> SkillsSp { get; set; }

        public int SnackAmount { get; set; }

        public int SnackHp { get; set; }

        public int SnackMp { get; set; }

        public int SpCooldown { get; set; }

        public byte Speed
        {
            get
            {
                byte bonusSpeed = (byte)GetBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.SetMovementNegated)[0];
                if (_speed + bonusSpeed > 59)
                {
                    return 59;
                }

                return (byte)(_speed + bonusSpeed);
            }

            set
            {
                LastSpeedChange = DateTime.Now;
                _speed = value > 59 ? (byte)59 : value;
            }
        }

        public List<StaticBonusDTO> StaticBonusList { get; set; }

        public int TimesUsed { get; set; }

        public List<long> TradeRequests { get; set; }

        public bool Undercover { get; set; }

        public bool UseSp { get; set; }

        public byte VehicleSpeed { private get; set; }

        public SpecialistInstance SpInstance { get; set; }

        public int WareHouseSize { get; set; }

        public int WaterResistance
        {
            get => BattleEntity.WaterResistance;
            set => BattleEntity.WaterResistance = value;
        }

        public IDisposable Life { get; set; }

        public bool CanAttack { get; set; }

        public int SheepScore1 { get; set; }

        public int SheepScore2 { get; set; }

        public int SheepScore3 { get; set; }

        public bool IsWaitingForGift { get; set; }

        #endregion

        #region Methods

        public void PushBackToDirection(int range)
        {
            short x = PositionX, y = PositionY;
            var type = MoveType.Up;
            switch (Direction)
            {
                case 0:
                    y += (short)range;
                    type = MoveType.Down;
                    break;
                case 1:
                    x -= (short)range;
                    type = MoveType.Left;
                    break;
                case 2:
                    y -= (short)range;
                    type = MoveType.Up;
                    break;
                case 3:
                    x += (short)range;
                    type = MoveType.Right;
                    break;
                case 4:
                    x += (short)range;
                    y += (short)range;
                    type = MoveType.DiagDownRight;
                    break;
                case 5:
                    x -= (short)range;
                    y += (short)range;
                    type = MoveType.DiagDownLeft;
                    break;
                case 6:
                    x -= (short)range;
                    y -= (short)range;
                    type = MoveType.DiagUpLeft;
                    break;
                case 7:
                    x += (short)range;
                    y -= (short)range;
                    type = MoveType.DiagUpRight;
                    break;
            }

            //MapCell cell = MapInstance.Map.GetLastGoodPosition(x, y, type, Session);

            //x = cell.X;
            //y = cell.Y;

            if (!MapInstance.Map.GetDefinedPosition(x, y))
            {
                return;
            }

            MapInstance?.Broadcast($"guri 3 1 {CharacterId} {x} {y} 3 8 2 - 1");
            PositionX = x;
            PositionY = y;
        }

        public string GenerateSmemo(string message, byte type) => $"s_memo {type} {message}";

        public string GenerateGb(byte type) => $"gb {type} {Session.Account.BankMoney / 1000} {Gold} 0 0";

        public void OpenBank()
        {
            Session.SendPacket(GenerateGb((byte)GoldBankPacketType.OpenBank));
            Session.SendPacket(GenerateSmemo(Language.Instance.GetMessageFromKey("OPEN_BANK"), (byte)SmemoType.Information));
        }

        public void GenerateSheepScore(UserType type)
        {
            if (!CanAttack)
            {
                return;
            }

            // For SheepScore , If u have Killed Most Player and u are not Killed you have more Point in official need to re-write this later c:
            // Not Important for moment.
            switch (type)
            {
                case UserType.Player:
                    SheepScore1 += 10;
                    SheepScore3 += 1;
                    break;
                case UserType.Monster:
                    SheepScore1 += 5;
                    SheepScore2 += 1;
                    break;
                default:
                    return;
            }

            Session.CurrentMapInstance?.Broadcast($"srlst 2 {CharacterId} {Name} {SheepScore1} {SheepScore2} {SheepScore3}");
            CanAttack = false;
            Session.SendPacket("sh_c");
            Observable.Timer(TimeSpan.FromSeconds(7)).Subscribe(s =>
            {
                Session.SendPacket("sh_o");
                CanAttack = true;
            });
        }

        public string GenerateDm(ushort dmg) => $"dm 1 {CharacterId} {dmg}";

        public string GeneratePetskill(int VNum = -1) => $"petski {VNum}";

        public void AddQuest(long questId, bool isMain = false)
        {
            var characterQuest = new CharacterQuest(questId, CharacterId);
            if (Quests.Any(q => q.QuestId == questId) || characterQuest.Quest == null ||
                isMain && Quests.Any(q => q.IsMainQuest) ||
                Quests.Where(q => q.Quest.QuestType != (byte)QuestType.WinRaid).ToList().Count >= 5 &&
                characterQuest.Quest.QuestType != (byte)QuestType.WinRaid && !isMain)
            {
                return;
            }

            if (characterQuest.Quest.LevelMin > Level)
            {
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                return;
            }

            if (ServerManager.Instance.MaxLevel == 99 && characterQuest.Quest.LevelMax < Level)
            {
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_HIGH_LVL"), 0));
                return;
            }

            if (!characterQuest.Quest.IsDaily && !characterQuest.IsMainQuest)
            {
                if (DaoFactory.Instance.QuestLogDao.LoadByCharacterId(CharacterId).Any(s => s.QuestId == questId))
                {
                    return;
                }
            }
            else if (characterQuest.Quest.IsDaily)
            {
                if (DaoFactory.Instance.QuestLogDao.LoadByCharacterId(CharacterId).Any(s =>
                    s.QuestId == questId && s.LastDaily != null && s.LastDaily.Value.AddHours(24) >= DateTime.Now))
                {
                    Session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(
                            Language.Instance.GetMessageFromKey("QUEST_ALREADY_DONE"), 0));
                    return;
                }
            }

            if (characterQuest.Quest.QuestType == (int)QuestType.TimesSpace &&
                ServerManager.Instance.TimeSpaces.All(t =>
                    t.LevelMinimum != (characterQuest.Quest.QuestObjectives.FirstOrDefault()?.Data ?? -1))
                || characterQuest.Quest.QuestType == (int)QuestType.Product ||
                characterQuest.Quest.QuestType == (int)QuestType.Collect3
                || characterQuest.Quest.QuestType == (int)QuestType.TransmitGold ||
                characterQuest.Quest.QuestType == (int)QuestType.TsPoint ||
                characterQuest.Quest.QuestType == (int)QuestType.NumberOfKill
                || characterQuest.Quest.QuestType == (int)QuestType.TargetReput ||
                characterQuest.Quest.QuestType == (int)QuestType.Inspect ||
                characterQuest.Quest.QuestType == (int)QuestType.Needed
                || characterQuest.Quest.QuestType == (int)QuestType.Collect5 ||
                QuestHelper.Instance.SkipQuests.Any(q => q == characterQuest.QuestId))
            {
                AddQuest(characterQuest.Quest.NextQuestId ?? -1, isMain);
                return;
            }

            if (characterQuest.Quest.TargetMap != null)
            {
                Session.SendPacket(characterQuest.Quest.TargetPacket());
            }

            characterQuest.IsMainQuest = isMain;
            Quests.Add(characterQuest);
            Session.SendPacket(GenerateQuestsPacket(questId));
        }

        public void RemoveQuest(long questId, bool IsGivingUp = false)
        {
            CharacterQuest questToRemove = Quests.FirstOrDefault(q => q.QuestId == questId);
            if (questToRemove == null)
            {
                return;
            }

            LogHelper.Instance.InsertQuestLog(CharacterId, Session.IpAddress, questToRemove.Quest.QuestId,
                DateTime.Now);
            if (questToRemove.Quest.TargetMap != null)
            {
                Session.SendPacket(questToRemove.Quest.RemoveTargetPacket());
            }

            Quests.RemoveWhere(s => s.QuestId != questId, out ConcurrentBag<CharacterQuest> tmp);
            Quests = tmp;
            Session.SendPacket(GenerateQuestsPacket());
            if (IsGivingUp)
            {
                return;
            }

            if (questToRemove.Quest.EndDialogId != null)
            {
                Session.SendPacket(GenerateNpcDialog((int)questToRemove.Quest.EndDialogId));
            }

            if (questToRemove.Quest.NextQuestId != null)
            {
                AddQuest((long)questToRemove.Quest.NextQuestId, questToRemove.IsMainQuest);
            }
        }

        public string GenerateQuestsPacket(long newQuestId = -1)
        {
            short a = 0;
            short b = 6;
            Quests.ToList().ForEach(qst =>
            {
                qst.QuestNumber = qst.IsMainQuest
                    ? (short)5
                    : !qst.IsMainQuest && !qst.Quest.IsDaily
                        ? b++
                        : a++;
            });
            return
                $"qstlist {Quests.Aggregate(string.Empty, (current, quest) => current + $" {quest.GetInfoPacket(quest.QuestId == newQuestId)}")}";
        }

        public void IncrementQuests(QuestType type, int firstData = 0, int secondData = 0, int thirdData = 0)
        {
            foreach (CharacterQuest quest in Quests.Where(q => q?.Quest?.QuestType == (int)type))
            {
                switch ((QuestType)quest.Quest.QuestType)
                {
                    case QuestType.Capture1:
                    case QuestType.Capture2:
                    case QuestType.WinRaid:
                    case QuestType.Collect1:
                    case QuestType.Collect2:
                    case QuestType.Collect3:
                    case QuestType.Collect4:
                    case QuestType.Hunt:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList()
                            .ForEach(d => IncrementObjective(quest, d.ObjectiveIndex));
                        break;

                    case QuestType.Product:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList()
                            .ForEach(d => IncrementObjective(quest, d.ObjectiveIndex, secondData));
                        break;

                    case QuestType.Dialog1:
                    case QuestType.Dialog2:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList().ForEach(d =>
                            IncrementObjective(quest, d.ObjectiveIndex, isOver: true));
                        break;

                    case QuestType.Wear:
                        if (quest.Quest.QuestObjectives.Any(q => q.SpecialData == firstData &&
                            (Session.Character.Inventory.Any(i =>
                                    i.Value.ItemVNum == q.Data &&
                                    i.Value.Type == InventoryType.Wear) ||
                                (quest.QuestId == 1541 || quest.QuestId == 1546) &&
                                Class != ClassType.Adventurer)))
                        {
                            IncrementObjective(quest, isOver: true);
                        }

                        break;

                    case QuestType.Brings:
                    case QuestType.Required:
                        quest.Quest.QuestObjectives.Where(o => o.Data == firstData).ToList().ForEach(d =>
                        {
                            if (Inventory.CountItem(d.SpecialData ?? -1) >= d.Objective)
                            {
                                Inventory.RemoveItemAmount(d.SpecialData ?? -1, d.Objective ?? 1);
                                IncrementObjective(quest, d.ObjectiveIndex, d.Objective ?? 1);
                            }
                        });
                        break;

                    case QuestType.GoTo:
                        if (quest.Quest.TargetMap == firstData && Math.Abs(secondData - quest.Quest.TargetX ?? 0) < 3 &&
                            Math.Abs(thirdData - quest.Quest.TargetY ?? 0) < 3)
                        {
                            IncrementObjective(quest, isOver: true);
                        }

                        break;

                    case QuestType.Use:
                        quest.Quest.QuestObjectives
                            .Where(o => o.Data == firstData &&
                                Mates.Any(m => m.NpcMonsterVNum == o.SpecialData && m.IsTeamMember)).ToList()
                            .ForEach(d => IncrementObjective(quest, d.ObjectiveIndex, d.Objective ?? 1));
                        break;

                    case QuestType.FlowerQuest:
                        if (firstData + 10 < Level)
                        {
                            continue;
                        }

                        IncrementObjective(quest, 1);
                        break;

                    //TODO : Later 
                    case QuestType.TsPoint:
                    case QuestType.TimesSpace:
                    case QuestType.NumberOfKill:
                    case QuestType.Inspect:
                    case QuestType.Needed:
                    case QuestType.TargetReput:
                    case QuestType.TransmitGold:
                    case QuestType.Collect5:
                        break;
                }
            }
        }

        private void IncrementObjective(CharacterQuest quest, byte index = 0, int amount = 1, bool isOver = false)
        {
            bool isFinish = isOver;
            quest.Incerment(index, amount);
            byte a = 1;
            if (quest.GetObjectives().All(q =>
                quest.GetObjectiveByIndex(a) == null || q >= quest.GetObjectiveByIndex(a++).Objective))
            {
                isFinish = true;
            }

            Session.SendPacket($"qsti {quest.GetInfoPacket(false)}");

            if (!isFinish)
            {
                return;
            }

            LastQuest = DateTime.Now;
            if (CustomQuestRewards((QuestType)quest.Quest.QuestType))
            {
                RemoveQuest(quest.QuestId);
                return;
            }

            Session.SendPacket(quest.Quest.GetRewardPacket(this));
            RemoveQuest(quest.QuestId);
        }

        public bool CustomQuestRewards(QuestType type)
        {
            switch (type)
            {
                case QuestType.FlowerQuest:
                    if (ServerManager.Instance.RandomNumber() < 50)
                    {
                        AddBuff(new Buff.Buff(378, Level));
                    }
                    else
                    {
                        AddBuff(new Buff.Buff(379, Level));
                    }

                    return true;

                default:
                    return false;
            }
        }

        public string GenerateAct6() =>
            $"act6 1 0 {ServerManager.Instance.Act6Zenas.Percentage / 10} {Convert.ToByte(ServerManager.Instance.Act6Zenas.Mode)} {ServerManager.Instance.Act6Zenas.CurrentTime} {ServerManager.Instance.Act6Zenas.TotalTime} {ServerManager.Instance.Act6Erenia.Percentage / 10} {Convert.ToByte(ServerManager.Instance.Act6Erenia.Mode)} {ServerManager.Instance.Act6Erenia.CurrentTime} {ServerManager.Instance.Act6Erenia.TotalTime}";

        public string GenerateFc() =>
            $"fc {(byte)Faction} {ServerManager.Instance.Act4AngelStat.MinutesUntilReset} {ServerManager.Instance.Act4AngelStat.Percentage / 100} {ServerManager.Instance.Act4AngelStat.Mode}" +
            $" {ServerManager.Instance.Act4AngelStat.CurrentTime} {ServerManager.Instance.Act4AngelStat.TotalTime} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsMorcos)}" +
            $" {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsHatus)} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsCalvina)} {Convert.ToByte(ServerManager.Instance.Act4AngelStat.IsBerios)}" +
            $" 0 {ServerManager.Instance.Act4DemonStat.Percentage / 100} {ServerManager.Instance.Act4DemonStat.Mode} {ServerManager.Instance.Act4DemonStat.CurrentTime} {ServerManager.Instance.Act4DemonStat.TotalTime}" +
            $" {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsMorcos)} {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsHatus)} {Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsCalvina)} " +
            $"{Convert.ToByte(ServerManager.Instance.Act4DemonStat.IsBerios)} 0";

        public string GenerateDg()
        {
            if (ServerManager.Instance.Act4RaidStart.AddMinutes(60) < DateTime.Now)
            {
                ServerManager.Instance.Act4RaidStart = DateTime.Now;
            }

            double seconds = (ServerManager.Instance.Act4RaidStart.AddMinutes(60) - DateTime.Now).TotalSeconds;
            return
                $"dg {(Session?.Character?.Family?.Act4Raid.Id ?? 0) + 1} {(seconds > 1800 ? 1 : 2)} {(int)seconds} 0";
        }

        public bool AddPet(Mate mate)
        {
            if (mate.MateType == MateType.Pet
                ? MaxMateCount <= Mates.Count
                : 3 <= Mates.Count(s => s.MateType == MateType.Partner))
            {
                return false;
            }

            Mates.Add(mate);
            MapInstance.Broadcast(mate.GenerateIn());
            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("YOU_GET_PET"), mate.Name),
                12));
            Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
            Session.SendPackets(GenerateScP());
            Session.SendPackets(GenerateScN());
            mate.RefreshStats();
            return true;
        }

        public void AddRelation(long characterId, CharacterRelationType relation)
        {
            var addRelation = new CharacterRelationDTO
            {
                CharacterId = CharacterId,
                RelatedCharacterId = characterId,
                RelationType = relation
            };

            DaoFactory.Instance.CharacterRelationDao.InsertOrUpdate(ref addRelation);
            ServerManager.Instance.RelationRefresh(addRelation.CharacterRelationId);
            Session.SendPacket(GenerateFinit());
            ClientSession target =
                ServerManager.Instance.Sessions.FirstOrDefault(s => s.Character?.CharacterId == characterId);
            target?.SendPacket(target.Character.GenerateFinit());
        }

        public void ChangeFaction(FactionType faction)
        {
            Faction = faction;
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_{(int)Faction}"), 0));
            Session.SendPacket("scr 0 0 0 0 0 0");
            Session.SendPacket(GenerateFaction());
            Session.SendPacket(GenerateStatChar());
            Session.SendPacket(GenerateEff(4799 + (int)Faction));
            Session.SendPacket(GenerateCond());
            Session.SendPacket(GenerateLev());
        }

        public void ChangeClass(ClassType characterClass, bool isCommand = false)
        {
            JobLevel = (byte)(isCommand && characterClass != ClassType.Adventurer ? JobLevel :
                isCommand && characterClass == ClassType.Adventurer ? 20 : 1);
            JobLevelXp = 0;
            Session.SendPacket("npinfo 0");
            Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());

            if (characterClass == (byte)ClassType.Adventurer)
            {
                HairStyle = (byte)HairStyle > 1 ? 0 : HairStyle;
            }

            LoadSpeed();
            Class = characterClass;
            Hp = (int)HpLoad();
            Mp = (int)MpLoad();
            Session.SendPacket(GenerateTit());
            Session.SendPacket(GenerateStat());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateEq());
            Session.CurrentMapInstance?.Broadcast(GenerateEff(8), PositionX, PositionY);
            Session.SendPacket(
                UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("CLASS_CHANGED"), 0));
            Session.CurrentMapInstance?.Broadcast(GenerateEff(196), PositionX, PositionY);
            Faction = Session.Character.Family == null
                ? (FactionType)(1 + ServerManager.Instance.RandomNumber(0, 2))
                : (FactionType)Session.Character.Family.FamilyFaction;
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                Language.Instance.GetMessageFromKey($"GET_PROTECTION_POWER_{(int)Faction}"), 0));
            Session.SendPacket("scr 0 0 0 0 0 0");
            Session.SendPacket(GenerateFaction());
            Session.SendPacket(GenerateStatChar());
            Session.SendPacket(GenerateEff(4799 + (int)Faction));
            Session.SendPacket(GenerateCond());
            Session.SendPacket(GenerateLev());
            Session.CurrentMapInstance?.Broadcast(Session, GenerateCMode());
            Session.CurrentMapInstance?.Broadcast(Session, this.GenerateIn(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, this.GenerateGidx(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(GenerateEff(6), PositionX, PositionY);
            Session.CurrentMapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
            foreach (CharacterSkill skill in Skills.Select(s => s.Value))
            {
                if (skill.SkillVNum >= 200)
                {
                    Skills.TryRemove(skill.SkillVNum, out CharacterSkill value);
                }
            }

            Skills[(short)(200 + 20 * (byte)Class)] = new CharacterSkill
            {
                SkillVNum = (short)(200 + 20 * (byte)Class),
                CharacterId = CharacterId
            };
            Skills[(short)(201 + 20 * (byte)Class)] = new CharacterSkill
            {
                SkillVNum = (short)(201 + 20 * (byte)Class),
                CharacterId = CharacterId
            };
            Skills[236] = new CharacterSkill { SkillVNum = 236, CharacterId = CharacterId };

            Session.SendPacket(GenerateSki());
            foreach (QuicklistEntryDTO quicklists in DaoFactory.Instance.QuicklistEntryDao.LoadByCharacterId(CharacterId)
                .Where(quicklists => QuicklistEntries.Any(qle => qle.Id == quicklists.Id)))
            {
                DaoFactory.Instance.QuicklistEntryDao.Delete(quicklists.Id);
            }

            QuicklistEntries = new List<QuicklistEntryDTO>
            {
                new QuicklistEntryDTO
                {
                    CharacterId = CharacterId,
                    Q1 = 0,
                    Q2 = 9,
                    Type = 1,
                    Slot = 3,
                    Pos = 1
                }
            };
            if (ServerManager.Instance.Groups.Any(s => s.IsMemberOfGroup(Session) && s.GroupType == GroupType.Group))
            {
                Session.CurrentMapInstance?.Broadcast(Session, $"pidx 1 1.{CharacterId}", ReceiverType.AllExceptMe);
            }
        }

        public void CheckHuntQuest()
        {
            CharacterQuest quest = Quests?.FirstOrDefault(q =>
                q?.Quest?.QuestType == (int)QuestType.Hunt && q.Quest?.TargetMap == MapInstance?.Map?.MapId &&
                Math.Abs(PositionX - q.Quest?.TargetX ?? 0) < 2 && Math.Abs(PositionY - q.Quest?.TargetY ?? 0) < 2);
            if (quest == null)
            {
                return;
            }

            ConcurrentBag<ToSummon> monsters = new ConcurrentBag<ToSummon>();
            for (int a = 0; a < quest.GetObjectiveByIndex(1)?.Objective / 2 + 1; a++)
            {
                monsters.Add(new ToSummon((short)(quest.GetObjectiveByIndex(1)?.Data ?? -1),
                    new MapCell
                    {
                        X = (short)(PositionX + ServerManager.Instance.RandomNumber(-2, 3)),
                        Y = (short)(PositionY + ServerManager.Instance.RandomNumber(-2, 3))
                    }, this, true));
            }

            EventHelper.Instance.RunEvent(new EventContainer(MapInstance, EventActionType.SPAWNMONSTERS,
                monsters.AsEnumerable()));
        }

        public void UpdateBushFire()
        {
            if (Session.Character == null || Session.CurrentMapInstance == null)
            {
                return;
            }

            BrushFire = BestFirstSearch.LoadBrushFireJagged(new GridPos
            {
                X = PositionX,
                Y = PositionY
            }, Session.CurrentMapInstance.Map.Grid);
        }

        public string GenerateBsInfo(byte type, int arenaeventtype, int time, byte titletype) => $"bsinfo {type} {arenaeventtype} {time} {titletype}";

        public void ChangeSex()
        {
            Gender = Gender == GenderType.Female ? GenderType.Male : GenderType.Female;
            if (IsVehicled)
            {
                Morph = Gender == GenderType.Female ? Morph + 1 : Morph - 1;
            }

            Session.SendPacket(
                UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SEX_CHANGED"), 0));
            Session.SendPacket(GenerateEq());
            Session.SendPacket(GenerateGender());
            Session.CurrentMapInstance?.Broadcast(Session, this.GenerateIn(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, this.GenerateGidx(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(GenerateCMode());
            Session.CurrentMapInstance?.Broadcast(GenerateEff(196), PositionX, PositionY);
        }

        public void CharacterLife()
        {
            if (Hp == 0 && LastHealth.AddSeconds(2) <= DateTime.Now)
            {
                Mp = 0;
                Session.SendPacket(GenerateStat());
                LastHealth = DateTime.Now;
                return;
            }

            bool change = false;

            if (LastHealth.AddSeconds(2) <= DateTime.Now || IsSitting && LastHealth.AddSeconds(1.5) <= DateTime.Now)
            {
                LastHealth = DateTime.Now;
                if (LastDefence.AddSeconds(4) <= DateTime.Now && LastSkillUse.AddSeconds(2) <= DateTime.Now && Hp > 0)
                {
                    change = Hp != (int)HpLoad() || change;
                    Hp += Hp + HealthHpLoad() < HpLoad() ? HealthHpLoad() : (int)HpLoad() - Hp;

                    change = Mp != (int)MpLoad() || change;
                    Mp += Mp + HealthMpLoad() < MpLoad() ? HealthMpLoad() : (int)MpLoad() - Mp;
                }
            }

            if (change)
            {
                Session.SendPacket(GenerateStat());
            }

            if (Session.Character.LastQuestSummon.AddSeconds(7) < DateTime.Now) // Quest in which you make monster spawn
            {
                Session.Character.CheckHuntQuest();
                Session.Character.LastQuestSummon = DateTime.Now;
            }

            if (CurrentMinigame != 0 && LastEffect.AddSeconds(3) <= DateTime.Now)
            {
                Session.CurrentMapInstance?.Broadcast(GenerateEff(CurrentMinigame));
                LastEffect = DateTime.Now;
            }

            if (LastEffect.AddSeconds(5) <= DateTime.Now)
            {
                if (Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.RaidInstance)
                {
                    Session.SendPacket(Session.Character.GenerateRaid(3, false));
                }

                BattleEntity.CellonOptions.Clear();
                var ring = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Ring, InventoryType.Wear);
                var bracelet = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Bracelet, InventoryType.Wear);
                var necklace = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Necklace, InventoryType.Wear);
                if (ring?.EquipmentOptions != null)
                {
                    foreach (EquipmentOptionDTO option in ring?.EquipmentOptions)
                    {
                        BattleEntity.CellonOptions.Add(option);
                    }
                }

                if (bracelet?.EquipmentOptions != null)
                {
                    foreach (EquipmentOptionDTO option in bracelet?.EquipmentOptions)
                    {
                        BattleEntity.CellonOptions.Add(option);
                    }
                }

                if (necklace?.EquipmentOptions != null)
                {
                    foreach (EquipmentOptionDTO option in necklace?.EquipmentOptions)
                    {
                        BattleEntity.CellonOptions.Add(option);
                    }
                }

                var amulet =
                    Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Amulet, InventoryType.Wear);
                if (amulet != null && !Invisible && !InvisibleGm)
                {
                    if (amulet.ItemVNum == 4503 || amulet.ItemVNum == 4504)
                    {
                        Session.CurrentMapInstance?.Broadcast(
                            GenerateEff(
                                amulet.Item.EffectValue + (Class == ClassType.Adventurer ? 0 : (byte)Class - 1)),
                            PositionX, PositionY);
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(GenerateEff(amulet.Item.EffectValue), PositionX,
                            PositionY);
                    }
                }

                if (Group != null && (Group.GroupType == GroupType.Team || Group.GroupType == GroupType.BigTeam ||
                    Group.GroupType == GroupType.GiantTeam))
                {
                    Session?.CurrentMapInstance?.Broadcast(Session, GenerateEff(828 + (Group != null && Group.IsLeader(Session) ? 1 : 0)),
                        ReceiverType.AllExceptGroup);
                    Session?.CurrentMapInstance?.Broadcast(Session, GenerateEff(830 + (Group != null && Group.IsLeader(Session) ? 1 : 0)),
                        ReceiverType.Group);
                }

                Mates.Where(s => s.CanPickUp).ToList()
                    .ForEach(s => Session.CurrentMapInstance?.Broadcast(s.GenerateEff(3007)));
                LastEffect = DateTime.Now;
            }

            if (MeditationDictionary.Count != 0)
            {
                for (short i = 532; i < 535; i++)
                {
                    if (MeditationDictionary.ContainsKey(i) && MeditationDictionary[i] < DateTime.Now)
                    {
                        Session.SendPacket(GenerateEff(i == 535 ? 4344 : 4343));
                        RemoveBuff((short)(i == 532 ? 533 :
                            i == 533 ? 532 : 534));
                        RemoveBuff((short)(i == 532 ? 534 :
                            i == 533 ? 534 : 532));
                        if (i == 534)
                        {
                            RemoveBuff(532);
                            RemoveBuff(533);
                        }

                        AddBuff(new Buff.Buff(i, Level));
                        MeditationDictionary.Remove(i);
                    }
                }
            }

            if (SkillComboCount > 0 && LastSkillCombo.AddSeconds(10) < DateTime.Now)
            {
                SkillComboCount = 0;
                Session.SendPackets(GenerateQuicklist());
                Session.SendPacket("mslot 0 -1");
            }

            if (!UseSp || LastSkillUse.AddSeconds(15) < DateTime.Now ||
                LastSpGaugeRemove.AddSeconds(1) > DateTime.Now || SpInstance == null)
            {
                return;
            }

            byte spType = (byte)(SpInstance.Item.Morph > 1 && SpInstance.Item.Morph < 8 ||
                SpInstance.Item.Morph > 9 && SpInstance.Item.Morph < 16 ? 3 :
                SpInstance.Item.Morph > 16 && SpInstance.Item.Morph < 29 ? 2 :
                SpInstance.Item.Morph == 9 ? 1 : 0);

            if (Authority != AuthorityType.User)
            {
                return;
            }

            if (SpPoint >= spType)
            {
                SpPoint -= spType;
            }
            else if (SpPoint < spType && SpPoint != 0)
            {
                spType -= (byte)SpPoint;
                SpPoint = 0;
                SpAdditionPoint -= spType;
            }
            else
            {
                switch (SpPoint)
                {
                    case 0 when SpAdditionPoint >= spType:
                        SpAdditionPoint -= spType;
                        break;

                    case 0 when SpAdditionPoint < spType:
                        SpAdditionPoint = 0;

                        double currentRunningSeconds =
                            (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
                        LoadPassive();
                        LastSp = currentRunningSeconds;
                        if (Session?.HasSession == true)
                        {
                            if (IsVehicled)
                            {
                                return;
                            }

                            UseSp = false;
                            SpInstance = null;
                            CharacterHelper.Instance.RemoveSpecialistWingsBuff(Session);
                            LoadSpeed();
                            Session.SendPacket(GenerateCond());
                            Session.SendPacket(GenerateLev());
                            SpCooldown = 30;
                            foreach (CharacterSkill ski in SkillsSp?.Where(s => !s.Value?.CanBeUsed() == true)
                                .Select(s => s.Value))
                            {
                                double temp = (ski.LastUse - DateTime.Now).TotalMilliseconds + ski.Skill.Cooldown * 100;
                                temp /= 1000;
                                SpCooldown = temp > SpCooldown ? (int)temp : SpCooldown;
                            }

                            Session.SendPacket(GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("STAY_TIME"), SpCooldown), 11));
                            Session.SendPacket($"sd {SpCooldown}");
                            Session.CurrentMapInstance?.Broadcast(GenerateCMode());
                            Session.CurrentMapInstance?.Broadcast(
                                UserInterfaceHelper.Instance.GenerateGuri(6, 1, CharacterId), PositionX, PositionY);

                            // ms_c
                            Session.SendPacket(GenerateSki());
                            Session.SendPackets(GenerateQuicklist());
                            Session.SendPacket(GenerateStat());
                            Session.SendPacket(GenerateStatChar());
                            Observable.Timer(TimeSpan.FromMilliseconds(SpCooldown * 1000)).Subscribe(o =>
                            {
                                Session.SendPacket(
                                    GenerateSay(Language.Instance.GetMessageFromKey("TRANSFORM_DISAPPEAR"), 11));
                                Session.SendPacket("sd 0");
                            });
                        }

                        break;
                }
            }

            Session?.SendPacket(GenerateSpPoint());
            LastSpGaugeRemove = DateTime.Now;
        }

        public string GenerateTaFc(byte type) => $"ta_fc {type} {CharacterId}";

        public void CloseExchangeOrTrade()
        {
            if (!InExchangeOrTrade)
            {
                return;
            }

            long? targetSessionId = ExchangeInfo?.TargetCharacterId;

            if (!targetSessionId.HasValue || !Session.HasCurrentMapInstance)
            {
                return;
            }

            ClientSession targetSession = Session.CurrentMapInstance.GetSessionByCharacterId(targetSessionId.Value);

            if (targetSession == null)
            {
                return;
            }

            Session.SendPacket("exc_close 0");
            targetSession.SendPacket("exc_close 0");
            ExchangeInfo = null;
            targetSession.Character.ExchangeInfo = null;
        }

        public void CloseShop()
        {
            if (!HasShopOpened || !Session.HasCurrentMapInstance)
            {
                return;
            }

            KeyValuePair<long, MapShop> shop =
                Session.CurrentMapInstance.UserShops.FirstOrDefault(
                    mapshop => mapshop.Value.OwnerId.Equals(CharacterId));

            if (shop.Equals(default(KeyValuePair<long, MapShop>)))
            {
                return;
            }

            Session.CurrentMapInstance.UserShops.Remove(shop.Key);

            // declare that the shop cannot be closed
            HasShopOpened = false;

            Session.CurrentMapInstance?.Broadcast(GenerateShopEnd());
            Session.CurrentMapInstance?.Broadcast(Session, GeneratePlayerFlag(0), ReceiverType.AllExceptMe);
            IsSitting = false;
            IsShopping = false; // close shop by character will always completely close the shop

            LoadSpeed();
            Session.SendPacket(GenerateCond());
            Session.CurrentMapInstance?.Broadcast(GenerateRest());
        }

        public void Dance()
        {
            IsDancing = !IsDancing;
        }

        public Character DeepCopy()
        {
            return (Character)MemberwiseClone();
        }

        public void DeleteBlackList(long characterId)
        {
            CharacterRelationDTO chara = CharacterRelations.FirstOrDefault(s => s.RelatedCharacterId == characterId);
            if (chara == null)
            {
                return;
            }

            long id = chara.CharacterRelationId;
            DaoFactory.Instance.CharacterRelationDao.Delete(id);
            ServerManager.Instance.RelationRefresh(id);
            Session.SendPacket(GenerateBlinit());
        }

        public void DeleteItem(InventoryType type, short slot)
        {
            if (Inventory == null)
            {
                return;
            }

            Inventory.DeleteFromSlotAndType(slot, type);
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(type, slot));
        }

        public void DeleteItemByItemInstanceId(Guid id)
        {
            if (Inventory == null)
            {
                return;
            }

            Tuple<short, InventoryType> result = Inventory.DeleteById(id);
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInventoryRemove(result.Item2, result.Item1));
        }

        public void DeleteRelationById(long id)
        {
            DaoFactory.Instance.CharacterRelationDao.Delete(id);
            ServerManager.Instance.RelationRefresh(id);

            Session.SendPacket(GenerateFinit());

            List<CharacterRelationDTO> lst = ServerManager.Instance.CharacterRelations
                .Where(s => s.CharacterId == CharacterId || s.RelatedCharacterId == CharacterId).ToList();
            string result = "finit";
            foreach (CharacterRelationDTO relation in lst.Where(c => c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse))
            {
                long id2 = relation.RelatedCharacterId == CharacterId
                    ? relation.CharacterId
                    : relation.RelatedCharacterId;
                bool isOnline =
                    CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, id2);
                result +=
                    $" {id2}|{(short)relation.RelationType}|{(isOnline ? 1 : 0)}|{DaoFactory.Instance.CharacterDao.LoadById(id2).Name}";
            }

            Save();
        }

        public void DeleteRelation(CharacterRelationDTO relation)
        {
            if (relation == null)
            {
                return;
            }

            DeleteRelationById(relation.CharacterRelationId);
        }

        public void DeleteRelation(long characterId)
        {
            CharacterRelationDTO chara = CharacterRelations.FirstOrDefault(s =>
                s.RelatedCharacterId == characterId || s.CharacterId == characterId);
            if (chara == null)
            {
                return;
            }

            DeleteRelationById(chara.CharacterRelationId);
        }

        public void DeleteTimeout()
        {
            if (Inventory == null)
            {
                return;
            }

            foreach (ItemInstance item in Inventory.Select(s => s.Value))
            {
                if (!item.IsBound || item.ItemDeleteTime == null || !(item.ItemDeleteTime < DateTime.Now))
                {
                    continue;
                }

                Inventory.DeleteById(item.Id);
                BattleEntity.StaticBcards.RemoveWhere(o => o.ItemVNum != item.ItemVNum, out ConcurrentBag<BCard> tmp);
                BattleEntity.StaticBcards = tmp;
                Session.SendPacket(item.Type == InventoryType.Wear
                    ? GenerateEquipment()
                    : UserInterfaceHelper.Instance.GenerateInventoryRemove(item.Type, item.Slot));
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
            }
        }

        public void DisposeShopAndExchange()
        {
            CloseShop();
            CloseExchangeOrTrade();
        }

        /// <summary>
        ///     Destroy the character's related vars
        /// </summary>
        public void Dispose()
        {
            DisposeShopAndExchange();
            GroupSentRequestCharacterIds.Clear();
            FamilyInviteCharacters.Clear();
            FriendRequestCharacters.Clear();
            Session.Character?.Life?.Dispose();
        }

        public string GenerateAct() => "act6";

        public string GenerateAt()
        {
            MapInstance mapForMusic = MapInstance;

            //at 698495 20001 5 8 2 0 {SecondaryMusic} {SecondaryMusicType} -1
            return
                $"at {CharacterId} {MapInstance.Map.MapId} {PositionX} {PositionY} 2 0 {mapForMusic?.Map.Music ?? 0} -1";
        }

        public string GenerateBlinit()
        {
            return CharacterRelations
                .Where(s => s.CharacterId == CharacterId && s.RelationType == CharacterRelationType.Blocked).Aggregate(
                    "blinit",
                    (current, relation) =>
                        current +
                        $" {relation.RelatedCharacterId}|{DaoFactory.Instance.CharacterDao.LoadById(relation.RelatedCharacterId).Name}");
        }

        public string GenerateCInfo() =>
            $"c_info {(Authority == AuthorityType.Moderator && !Undercover ? $"[{Language.Instance.GetMessageFromKey("SUPPORT")}]" + Name : Name)} - -1 {(Family != null && !Undercover ? $"{Family.FamilyId} {Family.Name}({Language.Instance.GetMessageFromKey(FamilyCharacter.Authority.ToString().ToUpper() ?? "") ?? "-1 -"})" : "-1 -")} {CharacterId} {(Invisible ? 6 : Undercover ? (byte)AuthorityType.User : Authority < AuthorityType.User ? (byte)AuthorityType.User : Authority > AuthorityType.Moderator ? 2 : (byte)Authority)} {(byte)Gender} {(byte)HairStyle} {(byte)HairColor} {(byte)Class} {(GetDignityIco() == 1 ? GetReputIco() : -GetDignityIco())} {(Authority == AuthorityType.Moderator ? 500 : Compliment)} {(UseSp || IsVehicled ? Morph : 0)} {(Invisible ? 1 : 0)} {Family?.FamilyLevel ?? 0} {(UseSp ? MorphUpgrade : 0)} {ArenaWinner}";

        public string GenerateCMap() => $"c_map 0 {MapInstance.Map.MapId} {(MapInstance.MapInstanceType != MapInstanceType.BaseMapInstance ? 1 : 0)}";

        public string GenerateCMode() => $"c_mode 1 {CharacterId} {(UseSp || IsVehicled ? Morph : 0)} {(UseSp ? MorphUpgrade : 0)} {(UseSp ? MorphUpgrade2 : 0)} {ArenaWinner}";

        public string GenerateCond() =>
            $"cond 1 {CharacterId} {(HasBuff(BCardType.CardType.SpecialAttack, (byte)AdditionalTypes.SpecialAttack.NoAttack) ? 1 : 0)} {(HasBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible) ? 1 : 0)} {Speed}";

        public MapCell GetPos() => new MapCell { X = PositionX, Y = PositionY };

        public object GetSession() => this;

        public AttackType GetAttackType(Skill skill = null)
        {
            switch (skill?.Type)
            {
                case 0:
                case 1:
                case 2:
                    return (AttackType)skill.Type;

                case 3:
                case 5:
                    return (AttackType)(Class - 1 < 0 ? 0 : Class - 1);
            }

            return AttackType.Close;
        }

        public bool IsTargetable(SessionType type, bool isPvP = false) =>
            type != WingsEmu.Packets.Enums.SessionType.MateAndNpc && (type != WingsEmu.Packets.Enums.SessionType.Character || isPvP) &&
            Hp > 0 && !InvisibleGm && !Invisible;

        public Node[][] GetBrushFire() =>
            BestFirstSearch.LoadBrushFireJagged(new GridPos { X = PositionX, Y = PositionY }, MapInstance.Map.Grid);

        public SessionType SessionType() => WingsEmu.Packets.Enums.SessionType.Character;

        public long GetId() => CharacterId;

        public void GenerateDeath(IBattleEntity killer)
        {
            Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(o =>
            {
                ServerManager.Instance.AskRevive(CharacterId,
                    killer.GetSession() is Character character ? character.Session : null);
            });
        }

        public void GenerateRewards(IBattleEntity target)
        {
            if (target.GetSession() is MapMonster monster)
            {
                GenerateKillBonus(monster);
            }
        }

        public void GenerateDignity(NpcMonster monsterinfo)
        {
            if (Level >= monsterinfo.Level || !(Dignity < 100) || Level <= 20)
            {
                return;
            }

            Dignity += (float)0.5;
            if (Dignity != (int)Dignity)
            {
                return;
            }

            Session.SendPacket(GenerateFd());
            Session.CurrentMapInstance?.Broadcast(Session, this.GenerateIn(), ReceiverType.AllExceptMe);
            Session.CurrentMapInstance?.Broadcast(Session, this.GenerateGidx(), ReceiverType.AllExceptMe);
            Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("RESTORE_DIGNITY"), 11));
        }

        public string GenerateDir() => $"dir 1 {CharacterId} {Direction}";

        public EffectPacket GenerateEff(int effectid) => new EffectPacket
        {
            EffectType = 1,
            CharacterId = CharacterId,
            Id = effectid
        };

        public string GenerateEq()
        {
            int color = (byte)HairColor;
            var head = Inventory?.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Hat, InventoryType.Wear);

            if (head != null && head.Item.IsColored)
            {
                color = head.Design;
            }

            return
                $"eq {CharacterId} {(Invisible ? 6 : Undercover ? (byte)AuthorityType.User : Authority < AuthorityType.GameMaster ? 0 : 2)} {(byte)Gender} {(byte)HairStyle} {color} {(byte)Class} {GenerateEqListForPacket()} {(!InvisibleGm ? GenerateEqRareUpgradeForPacket() : null)}";
        }

        public string GenerateEqListForPacket()
        {
            string[] invarray = new string[16];
            if (Inventory == null)
            {
                return
                    $"{invarray[(byte)EquipmentType.Hat]}.{invarray[(byte)EquipmentType.Armor]}.{invarray[(byte)EquipmentType.MainWeapon]}.{invarray[(byte)EquipmentType.SecondaryWeapon]}.{invarray[(byte)EquipmentType.Mask]}.{invarray[(byte)EquipmentType.Fairy]}.{invarray[(byte)EquipmentType.CostumeSuit]}.{invarray[(byte)EquipmentType.CostumeHat]}.{invarray[(byte)EquipmentType.WeaponSkin]}";
            }

            for (short i = 0; i < 16; i++)
            {
                ItemInstance item = Inventory.LoadBySlotAndType(i, InventoryType.Wear);
                if (item != null)
                {
                    invarray[i] = item.ItemVNum.ToString();
                }
                else
                {
                    invarray[i] = "-1";
                }
            }

            return
                $"{invarray[(byte)EquipmentType.Hat]}.{invarray[(byte)EquipmentType.Armor]}.{invarray[(byte)EquipmentType.MainWeapon]}.{invarray[(byte)EquipmentType.SecondaryWeapon]}.{invarray[(byte)EquipmentType.Mask]}.{invarray[(byte)EquipmentType.Fairy]}.{invarray[(byte)EquipmentType.CostumeSuit]}.{invarray[(byte)EquipmentType.CostumeHat]}.{invarray[(byte)EquipmentType.WeaponSkin]}";
        }

        public string GenerateEqRareUpgradeForPacket()
        {
            sbyte weaponRare = 0;
            byte weaponUpgrade = 0;
            sbyte armorRare = 0;
            byte armorUpgrade = 0;
            if (Inventory == null)
            {
                return $"{weaponUpgrade}{weaponRare} {armorUpgrade}{armorRare}";
            }

            for (short i = 0; i < 15; i++)
            {
                var wearable = Inventory.LoadBySlotAndType<WearableInstance>(i, InventoryType.Wear);
                if (wearable == null)
                {
                    continue;
                }

                switch (wearable.Item.EquipmentSlot)
                {
                    case EquipmentType.Armor:
                        armorRare = wearable.Rare;
                        armorUpgrade = wearable.Upgrade;
                        break;

                    case EquipmentType.MainWeapon:
                        weaponRare = wearable.Rare;
                        weaponUpgrade = wearable.Upgrade;
                        break;
                }
            }

            return $"{weaponUpgrade}{weaponRare} {armorUpgrade}{armorRare}";
        }

        public string GenerateEquipment()
        {
            string eqlist = string.Empty;
            sbyte weaponRare = 0;
            byte weaponUpgrade = 0;
            sbyte armorRare = 0;
            byte armorUpgrade = 0;

            for (short i = 0; i < 16; i++)
            {
                if (Inventory == null)
                {
                    continue;
                }

                ItemInstance item = Inventory.LoadBySlotAndType<WearableInstance>(i, InventoryType.Wear) ??
                    Inventory.LoadBySlotAndType<SpecialistInstance>(i, InventoryType.Wear);
                if (item == null)
                {
                    continue;
                }

                switch (item.Item.ItemType)
                {
                    case ItemType.Armor:
                        armorRare = item.Rare;
                        armorUpgrade = item.Upgrade;
                        ShellOptionArmor.Clear();

                        foreach (EquipmentOptionDTO dto in DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(item.Id))
                        {
                            ShellOptionArmor.Add(dto);
                        }

                        break;
                    case ItemType.Weapon:

                        switch (item.Item.EquipmentSlot)
                        {
                            case EquipmentType.SecondaryWeapon:
                                ShellOptionsSecondary.Clear();

                                foreach (EquipmentOptionDTO dto in DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(item.Id))
                                {
                                    ShellOptionsSecondary.Add(dto);
                                }

                                break;

                            case EquipmentType.MainWeapon:
                                ShellOptionsMain.Clear();

                                foreach (EquipmentOptionDTO dto in DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(item.Id))
                                {
                                    ShellOptionsMain.Add(dto);
                                }

                                weaponRare = item.Rare;
                                weaponUpgrade = item.Upgrade;
                                break;
                        }

                        break;
                }

                eqlist += $" {i}.{item.Item.VNum}.{item.Rare}.{(item.Item.IsColored ? item.Design : item.Upgrade)}.0";
            }

            return $"equip {weaponUpgrade}{weaponRare} {armorUpgrade}{armorRare}{eqlist}";
        }

        public string GenerateExts() => $"exts 0 {48 + (HaveBackpack() ? 1 : 0) * 12} {48 + (HaveBackpack() ? 1 : 0) * 12} {48 + (HaveBackpack() ? 1 : 0) * 12}";

        public string GenerateFaction() => $"fs {(byte)Faction}";

        public string GenerateFamilyMember()
        {
            string str = "gmbr 0";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    foreach (FamilyCharacter targetCharacter in Family?.FamilyCharacters)
                    {
                        bool isOnline =
                            CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup,
                                targetCharacter.CharacterId);
                        str +=
                            $" {targetCharacter.Character.CharacterId}|{Family.FamilyId}|{targetCharacter.Character.Name}|{targetCharacter.Character.Level}|{(byte)targetCharacter.Character.Class}|{(byte)targetCharacter.Authority}|{(byte)targetCharacter.Rank}|{(isOnline ? 1 : 0)}|{targetCharacter.Character.HeroLevel}";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return str;
        }

        public string GenerateFamilyMemberExp()
        {
            string str = "gexp";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    str = (Family?.FamilyCharacters).Aggregate(str,
                        (current, targetCharacter) =>
                            current + $" {targetCharacter.CharacterId}|{targetCharacter.Experience}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return str;
        }

        public string GenerateFamilyMemberMessage()
        {
            string str = "gmsg";
            try
            {
                if (Family?.FamilyCharacters != null)
                {
                    str = (Family?.FamilyCharacters).Aggregate(str,
                        (current, targetCharacter) =>
                            current + $" {targetCharacter.CharacterId}|{targetCharacter.DailyMessage}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return str;
        }

        public List<string> GenerateFamilyWarehouseHist()
        {
            if (Family == null)
            {
                return new List<string>();
            }

            List<string> packetList = new List<string>();
            string packet = string.Empty;
            int i = 0;
            int amount = -1;
            foreach (FamilyLogDTO log in Family.FamilyLogs
                .Where(s => s.FamilyLogType == FamilyLogType.WareHouseAdded ||
                    s.FamilyLogType == FamilyLogType.WareHouseRemoved).OrderByDescending(s => s.Timestamp)
                .Take(100))
            {
                packet +=
                    $" {(log.FamilyLogType == FamilyLogType.WareHouseAdded ? 0 : 1)}|{log.FamilyLogData}|{(int)(DateTime.Now - log.Timestamp).TotalHours}";
                i++;
                if (i == 50)
                {
                    i = 0;
                    packetList.Add($"fslog_stc {amount}{packet}");
                    amount++;
                }
                else if (i == Family.FamilyLogs.Count)
                {
                    packetList.Add($"fslog_stc {amount}{packet}");
                }
            }

            return packetList;
        }

        public void GenerateFamilyXp(int fxp)
        {
            if (Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.BlockFExp && s.DateEnd > DateTime.Now))
            {
                return;
            }

            if (Family == null || FamilyCharacter == null)
            {
                return;
            }

            FamilyCharacterDTO famchar = FamilyCharacter;
            FamilyDTO fam = Family;
            fam.FamilyExperience += fxp;
            famchar.Experience += fxp;
            if (CharacterHelper.LoadFamilyXpData(Family.FamilyLevel) <= fam.FamilyExperience)
            {
                fam.FamilyExperience -= CharacterHelper.LoadFamilyXpData(Family.FamilyLevel);
                fam.FamilyLevel++;
                Family.InsertFamilyLog(FamilyLogType.FamilyLevelUp, level: fam.FamilyLevel);
                CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = Family.FamilyId,
                    SourceCharacterId = CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = UserInterfaceHelper.Instance.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("FAMILY_UP")), 0),
                    Type = MessageType.Family
                });
            }

            DaoFactory.Instance.FamilyCharacterDao.InsertOrUpdate(ref famchar);
            DaoFactory.Instance.FamilyDao.InsertOrUpdate(ref fam);
            ServerManager.Instance.FamilyRefresh(Family.FamilyId);
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = Family.FamilyId,
                SourceCharacterId = CharacterId,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "fhis_stc",
                Type = MessageType.Family
            });
        }

        public string GenerateFd() => $"fd {Reput} {GetReputIco()} {(int)Dignity} {Math.Abs(GetDignityIco())}";

        public string GenerateFinfo(long? relatedCharacterLoggedId, bool isConnected)
        {
            return CharacterRelations.Where(c => c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse)
                .Where(relation => relatedCharacterLoggedId.HasValue &&
                    (relatedCharacterLoggedId.Value == relation.RelatedCharacterId ||
                        relatedCharacterLoggedId.Value == relation.CharacterId))
                .Aggregate("finfo",
                    (current, relation) => current + $" {relation.RelatedCharacterId}.{(isConnected ? 1 : 0)}");
        }

        public string GenerateFinit()
        {
            string result = "finit";
            foreach (CharacterRelationDTO relation in CharacterRelations.Where(c =>
                c.RelationType == CharacterRelationType.Friend || c.RelationType == CharacterRelationType.Spouse))
            {
                long id = relation.RelatedCharacterId == CharacterId
                    ? relation.CharacterId
                    : relation.RelatedCharacterId;
                bool isOnline =
                    CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup, id);
                result +=
                    $" {id}|{(short)relation.RelationType}|{(isOnline ? 1 : 0)}|{DaoFactory.Instance.CharacterDao.LoadById(id).Name}";
            }

            return result;
        }

        public string GenerateFStashAll()
        {
            string stash = $"f_stash_all {Family.WarehouseSize}";
            return Family.Warehouse.Select(s => s.Value)
                .Aggregate(stash, (current, item) => current + $" {item.GenerateStashPacket()}");
        }

        public string GenerateGender() => $"p_sex {(byte)Gender}";

        public string GenerateGet(long id) => $"get 1 {CharacterId} {id} 0";

        public string GenerateGExp()
        {
            return Family.FamilyCharacters.Aggregate("gexp",
                (current, familyCharacter) => current + $" {familyCharacter.CharacterId}|{familyCharacter.Experience}");
        }

        public string GenerateGInfo()
        {
            if (Family == null)
            {
                return string.Empty;
            }

            try
            {
                FamilyCharacter familyCharacter =
                    Family.FamilyCharacters.FirstOrDefault(s => s.Authority == FamilyAuthority.Head);
                if (familyCharacter != null)
                {
                    return
                        $"ginfo {Family.Name} {familyCharacter.Character.Name} {(byte)Family.FamilyHeadGender} {Family.FamilyLevel} {Family.FamilyExperience} {CharacterHelper.LoadFamilyXpData(Family.FamilyLevel)} {Family.FamilyCharacters.Count} {Family.MaxSize} {(byte)FamilyCharacter.Authority} {(Family.ManagerCanInvite ? 1 : 0)} {(Family.ManagerCanNotice ? 1 : 0)} {(Family.ManagerCanShout ? 1 : 0)} {(Family.ManagerCanGetHistory ? 1 : 0)} {(byte)Family.ManagerAuthorityType} {(Family.MemberCanGetHistory ? 1 : 0)} {(byte)Family.MemberAuthorityType} {Family.FamilyMessage.Replace(' ', '^')}";
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        public string GenerateGold() => $"gold {Gold} {Session.Account.BankMoney / 100}";

        public string GenerateIcon(int v1, int v2, short itemVNum) => $"icon {v1} {CharacterId} {v2} {itemVNum}";

        public string GenerateIdentity() => $"Character: {Name}";

        public string GenerateInvisible() => $"cl {CharacterId} {(Invisible ? 1 : 0)} {(InvisibleGm ? 1 : 0)}";

        public void GenerateKillBonus(MapMonster monsterToAttack)
        {
            lock(_syncObj)
            {
                if (monsterToAttack == null || monsterToAttack.IsAlive)
                {
                    return;
                }

                var random = new Random(DateTime.Now.Millisecond & monsterToAttack.MapMonsterId);

                // owner set
                long? dropOwner = monsterToAttack.DamageList.Count > 0
                    ? monsterToAttack.DamageList.First().Key.GetSession() is Mate mate ? mate.Owner.CharacterId :
                    monsterToAttack.DamageList.First().Key.GetId()
                    : (long?)null;

                Group group = null;
                if (dropOwner != null)
                {
                    group = ServerManager.Instance.Groups.FirstOrDefault(g =>
                        g.IsMemberOfGroup((long)dropOwner) && g.GroupType == GroupType.Group);
                }

                IncrementQuests(QuestType.Hunt, monsterToAttack.MonsterVNum);

                // end owner set
                if (!Session.HasCurrentMapInstance || monsterToAttack.Monster.MonsterType == MonsterType.Special)
                {
                    return;
                }

                List<DropDTO> droplist = monsterToAttack.Monster.Drops.Where(s =>
                        Session.CurrentMapInstance.Map.MapTypes.Any(m => m.MapTypeId == s.MapTypeId) ||
                        s.MapTypeId == null)
                    .ToList();

                #region Partners

                Mate partnerInTeam = Mates.FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Partner);
                Mate mateInTeam = Mates.FirstOrDefault(s => s.IsTeamMember && s.MateType == MateType.Pet);
                if (Level < monsterToAttack.Monster.Level + 15)
                {
                    if (partnerInTeam?.SpInstance != null && partnerInTeam.SpInstance.Agility < 100)
                    {
                        partnerInTeam.SpInstance.Agility = (byte)(partnerInTeam.SpInstance.Agility + 2 > 100 ? 100 : partnerInTeam.SpInstance.Agility + 2);
                        Session.SendPacket(partnerInTeam.GenerateScPacket());
                        if (partnerInTeam.SpInstance.Agility == 100)
                        {
                            Session.SendPacket(GenerateSay("L'adresse de ton partenaire a atteint les 100%", 10));
                        }
                    }
                }
                else if (Level < monsterToAttack.Monster.Level + 30)
                {
                    if (partnerInTeam?.SpInstance != null && partnerInTeam.SpInstance.Agility < 100)
                    {
                        partnerInTeam.SpInstance.Agility += 1;
                        Session.SendPacket(partnerInTeam.GenerateScPacket());
                        if (partnerInTeam.SpInstance.Agility == 100)
                        {
                            Session.SendPacket(GenerateSay("L'adresse de ton partenaire a atteint les 100%", 10));
                        }
                    }
                }

                if (mateInTeam != null)
                {
                    if (monsterToAttack.IsMateTrainer)
                    {
                        int probaLevelUp = ServerManager.Instance.RandomNumber();

                        if (probaLevelUp > 50)
                        {
                            // Upgrade 
                            probaLevelUp = ServerManager.Instance.RandomNumber();
                            if (probaLevelUp > 50)
                            {
                                // Weapon
                                probaLevelUp = ServerManager.Instance.RandomNumber();
                                if (probaLevelUp > 50 && mateInTeam.Attack < 10)
                                {
                                    // Upgrade
                                    mateInTeam.Attack++;
                                    Session.SendPacket(Session.Character.GenerateSay("the attack level of the mate increased", 10));
                                }
                            }
                            else
                            {
                                // Armor
                                probaLevelUp = ServerManager.Instance.RandomNumber();
                                if (probaLevelUp > 50 && mateInTeam.Defence < 10)
                                {
                                    mateInTeam.Defence++;
                                    Session.SendPacket(Session.Character.GenerateSay("the defence level of the mate increased", 10));
                                }
                            }
                        }
                        else
                        {
                            // Downgrade
                            probaLevelUp = ServerManager.Instance.RandomNumber();
                            if (probaLevelUp > 50)
                            {
                                // Weapon
                                probaLevelUp = ServerManager.Instance.RandomNumber();
                                if (probaLevelUp > 50 && mateInTeam.Attack > 0)
                                {
                                    // Downgrade
                                    mateInTeam.Attack--;
                                    Session.SendPacket(Session.Character.GenerateSay("the attack level of the mate decreased", 10));
                                }
                            }
                            else
                            {
                                // Armor
                                probaLevelUp = ServerManager.Instance.RandomNumber();
                                if (probaLevelUp > 50 && mateInTeam.Defence > 0)
                                {
                                    //DownGrade
                                    mateInTeam.Defence--;
                                    Session.SendPacket(Session.Character.GenerateSay("the defence level of the mate decreased", 10));
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Quest

                Quests.Where(q =>
                        q.Quest?.QuestType == (int)QuestType.Collect4 ||
                        q.Quest?.QuestType == (int)QuestType.Collect2)
                    .ToList().ForEach(qst =>
                    {
                        qst.Quest.QuestObjectives.ForEach(d =>
                        {
                            if (d.SpecialData == monsterToAttack.MonsterVNum)
                            {
                                droplist.Add(new DropDTO
                                {
                                    ItemVNum = (short)d.Data,
                                    Amount = 1,
                                    MonsterVNum = monsterToAttack.MonsterVNum,
                                    DropChance =
                                        (d.DropRate ?? 100) * 10 *
                                        ServerManager.Instance.QuestDropRate // Approx
                                });
                            }
                        });
                    });

                IncrementQuests(QuestType.FlowerQuest, monsterToAttack.Monster.Level);

                #endregion

                #region exp

                if (Hp <= 0)
                {
                    return;
                }

                Group grp = ServerManager.Instance.Groups.FirstOrDefault(g =>
                    g.IsMemberOfGroup(CharacterId) && g.GroupType == GroupType.Group);
                if (grp != null)
                {
                    foreach (ClientSession targetSession in grp.Characters.Where(g =>
                        g.Character.MapInstanceId == MapInstanceId))
                    {
                        if (grp.IsMemberOfGroup(monsterToAttack.DamageList.FirstOrDefault().Key == null ? CharacterId : monsterToAttack.DamageList.FirstOrDefault().Key.GetId()))
                        {
                            targetSession.Character.GenerateXp(monsterToAttack, true);
                        }
                        else
                        {
                            targetSession.SendPacket(
                                targetSession.Character.GenerateSay(
                                    Language.Instance.GetMessageFromKey("XP_NOTFIRSTHIT"), 10));
                            targetSession.Character.GenerateXp(monsterToAttack, false);
                        }
                    }
                }
                else
                {
                    GenerateXp(monsterToAttack, true);
                }

                // TODO ADD A CONFIGURATION FOR THAT
                if (Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance &&
                    ServerManager.Instance.ReputOnMonsters)
                {
                    GetReput(monsterToAttack.Monster.Level / 3);
                }

                GenerateDignity(monsterToAttack.Monster);

                #endregion

                #region Act4

                if (monsterToAttack.MapInstance?.MapInstanceType == MapInstanceType.Act4Instance)
                {
                    if (ServerManager.Instance.Act4AngelStat.Mode == 0 &&
                        ServerManager.Instance.Act4DemonStat.Mode == 0)
                    {
                        switch (Faction)
                        {
                            case FactionType.Angel:
                                ServerManager.Instance.Act4AngelStat.Percentage += 10000 / (ServerManager.Instance.GlacernonPercentRatePvm * 100);
                                break;
                            case FactionType.Demon:
                                ServerManager.Instance.Act4DemonStat.Percentage += 10000 / (ServerManager.Instance.GlacernonPercentRatePvm * 100);
                                break;
                        }

                        ServerManager.Instance.Act4Process();
                    }
                }

                #endregion

                #region Act6Stats

                if (monsterToAttack.MapInstance?.Map.MapId >= 229 && monsterToAttack.MapInstance?.Map.MapId <= 232 &&
                    ServerManager.Instance.Act6Zenas.Mode == 0)
                {
                    ServerManager.Instance.Act6Zenas.Percentage += 1000 / (ServerManager.Instance.CylloanPercentRate * 100);
                    ServerManager.Instance.Act6Process();
                }

                if (monsterToAttack.MapInstance?.Map.MapId >= 233 &&
                    monsterToAttack.MapInstance?.Map.MapId <= 236 ||
                    monsterToAttack.MapInstance?.Map.MapId == 2604 &&
                    ServerManager.Instance.Act6Erenia.Mode == 0)
                {
                    ServerManager.Instance.Act6Erenia.Percentage += 1000 / (ServerManager.Instance.CylloanPercentRate * 100);
                    ServerManager.Instance.Act6Process();
                }

                #endregion Act6Stats

                #region item drop

                int dropRate = ServerManager.Instance.DropRate * MapInstance.DropRate;
                int x = 0;
                foreach (DropDTO drop in droplist.OrderBy(s => random.Next()))
                {
                    if (x >= 4)
                    {
                        continue;
                    }

                    double rndamount = ServerManager.Instance.RandomNumber() * random.NextDouble();
                    if (!(rndamount <= (double)drop.DropChance * dropRate / 5000d))
                    {
                        continue;
                    }

                    x++;
                    if (Session.CurrentMapInstance == null)
                    {
                        continue;
                    }

                    if (Session.CurrentMapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act4) ||
                        Session.CurrentMapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act42) ||
                        monsterToAttack.Monster.MonsterType == MonsterType.Elite)
                    {
                        List<long> alreadyGifted = new List<long>();
                        foreach (IBattleEntity entity in monsterToAttack.DamageList.Keys)
                        {
                            long charId = entity.GetId();
                            if (alreadyGifted.Contains(charId))
                            {
                                continue;
                            }

                            ClientSession giftsession = ServerManager.Instance.GetSessionByCharacterId(charId);
                            giftsession?.Character.GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                            alreadyGifted.Add(charId);
                        }
                    }
                    else
                    {
                        if (group != null && group.GroupType == GroupType.Group)
                        {
                            if (group.SharingMode == (byte)GroupSharingType.ByOrder)
                            {
                                dropOwner = group.GetNextOrderedCharacterId(this);
                                if (dropOwner.HasValue)
                                {
                                    group.Characters.ToList()
                                        .ForEach(s => s.SendPacket(s.Character.GenerateSay(
                                            string.Format(Language.Instance.GetMessageFromKey("ITEM_BOUND_TO"),
                                                ServerManager.Instance.GetItem(drop.ItemVNum).Name,
                                                group.Characters.FirstOrDefault(c => c.Character.CharacterId == dropOwner)
                                                    ?.Character.Name, drop.Amount), 10)));
                                }
                            }
                            else
                            {
                                group.Characters.ToList()
                                    .ForEach(s => s.SendPacket(s.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("DROPPED_ITEM"),
                                            ServerManager.Instance.GetItem(drop.ItemVNum).Name, drop.Amount), 10)));
                            }
                        }

                        long? owner = dropOwner;
                        if (!ServerManager.Instance.AutoLoot)
                        {
                            Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(o =>
                            {
                                if (Session.HasCurrentMapInstance)
                                {
                                    Session.CurrentMapInstance.DropItemByMonster(owner, drop, monsterToAttack.MapX,
                                        monsterToAttack.MapY,
                                        Quests.Any(q =>
                                            (q.Quest.QuestType == (int)QuestType.Collect4 ||
                                                q.Quest.QuestType == (int)QuestType.Collect2) &&
                                            q.Quest.QuestObjectives.Any(qst => qst.Data == drop.ItemVNum)));
                                }
                            });
                        }
                        else
                        {
                            if (!Session.HasCurrentMapInstance)
                            {
                                continue;
                            }

                            Session.Character.GiftAdd(drop.ItemVNum, (byte)drop.Amount);
                            if (!Quests.Any(q =>
                                (q.Quest.QuestType == (int)QuestType.Collect4 || q.Quest.QuestType == (int)QuestType.Collect2) && q.Quest.QuestObjectives.Any(qst => qst.Data == drop.ItemVNum)))
                            {
                                continue;
                            }

                            IncrementQuests(QuestType.Collect4, drop.ItemVNum);
                            IncrementQuests(QuestType.Collect2, drop.ItemVNum);
                        }
                    }
                }

                #endregion

                #region gold drop

                // gold calculation
                double increase = Authority >= AuthorityType.Donator ? ((double)Authority - 5) / 100D + 1 : 1;
                int gold = (int)(GetGold(monsterToAttack) * increase);
                long maxGold = ServerManager.Instance.MaxGold;
                gold = gold > maxGold ? (int)maxGold : gold;
                double randChance = ServerManager.Instance.RandomNumber() * random.NextDouble();

                if (gold <= 0 || !(randChance <= ServerManager.Instance.GoldDropRate) ||
                    Session.CurrentMapInstance?.MapInstanceType == MapInstanceType.LodInstance)
                {
                    return;
                }

                if (Session.CurrentMapInstance == null)
                {
                    return;
                }

                var drop2 = new DropDTO
                {
                    Amount = gold,
                    ItemVNum = 1046
                };
                if (Session.CurrentMapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act4) ||
                    Session.CurrentMapInstance.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act42) ||
                    monsterToAttack.Monster.MonsterType == MonsterType.Elite)
                {
                    List<long> alreadyGifted = new List<long>();
                    foreach (IBattleEntity entity in monsterToAttack.DamageList.Keys)
                    {
                        long charId = entity.GetId();
                        if (alreadyGifted.Contains(charId))
                        {
                            continue;
                        }

                        ClientSession session = ServerManager.Instance.GetSessionByCharacterId(charId);
                        session?.Character.GetGold((int)(drop2.Amount *
                            (1 + GetBuff(BCardType.CardType.Item,
                                    (byte)AdditionalTypes.Item.IncreaseEarnedGold)[0] /
                                100D)));
                        alreadyGifted.Add(charId);
                    }
                }
                else
                {
                    if (group != null)
                    {
                        if (group.SharingMode == (byte)GroupSharingType.ByOrder)
                        {
                            dropOwner = group.GetNextOrderedCharacterId(this);

                            if (dropOwner.HasValue)
                            {
                                group.Characters.ToList().ForEach(s =>
                                    s.SendPacket(s.Character.GenerateSay(
                                        string.Format(Language.Instance.GetMessageFromKey("ITEM_BOUND_TO"),
                                            ServerManager.Instance.GetItem(drop2.ItemVNum).Name,
                                            group.Characters.FirstOrDefault(c => c.Character.CharacterId == (long)dropOwner)
                                                ?.Character.Name, drop2.Amount), 10)));
                            }
                        }
                        else
                        {
                            group.Characters.ToList()
                                .ForEach(s => s.SendPacket(s.Character.GenerateSay(
                                    string.Format(Language.Instance.GetMessageFromKey("DROPPED_ITEM"),
                                        ServerManager.Instance.GetItem(drop2.ItemVNum).Name, drop2.Amount), 10)));
                        }
                    }

                    if (!ServerManager.Instance.AutoLoot)
                    {
                        // delayed Drop if not a meteor
                        if (monsterToAttack.MonsterVNum != 2352)
                        {
                            Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(o =>
                            {
                                if (Session.HasCurrentMapInstance)
                                {
                                    Session.CurrentMapInstance.DropItemByMonster(dropOwner, drop2, monsterToAttack.MapX,
                                        monsterToAttack.MapY);
                                }
                            });
                        }
                    }
                    else
                    {
                        if (Session.HasCurrentMapInstance)
                        {
                            Session?.Character.GetGold((int)(drop2.Amount *
                                (1 + GetBuff(BCardType.CardType.Item,
                                        (byte)AdditionalTypes.Item.IncreaseEarnedGold)[0] /
                                    100D)));
                        }
                    }
                }

                #endregion
            }
        }

        public void GenerateMail(MailDTO mail)
        {
            MailList.Add((MailList.Count > 0 ? MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mail);
            if (!mail.IsSenderCopy && mail.ReceiverId == CharacterId)
            {
                Session.SendPacket(mail.AttachmentVNum != null ? GenerateParcel(mail) : GeneratePost(mail, 1));
            }
            else
            {
                Session.SendPacket(GeneratePost(mail, 2));
            }
        }

        public string GenerateLev() => Level < 100
            ? $"lev {Level} {LevelXp} {(!UseSp || SpInstance == null ? JobLevel : SpInstance.SpLevel)} {(!UseSp || SpInstance == null ? JobLevelXp : SpInstance.XP)} {XpLoad()} {(!UseSp || SpInstance == null ? JobXpLoad() : SpxpLoad())} {Reput} {GetCp()} {HeroXp} {HeroLevel} {HeroXpLoad()} {0}"
            : $"lev {Level} {(short)(LevelXp / XpLoad() * 10000)} {(!UseSp || SpInstance == null ? JobLevel : SpInstance.SpLevel)} {(!UseSp || SpInstance == null ? JobLevelXp : SpInstance.XP)} {10000} {(!UseSp || SpInstance == null ? JobXpLoad() : SpxpLoad())} {Reput} {GetCp()} {HeroXp} {HeroLevel} {HeroXpLoad()}";

        public string GenerateLevelUp() => $"levelup {CharacterId}";

        public void GenerateMiniland()
        {
            if (Miniland != null)
            {
                return;
            }

            Miniland = ServerManager.Instance.GenerateMapInstance(20001, MapInstanceType.NormalInstance,
                new InstanceBag());
            foreach (MinilandObjectDTO obj in DaoFactory.Instance.MinilandObjectDao.LoadByCharacterId(CharacterId))
            {
                var mapobj = (MapDesignObject)obj;
                if (mapobj.ItemInstanceId == null)
                {
                    continue;
                }

                var item = Inventory.LoadByItemInstance<ItemInstance>((Guid)mapobj.ItemInstanceId);
                if (item == null)
                {
                    continue;
                }

                mapobj.ItemInstance = item;
                Miniland.MapDesignObjects.Add(mapobj);
            }
        }

        public string GenerateMinilandPoint() => $"mlpt {MinilandPoint} 100";

        public string GenerateMinimapPosition()
        {
            if (MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance ||
                MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
            {
                return $"rsfp {MapInstance.MapIndexX} {MapInstance.MapIndexY}";
            }

            return "rsfp 0 -1";
        }

        public string GenerateMlinfo()
        {
            return
                $"mlinfo 3800 {MinilandPoint} 100 {GeneralLogs.Count(s => s.LogData == "Miniland" && s.Timestamp.Day == DateTime.Now.Day)} {GeneralLogs.Count(s => s.LogData == "Miniland")} 10 {(byte)MinilandState} {Language.Instance.GetMessageFromKey("WELCOME_MUSIC_INFO")} {Language.Instance.GetMessageFromKey("MINILAND_WELCOME_MESSAGE")}";
        }

        public string GenerateMlinfobr()
        {
            return
                $"mlinfobr 3800 {Name} {GeneralLogs.Count(s => s.LogData == "Miniland" && s.Timestamp.Day == DateTime.Now.Day)} {GeneralLogs.Count(s => s.LogData == "Miniland")} 25 {MinilandMessage.Replace(' ', '^')}";
        }

        public string GenerateMloMg(MapDesignObject mlobj, MinigamePacket packet) =>
            $"mlo_mg {packet.MinigameVNum} {MinilandPoint} 0 0 {mlobj.ItemInstance.DurabilityPoint} {mlobj.ItemInstance.Item.MinilandObjectPoint}";

        public MovePacket GenerateMv() => new MovePacket
        {
            CharacterId = CharacterId,
            MapX = PositionX,
            MapY = PositionY,
            Speed = Speed,
            MoveType = 1
        };

        public string GenerateNpcDialog(int value) => $"npc_req 1 {CharacterId} {value}";

        public string GenerateOut() => $"out 1 {CharacterId}";

        public string GeneratePairy()
        {
            // FAIRY BUFF CARD ID
            bool isBuffed = Buff.Any(b => b.Card.CardId == 131);

            WearableInstance fairy = null;
            if (Inventory != null)
            {
                fairy = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Fairy, InventoryType.Wear);
            }

            ElementRate = CharacterHelper.Instance.ElementRate(Class, Level);
            Element = 0;
            if (fairy == null)
            {
                return $"pairy 1 {CharacterId} 0 0 0 0";
            }

            ElementRate += fairy.ElementRate + fairy.Item.ElementRate + (isBuffed ? 30 : 0);
            Element = fairy.Item.Element;
            return
                $"pairy 1 {CharacterId} 4 {fairy.Item.Element} {fairy.ElementRate + fairy.Item.ElementRate} {fairy.Item.Morph + (isBuffed ? 5 : 0)}";
        }

        private string GenerateParcel(MailDTO mail)
        {
            return mail.AttachmentVNum != null
                ? $"parcel 1 1 {MailList.First(s => s.Value.MailId == mail.MailId).Key} {(mail.Title == "NOSMALL" ? 1 : 4)} 0 {mail.Date.ToString("yyMMddHHmm")} {mail.Title} {mail.AttachmentVNum} {mail.AttachmentAmount} {(byte)ServerManager.Instance.GetItem((short)mail.AttachmentVNum).Type}"
                : string.Empty;
        }

        public string GeneratePidx(bool isLeaveGroup = false)
        {
            if (isLeaveGroup || Group == null)
            {
                return $"pidx -1 1.{CharacterId}";
            }

            string str = $"pidx {Group.GroupId}";
            return Group.Characters.Where(s => s.Character != null).Aggregate(str,
                (current, s) =>
                    current + $" {(Group.IsMemberOfGroup(CharacterId) ? 1 : 0)}.{s.Character.CharacterId} ");
        }

        public string GeneratePinit()
        {
            Group grp = ServerManager.Instance.Groups.FirstOrDefault(s =>
                s.IsMemberOfGroup(CharacterId) && s.GroupType == GroupType.Group);
            List<Mate> mates = Mates;
            int i = 0;
            string str = string.Empty;
            if (mates != null)
            {
                foreach (Mate mate in mates.Where(s => s.IsTeamMember).OrderBy(s => s.MateType))
                {
                    i++;
                    str +=
                        $" 2|{mate.MateTransportId}|{(int)mate.MateType}|{mate.Level}|{mate.Name.Replace(' ', '^')}|-1|{(mate.IsUsingSp && mate.SpInstance != null ? mate.SpInstance.Item.Morph : mate.Monster.NpcMonsterVNum)}|0";
                }
            }

            if (grp == null)
            {
                return $"pinit {i}{str}";
            }

            foreach (ClientSession groupSessionForId in grp.Characters)
            {
                i++;
                str +=
                    $" 1|{groupSessionForId.Character.CharacterId}|{i}|{groupSessionForId.Character.Level}|{groupSessionForId.Character.Name}|0|{(byte)groupSessionForId.Character.Gender}|{(byte)groupSessionForId.Character.Class}|{(groupSessionForId.Character.UseSp ? groupSessionForId.Character.Morph : 0)}|{groupSessionForId.Character.HeroLevel}";
            }

            return $"pinit {i}{str}";
        }

        public string GeneratePlayerFlag(long pflag) => $"pflag 1 {CharacterId} {pflag}";

        public string GeneratePost(MailDTO mail, byte type)
        {
            return
                $"post 1 {type} {MailList.First(s => s.Value.MailId == mail.MailId).Key} 0 {(mail.IsOpened ? 1 : 0)} {mail.Date.ToString("yyMMddHHmm")} {(type == 2 ? DaoFactory.Instance.CharacterDao.LoadById(mail.ReceiverId).Name : DaoFactory.Instance.CharacterDao.LoadById(mail.SenderId).Name)} {mail.Title}";
        }

        public string GeneratePostMessage(MailDTO mailDto, byte type)
        {
            CharacterDTO sender = DaoFactory.Instance.CharacterDao.LoadById(mailDto.SenderId);

            return
                $"post 5 {type} {MailList.First(s => s.Value == mailDto).Key} 0 0 {(byte)mailDto.SenderClass} {(byte)mailDto.SenderGender} {mailDto.SenderMorphId} {(byte)mailDto.SenderHairStyle} {(byte)mailDto.SenderHairColor} {mailDto.EqPacket} {sender.Name} {mailDto.Title} {mailDto.Message}";
        }

        public string GeneratePStashAll()
        {
            string stash =
                $"pstash_all {(StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBackPack || Authority >= AuthorityType.VipPlusPlus) ? 50 : 0)}";
            return Inventory.Select(s => s.Value).Where(s => s.Type == InventoryType.PetWarehouse)
                .Aggregate(stash, (current, item) => current + $" {item.GenerateStashPacket()}");
        }

        public IEnumerable<string> GenerateQuicklist()
        {
            string[] pktQs = { "qslot 0", "qslot 1" };

            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    QuicklistEntryDTO qi =
                        QuicklistEntries.FirstOrDefault(n => n.Q1 == j && n.Q2 == i && n.Morph == (UseSp ? Morph : 0));
                    pktQs[j] += $" {qi?.Type ?? 7}.{qi?.Slot ?? 7}.{qi?.Pos.ToString() ?? "-1"}";
                }
            }

            return pktQs;
        }

        public string GenerateRc(int characterHealth) => $"rc 1 {CharacterId} {characterHealth} 0";

        public string GenerateRcsList(CsListPacket packet)
        {
            string list = string.Empty;
            BazaarItemLink[] billist = new BazaarItemLink[ServerManager.Instance.BazaarList.Count + 20];
            ServerManager.Instance.BazaarList.CopyTo(billist);
            foreach (BazaarItemLink bz in billist.Where(s => s != null && s.BazaarItem.SellerId == CharacterId)
                .Skip(packet.Index * 50).Take(50))
            {
                if (bz.Item == null)
                {
                    continue;
                }

                int soldedAmount = bz.BazaarItem.Amount - bz.Item.Amount;
                int amount = bz.BazaarItem.Amount;
                bool package = bz.BazaarItem.IsPackage;
                bool isNosbazar = bz.BazaarItem.MedalUsed;
                long price = bz.BazaarItem.Price;
                long minutesLeft = (long)(bz.BazaarItem.DateStart.AddHours(bz.BazaarItem.Duration) - DateTime.Now)
                    .TotalMinutes;
                byte status = minutesLeft >= 0
                    ? soldedAmount < amount ? (byte)BazaarType.OnSale : (byte)BazaarType.Solded
                    : (byte)BazaarType.DelayExpired;
                if (status == (byte)BazaarType.DelayExpired)
                {
                    minutesLeft =
                        (long)(bz.BazaarItem.DateStart.AddHours(bz.BazaarItem.Duration).AddDays(isNosbazar ? 30 : 7) -
                            DateTime.Now).TotalMinutes;
                }

                string info = string.Empty;
                if (bz.Item.Item.Type == InventoryType.Equipment)
                {
                    if (bz.Item is WearableInstance item)
                    {
                        item.EquipmentOptions.Clear();
                        item.EquipmentOptions.AddRange(
                            DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(item.Id));
                        info = item.GenerateEInfo().Replace(' ', '^').Replace("e_info^", "");
                    }
                }

                if (packet.Filter == 0 || packet.Filter == status)
                {
                    list +=
                        $"{bz.BazaarItem.BazaarItemId}|{bz.BazaarItem.SellerId}|{bz.Item.ItemVNum}|{soldedAmount}|{amount}|{(package ? 1 : 0)}|{price}|{status}|{minutesLeft}|{(isNosbazar ? 1 : 0)}|0|{bz.Item.Rare}|{bz.Item.Upgrade}|{info} ";
                }
            }

            return $"rc_slist {packet.Index} {list}";
        }

        public string GenerateReqInfo()
        {
            WearableInstance fairy = null;
            if (Inventory != null)
            {
                fairy = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Fairy, InventoryType.Wear);
            }

            bool isPvpPrimary = false;
            bool isPvpSecondary = false;
            bool isPvpArmor = false;

            if (Inventory?.PrimaryWeapon?.Item.Name.Contains(": ") == true)
            {
                isPvpPrimary = true;
            }

            if (Inventory?.SecondaryWeapon?.Item.Name.Contains(": ") == true)
            {
                isPvpSecondary = true;
            }

            if (Inventory?.Armor?.Item.Name.Contains(": ") == true)
            {
                isPvpArmor = true;
            }

            // tc_info 0 name 0 0 0 0 -1 - 0 0 0 0 0 0 0 0 0 0 0 wins deaths reput 0 0 0 morph
            // talentwin talentlose capitul rankingpoints arenapoints 0 0 ispvpprimary ispvpsecondary
            // ispvparmor herolvl desc
            return
                $"tc_info {Level} {Name} {fairy?.Item.Element ?? 0} {ElementRate + (Buff.Any(s => s.Card.CardId == 131) ? 30 : 0)} {(byte)Class} {(byte)Gender} {(Family != null ? $"{Family.FamilyId} {Family.Name}({Language.Instance.GetMessageFromKey(FamilyCharacter?.Authority.ToString()?.ToUpper() ?? "")})" : "-1 -")} {GetReputIco()} {GetDignityIco()} {(Inventory.PrimaryWeapon != null ? 1 : 0)} {Inventory.PrimaryWeapon?.Rare ?? 0} {Inventory.PrimaryWeapon?.Upgrade ?? 0} {(Inventory.SecondaryWeapon != null ? 1 : 0)} {Inventory.SecondaryWeapon?.Rare ?? 0} {Inventory.SecondaryWeapon?.Upgrade ?? 0} {(Inventory.Armor != null ? 1 : 0)} {Inventory.Armor?.Rare ?? 0} {Inventory.Armor?.Upgrade ?? 0} {Act4Kill} {Act4Dead} {Reput} {Act4Kill} {TalentWin} {TalentLose} {(UseSp ? Morph : 0)} {TalentWin} {TalentLose} {TalentSurrender} 0 {MasterPoints} {Compliment} 0 {(isPvpPrimary ? 1 : 0)} {(isPvpSecondary ? 1 : 0)} {(isPvpArmor ? 1 : 0)} {HeroLevel} {(string.IsNullOrEmpty(Biography) || Biography == null ? Language.Instance.GetMessageFromKey("NO_PREZ_MESSAGE") : Biography)}";
        }

        public string GenerateRest() => $"rest 1 {CharacterId} {(IsSitting ? 1 : 0)}";

        public string GenerateRaidBf(byte type) => $"raidbf 0 {type} 25 ";

        public string GenerateRevive()
        {
            if (MapInstance?.InstanceBag == null)
            {
                return $"revive 1 {CharacterId} 0";
            }

            int lives = MapInstance.InstanceBag.Lives - MapInstance.InstanceBag.DeadList.Count + 1;
            return $"revive 1 {CharacterId} {(lives > 0 ? lives : 0)}";
        }

        public string GenerateSay(string message, int type) => $"say 1 {CharacterId} {type} {message}";

        public string GenerateScal() => $"char_sc 1 {CharacterId} {Size}";

        public List<string> GenerateScN()
        {
            List<string> list = new List<string>();
            byte i = 0;
            Mates.Where(s => s.MateType == MateType.Partner).ToList().ForEach(s =>
            {
                s.PetId = i;
                s.LoadInventory();
                list.Add(s.GenerateScPacket());
                i++;
            });
            return list;
        }

        public List<string> GenerateScP(byte page = 0)
        {
            List<string> list = new List<string>();
            byte i = 0;
            Mates.Where(s => s.MateType == MateType.Pet).Skip(page * 10).Take(10).ToList().ForEach(s =>
            {
                s.PetId = i;
                list.Add(s.GenerateScPacket());
                i++;
            });
            return list;
        }

        public string GenerateScpStc() => $"sc_p_stc {MaxMateCount / 10}";

        public string GenerateShop(string shopname) => $"shop 1 {CharacterId} 1 3 0 {shopname}";

        private string GenerateShopEnd() => $"shop 1 {CharacterId} 0 0";

        public string GenerateSki()
        {
            List<CharacterSkill> characterSkills;
            if (UseSp)
            {
                characterSkills = SkillsSp.Values.OrderBy(s => s.Skill.CastId).ToList();
            }
            else
            {
                characterSkills = Skills.Values.OrderBy(s => s.Skill.CastId).ToList();
            }

            string skibase = string.Empty;
            string generatedSkills = string.Empty;
            if (!UseSp)
            {
                skibase = $"{200 + 20 * (byte)Class} {201 + 20 * (byte)Class}";
            }
            else if (characterSkills.Count > 0)
            {
                skibase = $"{characterSkills.ElementAt(0).SkillVNum} {characterSkills.ElementAt(0).SkillVNum}";
                generatedSkills = characterSkills?.Aggregate(string.Empty, (current, ski) => current + $" {ski.SkillVNum}");
            }

            return $"ski {skibase}{generatedSkills}";
        }

        public string GenerateSpk(object message, int type) => $"spk 1 {CharacterId} {type} {Name} {message}";

        public string GenerateSpPoint() => $"sp {SpAdditionPoint} 1000000 {SpPoint} 10000";


        [Obsolete(
            "GenerateStartupInventory should be used only on startup, for refreshing an inventory slot please use GenerateInventoryAdd instead.")]
        public void GenerateStartupInventory()
        {
            string inv0 = "inv 0",
                inv1 = "inv 1",
                inv2 = "inv 2",
                inv3 = "inv 3",
                inv6 = "inv 6",
                inv7 = "inv 7"; // inv 3 used for miniland objects
            if (Inventory != null)
            {
                foreach (ItemInstance inv in Inventory.Select(s => s.Value))
                {
                    switch (inv.Type)
                    {
                        case InventoryType.Wear:
                            if (inv is WearableInstance wearableinstance)
                            {
                                switch (inv.Slot)
                                {
                                    case (byte)EquipmentType.MainWeapon:
                                        Inventory.PrimaryWeapon = wearableinstance;
                                        EquipmentOptionHelper.Instance
                                            .ShellToBCards(wearableinstance.EquipmentOptions, wearableinstance.ItemVNum)
                                            .ForEach(s => BattleEntity.StaticBcards.Add(s));
                                        break;

                                    case (byte)EquipmentType.SecondaryWeapon:
                                        Inventory.SecondaryWeapon = wearableinstance;
                                        EquipmentOptionHelper.Instance
                                            .ShellToBCards(wearableinstance.EquipmentOptions, wearableinstance.ItemVNum)
                                            .ForEach(s => BattleEntity.StaticBcards.Add(s));
                                        break;

                                    case (byte)EquipmentType.Armor:
                                        Inventory.Armor = wearableinstance;
                                        EquipmentOptionHelper.Instance
                                            .ShellToBCards(wearableinstance.EquipmentOptions, wearableinstance.ItemVNum)
                                            .ForEach(s => BattleEntity.StaticBcards.Add(s));
                                        break;

                                    case (byte)EquipmentType.Bracelet:
                                    case (byte)EquipmentType.Necklace:
                                    case (byte)EquipmentType.Ring:
                                        EquipmentOptionHelper.Instance
                                            .CellonToBCards(wearableinstance.EquipmentOptions,
                                                wearableinstance.ItemVNum)
                                            .ForEach(s => BattleEntity.StaticBcards.Add(s));
                                        break;
                                }

                                inv.Item.BCards.ForEach(s => BattleEntity.StaticBcards.Add(s));
                            }

                            break;
                        case InventoryType.Equipment:
                            if (inv.Item.EquipmentSlot == EquipmentType.Sp)
                            {
                                if (inv is SpecialistInstance specialistInstance)
                                {
                                    inv0 +=
                                        $" {inv.Slot}.{inv.ItemVNum}.{specialistInstance.Rare}.{specialistInstance.Upgrade}.{specialistInstance.SpStoneUpgrade}";
                                }
                            }
                            else
                            {
                                if (inv is WearableInstance wearableInstance)
                                {
                                    inv0 +=
                                        $" {inv.Slot}.{inv.ItemVNum}.{wearableInstance.Rare}.{(inv.Item.IsColored ? wearableInstance.Design : wearableInstance.Upgrade)}.0";
                                }
                            }

                            break;

                        case InventoryType.Main:
                            inv1 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}.0";
                            break;

                        case InventoryType.Etc:
                            inv2 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}.0";
                            break;

                        case InventoryType.Miniland:
                            inv3 += $" {inv.Slot}.{inv.ItemVNum}.{inv.Amount}";
                            break;

                        case InventoryType.Specialist:
                            if (inv is SpecialistInstance specialist)
                            {
                                inv6 +=
                                    $" {inv.Slot}.{inv.ItemVNum}.{specialist.Rare}.{specialist.Upgrade}.{specialist.SpStoneUpgrade}";
                            }

                            break;

                        case InventoryType.Costume:
                            if (inv is WearableInstance costumeInstance)
                            {
                                inv7 +=
                                    $" {inv.Slot}.{inv.ItemVNum}.{costumeInstance.Rare}.{costumeInstance.Upgrade}.0";
                            }

                            break;
                    }
                }
            }

            Session.SendPacket(inv0);
            Session.SendPacket(inv1);
            Session.SendPacket(inv2);
            Session.SendPacket(inv3);
            Session.SendPacket(inv6);
            Session.SendPacket(inv7);
            Session.SendPacket(GetMinilandObjectList());
        }

        public string GenerateStashAll()
        {
            string stash = $"stash_all {WareHouseSize}";
            return Inventory.Where(s => s.Value.Type == InventoryType.Warehouse).Select(s => s.Value)
                .Aggregate(stash, (current, item) => current + $" {item.GenerateStashPacket()}");
        }

        public string GenerateStat()
        {
            double option =
                (WhisperBlocked ? Math.Pow(2, (int)CharacterOption.WhisperBlocked - 1) : 0)
                + (FamilyRequestBlocked ? Math.Pow(2, (int)CharacterOption.FamilyRequestBlocked - 1) : 0)
                + (!MouseAimLock ? Math.Pow(2, (int)CharacterOption.MouseAimLock - 1) : 0)
                + (MinilandInviteBlocked ? Math.Pow(2, (int)CharacterOption.MinilandInviteBlocked - 1) : 0)
                + (ExchangeBlocked ? Math.Pow(2, (int)CharacterOption.ExchangeBlocked - 1) : 0)
                + (FriendRequestBlocked ? Math.Pow(2, (int)CharacterOption.FriendRequestBlocked - 1) : 0)
                + (EmoticonsBlocked ? Math.Pow(2, (int)CharacterOption.EmoticonsBlocked - 1) : 0)
                + (HpBlocked ? Math.Pow(2, (int)CharacterOption.HpBlocked - 1) : 0)
                + (BuffBlocked ? Math.Pow(2, (int)CharacterOption.BuffBlocked - 1) : 0)
                + (GroupRequestBlocked ? Math.Pow(2, (int)CharacterOption.GroupRequestBlocked - 1) : 0)
                + (HeroChatBlocked ? Math.Pow(2, (int)CharacterOption.HeroChatBlocked - 1) : 0)
                + (QuickGetUp ? Math.Pow(2, (int)CharacterOption.QuickGetUp - 1) : 0)
                + (!IsPetAutoRelive ? 64 : 0)
                + (!IsPartnerAutoRelive ? 128 : 0);

            return $"stat {Hp} {HpLoad()} {Mp} {MpLoad()} 0 {option}";
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted",
            Justification = "Readability")]
        public string GenerateStatChar()
        {
            int type = 0;
            int type2 = 0;
            switch (Class)
            {
                case (byte)ClassType.Adventurer:
                    type = 0;
                    type2 = 1;
                    break;

                case ClassType.Magician:
                    type = 2;
                    type2 = 1;
                    break;

                case ClassType.Swordman:
                    type = 0;
                    type2 = 1;
                    break;

                case ClassType.Archer:
                    type = 1;
                    type2 = 0;
                    break;
            }

            int weaponUpgrade = 0;
            int secondaryUpgrade = 0;
            int armorUpgrade = 0;

            MinHit = CharacterHelper.Instance.MinHit(Class, Level);
            MaxHit = CharacterHelper.Instance.MaxHit(Class, Level);
            HitRate = CharacterHelper.Instance.HitRate(Class, Level);
            HitCriticalRate = CharacterHelper.Instance.HitCriticalRate(Class, Level);
            HitCritical = CharacterHelper.Instance.HitCritical(Class, Level);
            MinDistance = CharacterHelper.Instance.MinDistance(Class, Level);
            MaxDistance = CharacterHelper.Instance.MaxDistance(Class, Level);
            DistanceRate = CharacterHelper.Instance.DistanceRate(Class, Level);
            DistanceCriticalRate = CharacterHelper.Instance.DistCriticalRate(Class, Level);
            DistanceCritical = CharacterHelper.Instance.DistCritical(Class, Level);
            FireResistance = CharacterHelper.Instance.FireResistance(Class, Level) +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.FireIncreased, false)[0] +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.AllIncreased, false)[0];
            LightResistance = CharacterHelper.Instance.LightResistance(Class, Level) +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.LightIncreased, false)[0] +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.AllIncreased, false)[0];
            WaterResistance = CharacterHelper.Instance.WaterResistance(Class, Level) +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.WaterIncreased, false)[0] +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.AllIncreased, false)[0];
            DarkResistance = CharacterHelper.Instance.DarkResistance(Class, Level) +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.DarkIncreased, false)[0] +
                GetStuffBuff(BCardType.CardType.ElementResistance,
                    (byte)AdditionalTypes.ElementResistance.AllIncreased, false)[0];
            Defence = CharacterHelper.Instance.Defence(Class, Level);
            DefenceRate = CharacterHelper.Instance.DefenceRate(Class, Level);
            ElementRate = CharacterHelper.Instance.ElementRate(Class, Level);
            ElementRateSp = 0;
            DistanceDefence = CharacterHelper.Instance.DistanceDefence(Class, Level);
            DistanceDefenceRate = CharacterHelper.Instance.DistanceDefenceRate(Class, Level);
            MagicalDefence = CharacterHelper.Instance.MagicalDefence(Class, Level);
            if (UseSp)
            {
                // handle specialist
                if (SpInstance != null)
                {
                    int slHit = CharacterHelper.Instance.SlPoint(SpInstance.SlDamage, 0);
                    int slDefence = CharacterHelper.Instance.SlPoint(SpInstance.SlDefence, 1);
                    int slElement = CharacterHelper.Instance.SlPoint(SpInstance.SlElement, 2);
                    int slHp = CharacterHelper.Instance.SlPoint(SpInstance.SlHP, 3);

                    if (Session != null)
                    {
                        slHit += Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                                (byte)AdditionalTypes.SPSL.Attack) +
                            Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                                (byte)AdditionalTypes.SPSL.All);

                        slDefence +=
                            Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                                (byte)AdditionalTypes.SPSL.Defense) +
                            Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL, (byte)AdditionalTypes.SPSL.All);

                        slElement +=
                            Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                                (byte)AdditionalTypes.SPSL.Element) +
                            Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL, (byte)AdditionalTypes.SPSL.All);

                        slHp += Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                                (byte)AdditionalTypes.SPSL.HPMP) +
                            Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                                (byte)AdditionalTypes.SPSL.All);

                        slHit = slHit > 100 ? 100 : slHit;
                        slDefence = slDefence > 100 ? 100 : slDefence;
                        slElement = slElement > 100 ? 100 : slElement;
                        slHp = slHp > 100 ? 100 : slHp;
                    }

                    MinHit += SpInstance.DamageMinimum + slHit * 10;
                    MaxHit += SpInstance.DamageMaximum + slHit * 10;
                    MinDistance += SpInstance.DamageMinimum + slHit * 10;
                    MaxDistance += SpInstance.DamageMaximum + slHit * 10;
                    HitCriticalRate += SpInstance.CriticalLuckRate;
                    HitCritical += SpInstance.CriticalRate;
                    DistanceCriticalRate += SpInstance.CriticalLuckRate;
                    DistanceCritical += SpInstance.CriticalRate;
                    HitRate += SpInstance.HitRate;
                    DistanceRate += SpInstance.HitRate;
                    DefenceRate += SpInstance.DefenceDodge;
                    DistanceDefenceRate += SpInstance.DistanceDefenceDodge;
                    FireResistance += SpInstance.Item.FireResistance + SpInstance.SpFire;
                    WaterResistance += SpInstance.Item.WaterResistance + SpInstance.SpWater;
                    LightResistance += SpInstance.Item.LightResistance + SpInstance.SpLight;
                    DarkResistance += SpInstance.Item.DarkResistance + SpInstance.SpDark;
                    ElementRateSp += SpInstance.ElementRate + SpInstance.SpElement;
                    Defence += SpInstance.CloseDefence + slDefence * 10;
                    DistanceDefence += SpInstance.DistanceDefence + slDefence * 10;
                    MagicalDefence += SpInstance.MagicDefence + slDefence * 10;

                    int point = CharacterHelper.Instance.SlPoint((short)slHit, 0);
                    int p = 0;
                    if (point <= 10)
                    {
                        p = point * 5;
                    }
                    else if (point <= 20)
                    {
                        p = 50 + (point - 10) * 6;
                    }
                    else if (point <= 30)
                    {
                        p = 110 + (point - 20) * 7;
                    }
                    else if (point <= 40)
                    {
                        p = 180 + (point - 30) * 8;
                    }
                    else if (point <= 50)
                    {
                        p = 260 + (point - 40) * 9;
                    }
                    else if (point <= 60)
                    {
                        p = 350 + (point - 50) * 10;
                    }
                    else if (point <= 70)
                    {
                        p = 450 + (point - 60) * 11;
                    }
                    else if (point <= 80)
                    {
                        p = 560 + (point - 70) * 13;
                    }
                    else if (point <= 90)
                    {
                        p = 690 + (point - 80) * 14;
                    }
                    else if (point <= 94)
                    {
                        p = 830 + (point - 90) * 15;
                    }
                    else if (point <= 95)
                    {
                        p = 890 + 16;
                    }
                    else if (point <= 97)
                    {
                        p = 906 + (point - 95) * 17;
                    }
                    else if (point <= 100)
                    {
                        p = 940 + (point - 97) * 20;
                    }

                    MaxHit += p;
                    MinHit += p;
                    MaxDistance += p;
                    MinDistance += p;

                    point = CharacterHelper.Instance.SlPoint((short)slDefence, 1);
                    p = 0;
                    if (point <= 10)
                    {
                        p = point;
                    }
                    else if (point <= 20)
                    {
                        p = 10 + (point - 10) * 2;
                    }
                    else if (point <= 30)
                    {
                        p = 30 + (point - 20) * 3;
                    }
                    else if (point <= 40)
                    {
                        p = 60 + (point - 30) * 4;
                    }
                    else if (point <= 50)
                    {
                        p = 100 + (point - 40) * 5;
                    }
                    else if (point <= 60)
                    {
                        p = 150 + (point - 50) * 6;
                    }
                    else if (point <= 70)
                    {
                        p = 210 + (point - 60) * 7;
                    }
                    else if (point <= 80)
                    {
                        p = 280 + (point - 70) * 8;
                    }
                    else if (point <= 90)
                    {
                        p = 360 + (point - 80) * 9;
                    }
                    else if (point <= 100)
                    {
                        p = 450 + (point - 90) * 10;
                    }

                    Defence += p;
                    MagicalDefence += p;
                    DistanceDefence += p;

                    point = CharacterHelper.Instance.SlPoint((short)slElement, 2);
                    if (point <= 50)
                    {
                        p = point;
                    }
                    else
                    {
                        p = 50 + (point - 50) * 2;
                    }

                    ElementRateSp += p;
                }
            }

            // TODO: add base stats
            if (Inventory.PrimaryWeapon != null)
            {
                weaponUpgrade = Inventory.PrimaryWeapon.Upgrade;
                MinHit += Inventory.PrimaryWeapon.DamageMinimum + Inventory.PrimaryWeapon.Item.DamageMinimum;
                MaxHit += Inventory.PrimaryWeapon.DamageMaximum + Inventory.PrimaryWeapon.Item.DamageMaximum;
                HitRate += Inventory.PrimaryWeapon.HitRate + Inventory.PrimaryWeapon.Item.HitRate;
                HitCriticalRate += Inventory.PrimaryWeapon.CriticalLuckRate +
                    Inventory.PrimaryWeapon.Item.CriticalLuckRate;
                HitCritical += Inventory.PrimaryWeapon.CriticalRate + Inventory.PrimaryWeapon.Item.CriticalRate;

                // maxhp-mp
            }

            if (Inventory.SecondaryWeapon != null)
            {
                secondaryUpgrade = Inventory.SecondaryWeapon.Upgrade;
                MinDistance += Inventory.SecondaryWeapon.DamageMinimum + Inventory.SecondaryWeapon.Item.DamageMinimum;
                MaxDistance += Inventory.SecondaryWeapon.DamageMaximum + Inventory.SecondaryWeapon.Item.DamageMaximum;
                DistanceRate += Inventory.SecondaryWeapon.HitRate + Inventory.SecondaryWeapon.Item.HitRate;
                DistanceCriticalRate += Inventory.SecondaryWeapon.CriticalLuckRate +
                    Inventory.SecondaryWeapon.Item.CriticalLuckRate;
                DistanceCritical +=
                    Inventory.SecondaryWeapon.CriticalRate + Inventory.SecondaryWeapon.Item.CriticalRate;

                // maxhp-mp
            }

            if (Inventory.Armor != null)
            {
                armorUpgrade = Inventory.Armor.Upgrade;
                Defence += Inventory.Armor.CloseDefence + Inventory.Armor.Item.CloseDefence;
                DistanceDefence += Inventory.Armor.DistanceDefence + Inventory.Armor.Item.DistanceDefence;
                MagicalDefence += Inventory.Armor.MagicDefence + Inventory.Armor.Item.MagicDefence;
                DefenceRate += Inventory.Armor.DefenceDodge + Inventory.Armor.Item.DefenceDodge;
                DistanceDefenceRate += Inventory.Armor.DistanceDefenceDodge + Inventory.Armor.Item.DistanceDefenceDodge;
            }

            var fairy = Inventory?.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Fairy, InventoryType.Wear);
            if (fairy != null)
            {
                ElementRate += fairy.ElementRate + fairy.Item.ElementRate;
            }

            for (short i = 1; i < 14; i++)
            {
                var item = Inventory?.LoadBySlotAndType<WearableInstance>(i, InventoryType.Wear);
                if (item == null)
                {
                    continue;
                }

                if (item.Item.EquipmentSlot == EquipmentType.MainWeapon ||
                    item.Item.EquipmentSlot == EquipmentType.SecondaryWeapon ||
                    item.Item.EquipmentSlot == EquipmentType.Armor ||
                    item.Item.EquipmentSlot == EquipmentType.Sp)
                {
                    continue;
                }

                FireResistance += item.FireResistance + item.Item.FireResistance;
                LightResistance += item.LightResistance + item.Item.LightResistance;
                WaterResistance += item.WaterResistance + item.Item.WaterResistance;
                DarkResistance += item.DarkResistance + item.Item.DarkResistance;
                Defence += item.CloseDefence + item.Item.CloseDefence;
                DefenceRate += item.DefenceDodge + item.Item.DefenceDodge;
                DistanceDefence += item.DistanceDefence + item.Item.DistanceDefence;
                DistanceDefenceRate += item.DistanceDefenceDodge + item.Item.DistanceDefenceDodge;
            }

            return
                $"sc {type} {weaponUpgrade} {MinHit} {MaxHit} {HitRate} {HitCriticalRate} {HitCritical} {type2} {secondaryUpgrade} {MinDistance} {MaxDistance} {DistanceRate} {DistanceCriticalRate} {DistanceCritical} {armorUpgrade} {Defence} {DefenceRate} {DistanceDefence} {DistanceDefenceRate} {MagicalDefence} {FireResistance} {WaterResistance} {LightResistance} {DarkResistance}";
        }

        public string GenerateStatInfo()
        {
            return
                $"st 1 {CharacterId} {Level} {HeroLevel} {(int)(Hp / (float)HpLoad() * 100)} {(int)(Mp / (float)MpLoad() * 100)} {Hp} {Mp}{Buff.Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}")}";
        }

        public TalkPacket GenerateTalk(string message) => new TalkPacket
        {
            CharacterId = CharacterId,
            Message = message
        };

        public string GenerateTit() =>
            $"tit {Language.Instance.GetMessageFromKey(Class == (byte)ClassType.Adventurer ? ClassType.Adventurer.ToString().ToUpper() : Class == ClassType.Swordman ? ClassType.Swordman.ToString().ToUpper() : Class == ClassType.Archer ? ClassType.Archer.ToString().ToUpper() : ClassType.Magician.ToString().ToUpper())} {Name}";

        public string GenerateTp() => $"tp 1 {CharacterId} {PositionX} {PositionY} 0";

        public void GetAct4Points(int point)
        {
            //RefreshComplimentRankingIfNeeded();
            Act4Points += point;
        }

        public int GetCp()
        {
            int cpmax = (Class > 0 ? 40 : 0) + JobLevel * 2;
            int cpused = Skills.Select(s => s.Value).Aggregate(0, (current, ski) => current + ski?.Skill.CPCost ?? 0);
            return cpmax - cpused;
        }

        public void GetDamage(int damage, IBattleEntity entity, bool canKill = true)
        {
            if (Hp <= 0)
            {
                return;
            }

            LastDefence = DateTime.Now;
            CloseShop();
            CloseExchangeOrTrade();
            if (MapInstance == null)
            {
                return;
            }

            switch (MapInstance.MapInstanceType)
            {
                case MapInstanceType.IceBreakerInstance:
                    Hp -= damage / 3;
                    break;

                default:
                    Hp -= MapInstance.IsPvp && MapInstance.MapInstanceType != MapInstanceType.CaligorInstance
                        ? damage / 2
                        : damage;
                    break;
            }

            Hp = Hp <= 0 ? !canKill ? 1 : 0 : Hp;
            Session.SendPacket(GenerateStat());
            if (Hp <= 0)
            {
                GenerateDeath(entity);
                entity.GenerateRewards(this);
            }
        }

        public int GetDignityIco()
        {
            int icoDignity = 1;

            if (Dignity <= -100)
            {
                icoDignity = 2;
            }

            if (Dignity <= -200)
            {
                icoDignity = 3;
            }

            if (Dignity <= -400)
            {
                icoDignity = 4;
            }

            if (Dignity <= -600)
            {
                icoDignity = 5;
            }

            if (Dignity <= -800)
            {
                icoDignity = 6;
            }

            return icoDignity;
        }

        public List<Portal> GetExtraPortal()
        {
            if (MapInstance?.Map == null || Miniland == null)
            {
                return new List<Portal>();
            }

            return MapInstancePortalHandler.GenerateMinilandEntryPortals(MapInstance.Map.MapId, Miniland.MapInstanceId);
        }

        public void TeleportInRadius(int Valeur)
        {
            switch (Direction)
            {
                case 0:
                    // -y
                    ServerManager.Instance.TeleportForward(Session, MapInstanceId, PositionX, (short)(PositionY - Valeur));
                    break;
                case 1:
                    // +x
                    ServerManager.Instance.TeleportForward(Session, MapInstanceId, (short)(PositionX + Valeur), PositionY);
                    break;
                case 2:
                    // +y
                    ServerManager.Instance.TeleportForward(Session, Session.Character.MapInstanceId, Session.Character.PositionX, (short)(Session.Character.PositionY + Valeur));
                    break;
                case 3:
                    // -x
                    ServerManager.Instance.TeleportForward(Session, Session.Character.MapInstanceId, (short)(Session.Character.PositionX - Valeur), Session.Character.PositionY);
                    break;
                case 4:
                    ServerManager.Instance.TeleportForward(Session, Session.Character.MapInstanceId, (short)(Session.Character.PositionX - Valeur), (short)(Session.Character.PositionY - Valeur));
                    // -x -y
                    break;
                case 5:
                    // +x +y
                    ServerManager.Instance.TeleportForward(Session, Session.Character.MapInstanceId, (short)(Session.Character.PositionX - Valeur), (short)(Session.Character.PositionY - Valeur));
                    break;
                case 6:
                    // +x -y
                    ServerManager.Instance.TeleportForward(Session, Session.Character.MapInstanceId, (short)(Session.Character.PositionX + Valeur), (short)(Session.Character.PositionY + Valeur));
                    break;
                case 7:
                    // -x +y
                    ServerManager.Instance.TeleportForward(Session, Session.Character.MapInstanceId, (short)(Session.Character.PositionX - Valeur), (short)(Session.Character.PositionY + Valeur));
                    break;
            }
        }

        public List<string> GetFamilyHistory()
        {
            //TODO: Fix some bugs(missing history etc)
            if (Family == null)
            {
                return new List<string>();
            }

            string packetheader = "ghis";
            List<string> packetList = new List<string>();
            string packet = string.Empty;
            int i = 0;
            int amount = 0;
            foreach (FamilyLogDTO log in Family.FamilyLogs
                .Where(s => s.FamilyLogType != FamilyLogType.WareHouseAdded &&
                    s.FamilyLogType != FamilyLogType.WareHouseRemoved).OrderByDescending(s => s.Timestamp)
                .Take(100))
            {
                packet +=
                    $" {(byte)log.FamilyLogType}|{log.FamilyLogData}|{(int)(DateTime.Now - log.Timestamp).TotalHours}";
                i++;
                if (i == 50)
                {
                    i = 0;
                    packetList.Add($"{packetheader}{(amount == 0 ? " 0 " : "")}{packet}");
                    amount++;
                }
                else if ((i + 50 * amount) == Family.FamilyLogs.Count)
                {
                    packetList.Add($"{packetheader}{(amount == 0 ? " 0 " : "")}{packet}");
                }
            }

            return packetList;
        }

        public string GetMinilandObjectList()
        {
            string mlobjstring = "mlobjlst";
            foreach (ItemInstance item in Inventory.Where(s => s.Value.Type == InventoryType.Miniland)
                .OrderBy(s => s.Value.Slot).Select(s => s.Value))
            {
                if (item.Item.IsWarehouse)
                {
                    WareHouseSize = item.Item.MinilandObjectPoint;
                }

                MapDesignObject mp =
                    Session.Character.MapInstance.MapDesignObjects.FirstOrDefault(s => s.ItemInstanceId == item.Id);
                bool used = mp != null;
                mlobjstring +=
                    $" {item.Slot}.{(used ? 1 : 0)}.{(used ? mp.MapX : 0)}.{(used ? mp.MapY : 0)}.{(item.Item.Width != 0 ? item.Item.Width : 1)}.{(item.Item.Height != 0 ? item.Item.Height : 1)}.{(used ? mp.ItemInstance.DurabilityPoint : 0)}.100.0.1";
            }

            return mlobjstring;
        }

        public void GetXp(long val)
        {
            LevelXp += val * ServerManager.Instance.XpRate *
                (int)(1 + GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D);
            GenerateLevelXpLevelUp();
        }

        public void GetJobExp(long val)
        {
            if (UseSp && SpInstance != null)
            {
                if (SpInstance.SpLevel >= ServerManager.Instance.MaxSpLevel)
                {
                    return;
                }

                int multiplier = SpInstance.SpLevel < 10 ? 10 :
                    SpInstance.SpLevel < 19 ? 5 : 1;
                SpInstance.XP +=
                    (int)(val * (multiplier +
                            GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D) *
                        ((double)Authority / 100 + 1));
                GenerateSpXpLevelUp();
                return;
            }

            if (JobLevel >= ServerManager.Instance.MaxJobLevel)
            {
                return;
            }

            JobLevelXp +=
                (int)(val * (1 + GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D));
            GenerateJobXpLevelUp();
        }

        public void GetReputOnKill(object sender, KillArgs args)
        {
            switch (args.KilledEntity.SessionType())
            {
                case WingsEmu.Packets.Enums.SessionType.Monster:
                    GetReput(args.KilledEntity.BattleEntity.Level * ServerManager.Instance.ReputRate);
                    break;
            }
        }

        public void GetReput(long val, bool enableMessage = false)
        {
            Reput += val * ServerManager.Instance.ReputRate;
            Session.SendPacket(GenerateFd());
            if (!enableMessage)
            {
                return;
            }

            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_INCREASE"), val * ServerManager.Instance.ReputRate),
                11));
        }

        public void LoseReput(long val)
        {
            Reput -= val;
            Session.SendPacket(GenerateFd());
            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("REPUT_DECREASE"), val),
                11));
        }

        public void GetGold(long val, bool isQuest = false)
        {
            Session.Character.Gold += val;
            if (Session.Character.Gold > ServerManager.Instance.MaxGold)
            {
                Session.Character.Gold = ServerManager.Instance.MaxGold;
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("MAX_GOLD"), 0));
            }

            Session.SendPacket(isQuest
                ? GenerateSay($"Quest reward: [ {ServerManager.Instance.GetItem(1046).Name} x {val} ]", 10)
                : Session.Character.GenerateSay(
                    $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {ServerManager.Instance.GetItem(1046).Name} x {val}",
                    10));
            Session.SendPacket(Session.Character.GenerateGold());
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted",
            Justification = "Readability")]
        public int GetReputIco()
        {
            if (Reput >= 5000001)
            {
                switch (IsReputHero())
                {
                    case 1:
                        return 28;

                    case 2:
                        return 29;

                    case 3:
                        return 30;

                    case 4:
                        return 31;

                    case 5:
                        return 32;
                }
            }

            if (Reput <= 50)
            {
                return 1;
            }

            if (Reput <= 150)
            {
                return 2;
            }

            if (Reput <= 250)
            {
                return 3;
            }

            if (Reput <= 500)
            {
                return 4;
            }

            if (Reput <= 750)
            {
                return 5;
            }

            if (Reput <= 1000)
            {
                return 6;
            }

            if (Reput <= 2250)
            {
                return 7;
            }

            if (Reput <= 3500)
            {
                return 8;
            }

            if (Reput <= 5000)
            {
                return 9;
            }

            if (Reput <= 9500)
            {
                return 10;
            }

            if (Reput <= 19000)
            {
                return 11;
            }

            if (Reput <= 25000)
            {
                return 12;
            }

            if (Reput <= 40000)
            {
                return 13;
            }

            if (Reput <= 60000)
            {
                return 14;
            }

            if (Reput <= 85000)
            {
                return 15;
            }

            if (Reput <= 115000)
            {
                return 16;
            }

            if (Reput <= 150000)
            {
                return 17;
            }

            if (Reput <= 190000)
            {
                return 18;
            }

            if (Reput <= 235000)
            {
                return 19;
            }

            if (Reput <= 285000)
            {
                return 20;
            }

            if (Reput <= 350000)
            {
                return 21;
            }

            if (Reput <= 500000)
            {
                return 22;
            }

            if (Reput <= 1500000)
            {
                return 23;
            }

            if (Reput <= 2500000)
            {
                return 24;
            }

            if (Reput <= 3750000)
            {
                return 25;
            }

            return Reput <= 5000000 ? 26 : 27;
        }

        public void GiftAdd(short itemVNum, ushort amount, short design = 0, byte upgrade = 0, sbyte rare = 0,
            bool isQuest = false)
        {
            //TODO add the rare support
            if (Inventory == null)
            {
                return;
            }

            lock(Inventory)
            {
                ItemInstance newItem = Inventory.InstantiateItemInstance(itemVNum, CharacterId, amount, rare);
                if (newItem == null)
                {
                    return;
                }

                newItem.Design = design;
                newItem.Rare = rare;
                switch (newItem.Item.ItemType)
                {
                    case ItemType.Armor:
                    case ItemType.Weapon:
                    case ItemType.Shell:
                        ((WearableInstance)newItem).RarifyItem(Session, RarifyMode.Drop, RarifyProtection.None);
                        if (rare != 0)
                        {
                            newItem.Rare = rare;
                            ((WearableInstance)newItem).SetRarityPoint();
                        }

                        newItem.Upgrade = upgrade;
                        break;
                    case ItemType.Specialist:
                        newItem.Upgrade = upgrade;
                        break;
                }

                if (newItem.Item.ItemType == ItemType.Shell)
                {
                    byte[] incompleteShells = { 25, 30, 40, 55, 60, 65, 70, 75, 80, 85 };
                    int rand = ServerManager.Instance.RandomNumber(0, 101);
                    if (!ShellGeneratorHelper.Instance.ShellTypes.TryGetValue(newItem.ItemVNum, out byte shellType))
                    {
                        return;
                    }

                    bool isIncomplete = shellType == 8 || shellType == 9;

                    if (rand < 84)
                    {
                        if (isIncomplete)
                        {
                            newItem.Upgrade = incompleteShells[ServerManager.Instance.RandomNumber(0, 6)];
                        }
                        else
                        {
                            newItem.Upgrade = (byte)ServerManager.Instance.RandomNumber(50, 75);
                        }
                    }
                    else if (rand <= 99)
                    {
                        if (isIncomplete)
                        {
                            newItem.Upgrade = 75;
                        }
                        else
                        {
                            newItem.Upgrade = (byte)ServerManager.Instance.RandomNumber(75, 79);
                        }
                    }
                    else
                    {
                        if (isIncomplete)
                        {
                            newItem.Upgrade = (byte)(ServerManager.Instance.RandomNumber() > 50 ? 85 : 80);
                        }
                        else
                        {
                            newItem.Upgrade = (byte)ServerManager.Instance.RandomNumber(80, 90);
                        }
                    }
                }

                List<ItemInstance> newInv = Inventory.AddToInventory(newItem);
                if (newInv.Count > 0)
                {
                    Session.SendPacket(isQuest
                        ? GenerateSay($"Quest reward: [ {newItem.Item.Name} x {amount} ]", 10)
                        : GenerateSay(
                            $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {newItem.Item.Name} x {amount}",
                            10));
                }
                else
                {
                    if (MailList.Count > 40)
                    {
                        return;
                    }

                    SendGift(CharacterId, itemVNum, amount, newItem.Rare, newItem.Upgrade, false, (byte)design);
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        Language.Instance.GetMessageFromKey("ITEM_ACQUIRED_BY_THE_GIANT_MONSTER"), 0));
                }
            }
        }

        public bool HaveBackpack()
        {
            return StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.BackPack);
        }

        public double HpLoad()
        {
            double multiplicator = 1.0;
            int hp = 0;
            if (UseSp)
            {
                SpecialistInstance specialist = SpInstance;
                if (SpInstance != null)
                {
                    int shellSlHpMp = SpInstance.SlHP +
                        Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                            (byte)AdditionalTypes.SPSL.HPMP) +
                        Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                            (byte)AdditionalTypes.SPSL.All);
                    int point = CharacterHelper.Instance.SlPoint((short)(shellSlHpMp > 410 ? 410 : shellSlHpMp), 3);

                    if (point <= 50)
                    {
                        multiplicator += point / 100.0;
                    }
                    else
                    {
                        multiplicator += 0.5 + (point - 50.00) / 50.00;
                    }

                    hp = specialist.HP + specialist.SpHP * 100;
                }
            }

            bool bearSpirit = HasBuff(BCardType.CardType.BearSpirit, (byte)AdditionalTypes.BearSpirit.IncreaseMaximumHP);
            multiplicator += GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.IncreasesMaximumHP)[0] / 100D;
            hp += CharacterHelper.Instance.HpData[(byte)Class, Level]
                + GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPIncreased)[0]
                + GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPMPIncreased)[0]
                - GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPDecreased)[0];
            hp = (int)(hp * multiplicator);
            return bearSpirit
                ? hp > 16666
                    ? hp + 5000
                    : (int)(hp * (1.0 + GetBuff(BCardType.CardType.BearSpirit,
                        (byte)AdditionalTypes.BearSpirit.IncreaseMaximumHP)[0] / 100D))
                : hp;
        }

        public void InsertOrUpdatePenalty(PenaltyLogDTO log)
        {
            DaoFactory.Instance.PenaltyLogDao.InsertOrUpdate(ref log);
        }

        public bool IsBlockedByCharacter(long characterId)
        {
            return CharacterRelations.Any(b =>
                b.RelationType == CharacterRelationType.Blocked && b.CharacterId.Equals(characterId) &&
                characterId != CharacterId);
        }

        public bool IsBlockingCharacter(long characterId)
        {
            return CharacterRelations.Any(c =>
                c.RelationType == CharacterRelationType.Blocked && c.RelatedCharacterId.Equals(characterId));
        }

        public bool IsFriendlistFull()
        {
            return CharacterRelations.Where(s => s.RelationType == CharacterRelationType.Friend).ToList().Count >= 80;
        }

        public bool IsFriendOfCharacter(long characterId)
        {
            return CharacterRelations.Any(c =>
                c.RelationType == CharacterRelationType.Friend &&
                (c.RelatedCharacterId.Equals(characterId) || c.CharacterId.Equals(characterId)));
        }

        public bool IsMarriedToCharacter(long characterId)
        {
            return CharacterRelations.Any(c => c.RelationType == CharacterRelationType.Spouse && (c.RelatedCharacterId.Equals(characterId) || c.CharacterId.Equals(characterId)));
        }

        /// <summary>
        ///     Checks if the current character is in range of the given position
        /// </summary>
        /// <param name="xCoordinate">The x coordinate of the object to check.</param>
        /// <param name="yCoordinate">The y coordinate of the object to check.</param>
        /// <param name="range">The range of the coordinates to be maximal distanced.</param>
        /// <returns>True if the object is in Range, False if not.</returns>
        public bool IsInRange(int xCoordinate, int yCoordinate, int range = 50) => Math.Abs(PositionX - xCoordinate) <= range && Math.Abs(PositionY - yCoordinate) <= range;

        public bool IsMuted()
        {
            return Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.Muted && s.DateEnd > DateTime.Now);
        }

        public int IsReputHero()
        {
            int i = 0;
            foreach (CharacterDTO characterDto in ServerManager.Instance.TopReputation)
            {
                var character = (Character)characterDto;
                i++;
                if (character.CharacterId != CharacterId)
                {
                    continue;
                }

                switch (i)
                {
                    case 1:
                        return 5;
                    case 2:
                        return 4;
                    case 3:
                        return 3;
                }

                if (i <= 13)
                {
                    return 2;
                }

                if (i <= 43)
                {
                    return 1;
                }
            }

            return 0;
        }

        public void LearnAdventurerSkill()
        {
            if (Class != 0)
            {
                return;
            }

            byte newSkill = 0;
            for (int i = 200; i <= 210; i++)
            {
                if (i == 209)
                {
                    i++;
                }

                Skill skinfo = ServerManager.Instance.GetSkill((short)i);
                if (skinfo.Class != 0 || JobLevel < skinfo.LevelMinimum)
                {
                    continue;
                }

                if (Skills.Any(s => s.Value.SkillVNum == i))
                {
                    continue;
                }

                newSkill = 1;
                Skills[i] = new CharacterSkill { SkillVNum = (short)i, CharacterId = CharacterId };
            }

            if (newSkill <= 0)
            {
                return;
            }

            Session.SendPacket(
                UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SKILL_LEARNED"), 0));
            Session.SendPacket(GenerateSki());
            Session.SendPackets(GenerateQuicklist());
        }

        public void LearnSpSkill()
        {
            byte skillSpCount = (byte)SkillsSp.Count;
            SkillsSp = new ConcurrentDictionary<int, CharacterSkill>();
            foreach (Skill ski in ServerManager.Instance.GetAllSkill())
            {
                if (SpInstance != null && ski.Class == (Morph + 31) && SpInstance.SpLevel >= ski.LevelMinimum)
                {
                    SkillsSp[ski.SkillVNum] = new CharacterSkill { SkillVNum = ski.SkillVNum, CharacterId = CharacterId };
                }
            }

            if (SkillsSp.Count != skillSpCount)
            {
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SKILL_LEARNED"), 0));
            }
        }

        public void LoadInventory()
        {
            IEnumerable<ItemInstanceDTO> inventories = DaoFactory.Instance.IteminstanceDao.LoadByCharacterId(CharacterId)
                .Where(s => s.Type != InventoryType.FamilyWareHouse).ToList();
            IEnumerable<CharacterDTO> characters =
                DaoFactory.Instance.CharacterDao.LoadAllCharactersByAccount(Session.Account.AccountId);
            inventories = characters.Where(s => s.CharacterId != CharacterId).Aggregate(inventories,
                (current, character) => current.Concat(DaoFactory.Instance.IteminstanceDao
                    .LoadByCharacterId(character.CharacterId).Where(s => s.Type == InventoryType.Warehouse).ToList()));
            Inventory = new Inventory(this);
            foreach (ItemInstanceDTO inventory in inventories)
            {
                inventory.CharacterId = CharacterId;
                Inventory[inventory.Id] = (ItemInstance)inventory;
                var wearinstance = inventory as WearableInstance;
                wearinstance?.EquipmentOptions.Clear();
                wearinstance?.EquipmentOptions.AddRange(
                    DaoFactory.Instance.EquipmentOptionDao.GetOptionsByWearableInstanceId(wearinstance.Id));
            }
        }

        public void LoadQuicklists()
        {
            QuicklistEntries = new List<QuicklistEntryDTO>();
            foreach (QuicklistEntryDTO qle in DaoFactory.Instance.QuicklistEntryDao.LoadByCharacterId(CharacterId).ToList())
            {
                QuicklistEntries.Add(qle);
            }
        }

        public void LoadPassive()
        {
            // TODO IMPROVE PERFORMANCES
            // ACCESSING A DICTIONARY LIKE THIS IS CPU CYCLE KILLER
            PassiveSkillHelper.Instance.PassiveSkillToBcards(Skills.Values.Where(s => s?.Skill.SkillType == 0).ToList())
                .ForEach(s => BattleEntity.StaticBcards.Add(s));
        }


        public void LoadSkills()
        {
            Skills = new ConcurrentDictionary<int, CharacterSkill>();
            IEnumerable<CharacterSkillDTO> characterskillDto =
                DaoFactory.Instance.CharacterSkillDao.LoadByCharacterId(CharacterId).ToList();
            foreach (CharacterSkillDTO characterskill in characterskillDto.OrderBy(s => s.SkillVNum))
            {
                if (!Skills.ContainsKey(characterskill.SkillVNum))
                {
                    Skills[characterskill.SkillVNum] = characterskill as CharacterSkill;
                }
            }
        }

        public void LoadSpeed()
        {
            if (MapInstance == ServerManager.Instance.LobbyMapInstance)
            {
                Speed = ServerManager.Instance.LobbySpeed;
            }
            else if (!IsVehicled && !IsCustomSpeed) // only load speed if you dont use custom speed
            {
                Speed = CharacterHelper.Instance.SpeedData[(byte)Class];

                if (UseSp)
                {
                    if (SpInstance != null)
                    {
                        Speed += SpInstance.Item.Speed;
                    }
                }

                Speed += (byte)GetBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementSpeedIncreased)[0];
                Speed *= (byte)(1 + GetBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MoveSpeedIncreased)[0] / 100D);
                Speed -= (byte)GetBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementSpeedDecreased)[0];
            }

            if (IsShopping)
            {
                Speed = 0;
                IsCustomSpeed = false;
                return;
            }

            // reload vehicle speed after opening an shop for instance
            if (IsVehicled)
            {
                Speed = VehicleSpeed;
            }
        }

        public double MpLoad()
        {
            int mp = 0;
            double multiplicator = 1.0;
            if (UseSp)
            {
                if (SpInstance != null)
                {
                    int shellSlHpMp = SpInstance.SlHP +
                        Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                            (byte)AdditionalTypes.SPSL.HPMP) +
                        Session.Character.GetMostValueEquipmentBuff(BCardType.CardType.SPSL,
                            (byte)AdditionalTypes.SPSL.All);
                    int point = CharacterHelper.Instance.SlPoint((short)(shellSlHpMp > 410 ? 410 : shellSlHpMp), 3);

                    if (point <= 50)
                    {
                        multiplicator += point / 100.0;
                    }
                    else
                    {
                        multiplicator += 0.5 + (point - 50.00) / 50.00;
                    }

                    mp = SpInstance != null ? SpInstance.MP + SpInstance.SpHP * 100 : 0;
                }
            }

            bool bearSpirit = HasBuff(BCardType.CardType.BearSpirit, (byte)AdditionalTypes.BearSpirit.IncreaseMaximumMP);
            multiplicator += GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.IncreasesMaximumMP)[0] / 100D;
            mp += CharacterHelper.Instance.MpData[(byte)Class, Level]
                + GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumMPIncreased)[0]
                + GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPMPIncreased)[0]
                - GetBuff(BCardType.CardType.MaxHPMP, (byte)AdditionalTypes.MaxHPMP.MaximumHPDecreased)[0];
            mp = (int)(mp * multiplicator);
            return bearSpirit
                ? mp > 16666
                    ? mp + 5000
                    : (int)(mp * (1.0 + GetBuff(BCardType.CardType.BearSpirit,
                        (byte)AdditionalTypes.BearSpirit.IncreaseMaximumMP)[0] / 100D))
                : mp;
        }

        public void NotifyRarifyResult(sbyte rare)
        {
            Session.SendPacket(GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RARIFY_SUCCESS"), rare),
                12));
            Session.SendPacket(
                UserInterfaceHelper.Instance.GenerateMsg(
                    string.Format(Language.Instance.GetMessageFromKey("RARIFY_SUCCESS"), rare), 0));
            MapInstance.Broadcast(GenerateEff(3005), PositionX, PositionY);
            Session.SendPacket("shop_end 1");
        }

        public string OpenFamilyWarehouse()
        {
            if (Family == null || Family.WarehouseSize == 0)
            {
                return UserInterfaceHelper.Instance.GenerateInfo(
                    Language.Instance.GetMessageFromKey("NO_FAMILY_WAREHOUSE"));
            }

            return GenerateFStashAll();
        }

        public List<string> OpenFamilyWarehouseHist()
        {
            List<string> packetList = new List<string>();
            if (Family != null && FamilyCharacter != null)
            {
                if (FamilyCharacter.Authority == FamilyAuthority.Manager && Family.ManagerCanGetHistory ||
                    FamilyCharacter.Authority == FamilyAuthority.Member && Family.MemberCanGetHistory ||
                    FamilyCharacter.Authority < FamilyAuthority.Manager)
                {
                    return GenerateFamilyWarehouseHist();
                }
            }

            packetList.Add(
                UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("NO_FAMILY_RIGHT")));
            return packetList;
        }

        public void RemoveVehicle()
        {
            SpecialistInstance sp = null;
            if (Inventory != null)
            {
                sp = Inventory.LoadBySlotAndType<SpecialistInstance>((byte)EquipmentType.Sp, InventoryType.Wear);
            }

            IsVehicled = false;
            LoadSpeed();
            if (UseSp)
            {
                if (sp != null)
                {
                    Morph = sp.Item.Morph;
                    MorphUpgrade = sp.Upgrade;
                    MorphUpgrade2 = sp.Design;
                }
            }
            else
            {
                Morph = 0;
            }

            Session.CurrentMapInstance?.Broadcast(GenerateCMode());
            Session.SendPacket(GenerateCond());
            LastSpeedChange = DateTime.Now;
        }

        public void Rest()
        {
            if (LastSkillUse.AddSeconds(4) > DateTime.Now || LastDefence.AddSeconds(4) > DateTime.Now)
            {
                return;
            }

            if (!IsVehicled)
            {
                IsSitting = !IsSitting;
                Session.CurrentMapInstance?.Broadcast(GenerateRest());
            }
            else
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("IMPOSSIBLE_TO_USE"), 10));
            }
        }


        public void ChangeChannel(string ip, short port, byte mode)
        {
            Session.SendPacket($"mz {ip} {port} {Session.Character.Slot}");
            Session.SendPacket($"it {mode}");

            Session.IsDisposing = true;

            CommunicationServiceClient.Instance.RegisterCrossServerLogin(Session.Account.AccountId,
                Session.SessionId);

            Session.Character.Save();
            Session.Disconnect();
        }

        /// <summary>
        ///     Connect to act4
        /// </summary>
        /// <returns></returns>
        public bool ConnectAct4()
        {
            if (Faction == FactionType.Neutral)
            {
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateInfo(
                        Language.Instance.GetMessageFromKey("ACT4_NEED_FACTION")));
                return false;
            }

            SerializableWorldServer act4ChannelInfo =
                CommunicationServiceClient.Instance.GetAct4ChannelInfo(ServerManager.Instance.ServerGroup);
            if (act4ChannelInfo == null)
            {
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateInfo(
                        Language.Instance.GetMessageFromKey("ACT4_CHANNEL_OFFLINE")));
                return false;
            }

            SerializableWorldServer connection =
                CommunicationServiceClient.Instance.GetPreviousChannelByAccountId(Session.Account.AccountId);
            if (connection == null)
            {
                if (Session.Character.MapId == 153)
                {
                    // RESPAWN AT CITADEL
                    Session.Character.MapX = (short)(39 + ServerManager.Instance.RandomNumber(-2, 3));
                    Session.Character.MapY = (short)(42 + ServerManager.Instance.RandomNumber(-2, 3));
                    Session.Character.MapId = (short)(Session.Character.Faction == FactionType.Angel ? 130 : 131);
                }

                switch (Session.Character.Faction)
                {
                    case FactionType.Angel:
                        Session.Character.MapId = 130;
                        Session.Character.MapX = (short)(12 + ServerManager.Instance.RandomNumber(-2, 3));
                        Session.Character.MapY = (short)(40 + ServerManager.Instance.RandomNumber(-2, 3));
                        break;
                    case FactionType.Demon:
                        Session.Character.MapId = 131;
                        Session.Character.MapX = (short)(12 + ServerManager.Instance.RandomNumber(-2, 3));
                        Session.Character.MapY = (short)(40 + ServerManager.Instance.RandomNumber(-2, 3));
                        break;
                }

                ChangeChannel(act4ChannelInfo.EndPointIp, (short)act4ChannelInfo.EndPointPort, 1);
                return true;
            }

            if (Session.CurrentMapInstance?.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.Act4) != true)
            {
                return true;
            }

            MapInstance map =
                ServerManager.Instance.Act4Maps.FirstOrDefault(s =>
                    s.Map.MapId == Session.CurrentMapInstance.Map.MapId);
            if (map != null)
            {
                ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, map.MapInstanceId,
                    Session.Character.MapX, Session.Character.MapY);
            }

            return true;
        }

        /// <summary>
        ///     Save the Character
        /// </summary>
        public void Save()
        {
            try
            {
                AccountDTO account = Session.Account;
                DaoFactory.Instance.AccountDao.InsertOrUpdate(ref account);

                CharacterDTO character = DeepCopy();
                DaoFactory.Instance.CharacterDao.InsertOrUpdate(ref character);

                if (Inventory != null)
                {
                    // be sure that noone tries to edit while saving is currently editing
                    lock(Inventory)
                    {
                        // load and concat inventory with equipment
                        IEnumerable<ItemInstance> inventories = Inventory.Select(s => s.Value);
                        IEnumerable<Guid> currentlySavedInventoryIds =
                            DaoFactory.Instance.IteminstanceDao.LoadSlotAndTypeByCharacterId(CharacterId);
                        IEnumerable<CharacterDTO> characters =
                            DaoFactory.Instance.CharacterDao.LoadAllCharactersByAccount(Session.Account.AccountId);
                        currentlySavedInventoryIds = characters.Where(s => s.CharacterId != CharacterId)
                            .Aggregate(currentlySavedInventoryIds,
                                (current, characteraccount) => current.Concat(DaoFactory.Instance.IteminstanceDao.LoadByCharacterId(characteraccount.CharacterId)
                                    .Where(s => s.Type == InventoryType.Warehouse)
                                    .Select(i => i.Id).ToList()));

                        IEnumerable<MinilandObjectDTO> currentlySavedMinilandObjectEntries =
                            DaoFactory.Instance.MinilandObjectDao.LoadByCharacterId(CharacterId).ToList();
                        foreach (MinilandObjectDTO mobjToDelete in currentlySavedMinilandObjectEntries.Except(
                            Miniland.MapDesignObjects))
                        {
                            DaoFactory.Instance.MinilandObjectDao.DeleteById(mobjToDelete.MinilandObjectId);
                        }

                        // remove all which are saved but not in our current enumerable
                        IEnumerable<ItemInstance> itemInstances = inventories as IList<ItemInstance> ?? inventories.ToList();
                        DaoFactory.Instance.IteminstanceDao.Delete(currentlySavedInventoryIds.Except(itemInstances.Select(i => i.Id)));

                        // create or update all which are new or do still exist
                        foreach (ItemInstance itemInstance in itemInstances.Where(s =>
                            s.Type != InventoryType.Bazaar && s.Type != InventoryType.FamilyWareHouse))
                        {
                            DaoFactory.Instance.IteminstanceDao.InsertOrUpdate(itemInstance);
                            if (!(itemInstance is WearableInstance instance))
                            {
                                continue;
                            }

                            if (instance.EquipmentOptions.Count == 0)
                            {
                                continue;
                            }

                            DaoFactory.Instance.EquipmentOptionDao.DeleteByWearableInstanceId(instance.Id);
                            instance.EquipmentOptions.ForEach(s =>
                            {
                                s.WearableInstanceId = instance.Id;
                                DaoFactory.Instance.EquipmentOptionDao.InsertOrUpdate(s);
                            });
                        }
                    }
                }

                if (Skills != null)
                {
                    IEnumerable<Guid> currentlySavedCharacterSkills =
                        DaoFactory.Instance.CharacterSkillDao.LoadKeysByCharacterId(CharacterId).ToList();

                    DaoFactory.Instance.CharacterSkillDao.Delete(
                        currentlySavedCharacterSkills.Except(Skills.Select(s => s.Value.Id)));

                    DaoFactory.Instance.CharacterSkillDao.InsertOrUpdate(Skills.Values);
                }

                IEnumerable<long> currentlySavedMates =
                    DaoFactory.Instance.MateDao.LoadByCharacterId(CharacterId).Select(s => s.MateId);

                foreach (long matesToDeleteId in currentlySavedMates.Except(Mates.Select(s => s.MateId)))
                {
                    DaoFactory.Instance.MateDao.Delete(matesToDeleteId);
                }

                foreach (Mate mate in Mates)
                {
                    MateDTO matesave = mate;
                    DaoFactory.Instance.MateDao.InsertOrUpdate(ref matesave);
                }

                IEnumerable<QuicklistEntryDTO> quickListEntriesToInsertOrUpdate = QuicklistEntries.ToList();

                IEnumerable<Guid> currentlySavedQuicklistEntries =
                    DaoFactory.Instance.QuicklistEntryDao.LoadKeysByCharacterId(CharacterId).ToList();
                DaoFactory.Instance.QuicklistEntryDao.Delete(
                    currentlySavedQuicklistEntries.Except(QuicklistEntries.Select(s => s.Id)));

                foreach (QuicklistEntryDTO quicklistEntry in quickListEntriesToInsertOrUpdate)
                {
                    DaoFactory.Instance.QuicklistEntryDao.InsertOrUpdate(quicklistEntry);
                }

                IEnumerable<MailDTO> mailDtoToInsertOrUpdate = MailList.Values.ToList();

                IEnumerable<long> currentlySavedMailistEntries =
                    DaoFactory.Instance.MailDao.LoadByCharacterId(CharacterId).Select(s => s.MailId).ToList();
                foreach (long maildtoEntryToDelete in currentlySavedMailistEntries.Except(
                    MailList.Values.Select(s => s.MailId)))
                {
                    DaoFactory.Instance.MailDao.DeleteById(maildtoEntryToDelete);
                }

                foreach (MailDTO mailEntry in mailDtoToInsertOrUpdate)
                {
                    MailDTO save = mailEntry;
                    DaoFactory.Instance.MailDao.InsertOrUpdate(ref save);
                }

                foreach (MinilandObjectDTO mobjEntry in (IEnumerable<MinilandObjectDTO>)Miniland.MapDesignObjects.ToList())
                {
                    MinilandObjectDTO mobj = mobjEntry;
                    DaoFactory.Instance.MinilandObjectDao.InsertOrUpdate(ref mobj);
                }

                IEnumerable<short> currentlySavedBonus = DaoFactory.Instance.StaticBonusDao.LoadTypeByCharacterId(CharacterId);
                foreach (short bonusToDelete in currentlySavedBonus.Except(Buff.Select(s => s.Card.CardId)))
                {
                    DaoFactory.Instance.StaticBonusDao.Delete(bonusToDelete, CharacterId);
                }

                foreach (StaticBonusDTO bonus in StaticBonusList.ToArray())
                {
                    StaticBonusDTO bonus2 = bonus;
                    DaoFactory.Instance.StaticBonusDao.InsertOrUpdate(ref bonus2);
                }

                IEnumerable<short> currentlySavedBuff = DaoFactory.Instance.StaticBuffDao.LoadByTypeCharacterId(CharacterId);
                foreach (short bonusToDelete in currentlySavedBuff.Except(Buff.Select(s => s.Card.CardId)))
                {
                    DaoFactory.Instance.StaticBuffDao.Delete(bonusToDelete, CharacterId);
                }

                //Quest
                DaoFactory.Instance.CharacterQuestDao.LoadByCharacterId(CharacterId).ToList()
                    .ForEach(q => DaoFactory.Instance.CharacterQuestDao.Delete(CharacterId, q.QuestId));
                Quests.ToList().ForEach(qst => DaoFactory.Instance.CharacterQuestDao.InsertOrUpdate(qst));

                foreach (Buff.Buff buff in Buff.Where(s => s.StaticBuff).ToArray())
                {
                    var bf = new StaticBuffDTO
                    {
                        CharacterId = CharacterId,
                        RemainingTime = (int)(buff.RemainingTime - (DateTime.Now - buff.Start).TotalSeconds),
                        CardId = buff.Card.CardId
                    };
                    DaoFactory.Instance.StaticBuffDao.InsertOrUpdate(ref bf);
                }

                foreach (StaticBonusDTO bonus in StaticBonusList.ToArray())
                {
                    StaticBonusDTO bonus2 = bonus;
                    DaoFactory.Instance.StaticBonusDao.InsertOrUpdate(ref bonus2);
                }

                foreach (GeneralLogDTO general in GeneralLogs)
                {
                    if (!DaoFactory.Instance.GeneralLogDao.IdAlreadySet(general.LogId))
                    {
                        DaoFactory.Instance.GeneralLogDao.Insert(general);
                    }
                }

                foreach (RespawnDTO resp in Respawns)
                {
                    RespawnDTO res = resp;
                    if (resp.MapId != 0 && resp.X != 0 && resp.Y != 0)
                    {
                        DaoFactory.Instance.RespawnDao.InsertOrUpdate(ref res);
                    }
                }

                Logger.Log.Info($"[DB] Successfully saved Character {Name}");
            }
            catch (Exception e)
            {
                Logger.Log.Error("Save Character failed. SessionId: " + Session.SessionId, e);
            }
        }

        public void SendGift(long id, short vnum, ushort amount, sbyte rare, byte upgrade, bool isNosmall, byte design = 0)
        {
            Item it = ServerManager.Instance.GetItem(vnum);

            if (it == null)
            {
                return;
            }

            if (it.ItemType != ItemType.Weapon && it.ItemType != ItemType.Armor && it.ItemType != ItemType.Specialist)
            {
                upgrade = 0;
            }
            else if (it.ItemType != ItemType.Weapon && it.ItemType != ItemType.Armor)
            {
                rare = 0;
            }

            if (rare > 8 || rare < -2)
            {
                rare = 0;
            }

            if (upgrade > 10 && it.ItemType != ItemType.Specialist)
            {
                upgrade = 0;
            }
            else if (it.ItemType == ItemType.Specialist && upgrade > 15)
            {
                upgrade = 0;
            }

            // maximum size of the amount is 99
            if (amount > 999)
            {
                amount = 999;
            }

            if (amount == 0)
            {
                amount = 1;
            }

            var mail = new MailDTO
            {
                AttachmentAmount = it.Type == InventoryType.Etc || it.Type == InventoryType.Main ? amount : (byte)1,
                IsOpened = false,
                Date = DateTime.Now,
                ReceiverId = id,
                SenderId = CharacterId,
                AttachmentRarity = (byte)rare,
                AttachmentUpgrade = upgrade,
                IsSenderCopy = false,
                Title = isNosmall ? "NOSMALL" : Name,
                AttachmentVNum = vnum,
                SenderClass = Class,
                SenderGender = Gender,
                SenderHairColor = HairColor,
                SenderHairStyle = HairStyle,
                EqPacket = GenerateEqListForPacket(),
                Design = design,
                SenderMorphId = Morph == 0 ? (short)-1 : (short)(Morph > short.MaxValue ? 0 : Morph)
            };

            CommunicationServiceClient.Instance.SendMail(ServerManager.Instance.ServerGroup, mail);

            if (id != CharacterId)
            {
                return;
            }

            Session.SendPacket(
                GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_GIFTED")} {mail.AttachmentAmount}", 12));
        }

        public void SetRespawnPoint(short mapId, short mapX, short mapY)
        {
            if (!Session.HasCurrentMapInstance || Session.CurrentMapInstance.Map.MapTypes.Count == 0)
            {
                return;
            }

            long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes.ElementAt(0).RespawnMapTypeId;
            if (respawnmaptype == null)
            {
                return;
            }

            RespawnDTO resp = Respawns.FirstOrDefault(s => s.RespawnMapTypeId == respawnmaptype);
            if (resp == null)
            {
                resp = new RespawnDTO
                {
                    CharacterId = CharacterId,
                    MapId = mapId,
                    X = mapX,
                    Y = mapY,
                    RespawnMapTypeId = (long)respawnmaptype
                };
                Respawns.Add(resp);
            }
            else
            {
                resp.X = mapX;
                resp.Y = mapY;
                resp.MapId = mapId;
            }
        }

        public void SetReturnPoint(short mapId, short mapX, short mapY)
        {
            if (!Session.HasCurrentMapInstance || Session.CurrentMapInstance.Map.MapTypes.Count == 0)
            {
                return;
            }

            long? respawnmaptype = Session.CurrentMapInstance.Map.MapTypes.ElementAt(0).ReturnMapTypeId;
            if (respawnmaptype == null)
            {
                return;
            }

            RespawnDTO resp = Respawns.FirstOrDefault(s => s.RespawnMapTypeId == respawnmaptype);
            if (resp == null)
            {
                resp = new RespawnDTO
                {
                    CharacterId = CharacterId,
                    MapId = mapId,
                    X = mapX,
                    Y = mapY,
                    RespawnMapTypeId = (long)respawnmaptype
                };
                Respawns.Add(resp);
            }
            else
            {
                resp.X = mapX;
                resp.Y = mapY;
                resp.MapId = mapId;
            }
        }

        public bool WeaponLoaded(CharacterSkill ski)
        {
            if (ski == null)
            {
                return false;
            }

            WearableInstance inv;
            switch (Class)
            {
                default:
                    return false;

                case ClassType.Adventurer:
                    if (ski.Skill.Type != 1)
                    {
                        return true;
                    }

                    if (Inventory == null)
                    {
                        return true;
                    }

                    var wearable = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.SecondaryWeapon,
                        InventoryType.Wear);
                    if (wearable != null)
                    {
                        if (wearable.Ammo > 0)
                        {
                            wearable.Ammo--;
                            return true;
                        }

                        if (Inventory.CountItem(2081) < 1)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("NO_AMMO_ADVENTURER"), 10));
                            return false;
                        }

                        Inventory.RemoveItemAmount(2081);
                        wearable.Ammo = 100;
                        Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_ADVENTURER"),
                            10));
                        return true;
                    }

                    Session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                    return false;

                case ClassType.Swordman:
                    if (ski.Skill.Type != 1)
                    {
                        return true;
                    }

                    if (Inventory == null)
                    {
                        return true;
                    }

                    inv = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.SecondaryWeapon,
                        InventoryType.Wear);
                    if (inv != null)
                    {
                        if (inv.Ammo > 0)
                        {
                            inv.Ammo--;
                            return true;
                        }

                        if (Inventory.CountItem(2082) < 1)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("NO_AMMO_SWORDSMAN"), 10));
                            return false;
                        }

                        Inventory.RemoveItemAmount(2082);
                        inv.Ammo = 100;
                        Session.SendPacket(
                            GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_SWORDSMAN"), 10));
                        return true;
                    }

                    Session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                    return false;

                case ClassType.Archer:
                    if (ski.Skill.Type != 1)
                    {
                        return true;
                    }

                    if (Inventory == null)
                    {
                        return true;
                    }

                    inv = Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.MainWeapon,
                        InventoryType.Wear);
                    if (inv != null)
                    {
                        if (inv.Ammo > 0)
                        {
                            inv.Ammo--;
                            return true;
                        }

                        if (Inventory.CountItem(2083) < 1)
                        {
                            Session.SendPacket(
                                UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("NO_AMMO_ARCHER"), 10));
                            return false;
                        }

                        Inventory.RemoveItemAmount(2083);
                        inv.Ammo = 100;
                        Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("AMMO_LOADED_ARCHER"), 10));
                        return true;
                    }

                    Session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NO_WEAPON"), 10));
                    return false;

                case ClassType.Magician:
                    return true;
                case ClassType.Wrestler:
                    return true;
            }
        }

        internal void RefreshValidity()
        {
            if (StaticBonusList.RemoveAll(
                s => s.StaticBonusType == StaticBonusType.BackPack && s.DateEnd < DateTime.Now) > 0)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
                Session.SendPacket(GenerateExts());
            }

            if (StaticBonusList.RemoveAll(s => s.DateEnd < DateTime.Now) > 0)
            {
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
            }

            if (Inventory == null)
            {
                return;
            }

            foreach (object suit in Enum.GetValues(typeof(EquipmentType)))
            {
                var item = Inventory.LoadBySlotAndType<WearableInstance>((byte)suit, InventoryType.Wear);
                if (item == null || item.DurabilityPoint <= 0 || item.Item.EquipmentSlot == EquipmentType.Amulet)
                {
                    continue;
                }

                item.DurabilityPoint--;
                if (item.DurabilityPoint != 0)
                {
                    continue;
                }

                Inventory.DeleteById(item.Id);
                Session.SendPacket(GenerateStatChar());
                Session.CurrentMapInstance?.Broadcast(GenerateEq());
                Session.SendPacket(GenerateEquipment());
                Session.SendPacket(GenerateSay(Language.Instance.GetMessageFromKey("ITEM_TIMEOUT"), 10));
            }
        }

        internal void SetSession(ClientSession clientSession)
        {
            Session = clientSession;
        }

        private void GenerateXp(MapMonster monster, bool isMonsterOwner)
        {
            NpcMonster monsterinfo = monster.Monster;
            if (Session.Account.PenaltyLogs.Any(s => s.Penalty == PenaltyType.BlockExp && s.DateEnd > DateTime.Now))
            {
                return;
            }

            Group grp = ServerManager.Instance.Groups.FirstOrDefault(g =>
                g.IsMemberOfGroup(CharacterId) && g.GroupType == GroupType.Group);
            if (Hp <= 0)
            {
                return;
            }

            if ((int)(LevelXp / (XpLoad() / 10)) < (int)((LevelXp + monsterinfo.XP) / (XpLoad() / 10)))
            {
                Hp = (int)HpLoad();
                Mp = (int)MpLoad();
                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateEff(5));
            }

            int xp;
            if (isMonsterOwner)
            {
                xp = (int)(GetXp(monsterinfo, grp) *
                    (1 + GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D));
            }
            else
            {
                xp = (int)(GetXp(monsterinfo, grp) / 3D *
                    (1 + GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D));
            }

            if (Authority >= AuthorityType.Donator)
            {
                xp *= (int)((double)Authority / 100D + 1);
            }

            if (Level < ServerManager.Instance.MaxLevel)
            {
                LevelXp += xp;
            }

            foreach (Mate mate in Mates.Where(x => x.IsTeamMember && x.IsAlive))
            {
                mate.GenerateXp(xp);
            }

            if (Class == 0 && JobLevel < 20 || Class != 0 && JobLevel < ServerManager.Instance.MaxJobLevel)
            {
                if (SpInstance != null && UseSp && SpInstance.SpLevel < ServerManager.Instance.MaxSpLevel &&
                    SpInstance.SpLevel > 19)
                {
                    JobLevelXp += (int)(GetJxp(monsterinfo, grp) / 2D *
                        (1 + GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] /
                            100D));
                }
                else
                {
                    JobLevelXp += (int)(GetJxp(monsterinfo, grp) *
                        (1 + GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] /
                            100D));
                }
            }

            if (SpInstance != null && UseSp && SpInstance.SpLevel < ServerManager.Instance.MaxSpLevel)
            {
                int multiplier = SpInstance.SpLevel < 10 ? 10 :
                    SpInstance.SpLevel < 19 ? 5 : 1;
                SpInstance.XP += (int)(GetJxp(monsterinfo, grp) *
                    (multiplier +
                        GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D) *
                    ((double)Authority / 100 + 1));
            }

            if (HeroLevel > 0 && HeroLevel < ServerManager.Instance.MaxHeroLevel)
            {
                HeroXp += (int)(GetHxp(monsterinfo, grp) *
                    (1 + GetBuff(BCardType.CardType.Item, (byte)AdditionalTypes.Item.EXPIncreased)[0] / 100D) *
                    ((double)Authority / 100 + 1));
            }

            GenerateLevelXpLevelUp();
            var fairy = Inventory?.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Fairy, InventoryType.Wear);
            if (fairy != null)
            {
                if (fairy.ElementRate + fairy.Item.ElementRate < fairy.Item.MaxElementRate &&
                    Level <= monsterinfo.Level + 15 && Level >= monsterinfo.Level - 15)
                {
                    fairy.XP += (int)(ServerManager.Instance.FairyXpRate *
                        (1 + GetBuff(BCardType.CardType.FairyXPIncrease,
                            (byte)AdditionalTypes.FairyXPIncrease.IncreaseFairyXPPoints)[0] / 100D));
                }

                GenerateFairyXpLevelUp();
            }

            GenerateJobXpLevelUp();
            if (SpInstance != null)
            {
                GenerateSpXpLevelUp();
            }

            GenerateHeroXpLevelUp();
            Session.SendPacket(GenerateLev());
        }

        private void GenerateLevelXpLevelUp()
        {
            double t = XpLoad();
            while (LevelXp >= t)
            {
                LevelXp -= (long)t;
                Level++;
                RewardsHelper.Instance.GetLevelUpRewards(Session, LevelType.Level);
                t = XpLoad();
                if (Level >= ServerManager.Instance.MaxLevel)
                {
                    Level = (byte)ServerManager.Instance.MaxLevel;
                    LevelXp = 0;
                }

                if (Level == ServerManager.Instance.HeroicStartLevel && HeroLevel == 0)
                {
                    HeroLevel = 1;
                    HeroXp = 0;
                }

                BattleEntity.Level = Level;
                Hp = (int)HpLoad();
                Mp = (int)MpLoad();
                Session.SendPacket(GenerateStat());
                if (Family != null)
                {
                    if (Level > 20 && (Level % 10) == 0)
                    {
                        Family.InsertFamilyLog(FamilyLogType.LevelUp, Name, level: Level);
                        Family.InsertFamilyLog(FamilyLogType.FamilyXP, Name, experience: 20 * Level);
                        GenerateFamilyXp(20 * Level);
                    }
                    else if (Level > 80)
                    {
                        Family.InsertFamilyLog(FamilyLogType.LevelUp, Name, level: Level);
                    }
                    else
                    {
                        ServerManager.Instance.FamilyRefresh(Family.FamilyId);
                        CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                        {
                            DestinationCharacterId = Family.FamilyId,
                            SourceCharacterId = CharacterId,
                            SourceWorldId = ServerManager.Instance.WorldId,
                            Message = "fhis_stc",
                            Type = MessageType.Family
                        });
                    }
                }

                Session.SendPacket(GenerateLevelUp());
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("LEVELUP"), 0));
                Session.CurrentMapInstance?.Broadcast(GenerateEff(6), PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
                ServerManager.Instance.UpdateGroup(CharacterId);
            }
        }

        private void GenerateFairyXpLevelUp()
        {
            // TODO CLEANUP AND ADD FAIRY PROPERTY
            var fairy = Inventory?.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Fairy, InventoryType.Wear);
            if (fairy == null)
            {
                return;
            }

            double t = CharacterHelper.LoadFairyXpData(fairy.ElementRate + fairy.Item.ElementRate);
            while (fairy.XP >= t)
            {
                fairy.XP -= (int)t;
                fairy.ElementRate++;
                if ((fairy.ElementRate + fairy.Item.ElementRate) == fairy.Item.MaxElementRate)
                {
                    fairy.XP = 0;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("FAIRYMAX"), fairy.Item.Name), 10));
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("FAIRY_LEVELUP"), fairy.Item.Name), 10));
                }

                Session.SendPacket(GeneratePairy());
            }
        }

        private void GenerateJobXpLevelUp()
        {
            double t = JobXpLoad();
            while (JobLevelXp >= t)
            {
                JobLevelXp -= (long)t;
                JobLevel++;
                t = JobXpLoad();
                if (JobLevel >= 20 && Class == 0)
                {
                    JobLevel = 20;
                    JobLevelXp = 0;
                }
                else if (JobLevel >= ServerManager.Instance.MaxJobLevel)
                {
                    JobLevel = (byte)ServerManager.Instance.MaxJobLevel;
                    JobLevelXp = 0;
                }

                Hp = (int)HpLoad();
                Mp = (int)MpLoad();
                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateLevelUp());
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("JOB_LEVELUP"), 0));
                RewardsHelper.Instance.GetLevelUpRewards(Session, LevelType.JobLevel);
                LearnAdventurerSkill();
                Session.CurrentMapInstance?.Broadcast(GenerateEff(8), PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
            }
        }

        private void GenerateSpXpLevelUp()
        {
            double t = SpxpLoad();

            while (UseSp && SpInstance.XP >= t)
            {
                SpInstance.XP -= (long)t;
                SpInstance.SpLevel++;
                t = SpxpLoad();
                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateLevelUp());
                if (SpInstance.SpLevel >= ServerManager.Instance.MaxSpLevel)
                {
                    SpInstance.SpLevel = (byte)ServerManager.Instance.MaxSpLevel;
                    SpInstance.XP = 0;
                }

                LearnSpSkill();
                Skills.Select(s => s.Value).ToList().ForEach(s => s.LastUse = DateTime.Now.AddDays(-1));
                Session.SendPacket(GenerateSki());
                Session.SendPackets(GenerateQuicklist());

                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SP_LEVELUP"), 0));
                Session.CurrentMapInstance?.Broadcast(GenerateEff(8), PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
            }
        }

        private void GenerateHeroXpLevelUp()
        {
            double t = HeroXpLoad();
            while (HeroXp >= t)
            {
                HeroXp -= (long)t;
                HeroLevel++;
                RewardsHelper.Instance.GetLevelUpRewards(Session, LevelType.Heroic);
                t = HeroXpLoad();
                if (HeroLevel >= ServerManager.Instance.MaxHeroLevel)
                {
                    HeroLevel = (byte)ServerManager.Instance.MaxHeroLevel;
                    HeroXp = 0;
                }

                Hp = (int)HpLoad();
                Mp = (int)MpLoad();
                Session.SendPacket(GenerateStat());
                Session.SendPacket(GenerateLevelUp());
                Session.SendPacket(
                    UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("HERO_LEVELUP"), 0));
                Session.CurrentMapInstance?.Broadcast(GenerateEff(8), PositionX, PositionY);
                Session.CurrentMapInstance?.Broadcast(GenerateEff(198), PositionX, PositionY);
            }
        }

        private int GetGold(MapMonster mapMonster)
        {
            if (!(MapInstance.MapInstanceType == MapInstanceType.BaseMapInstance ||
                MapInstance.MapInstanceType == MapInstanceType.TimeSpaceInstance) || mapMonster.Monster == null)
            {
                return 0;
            }

            int lowBaseGold = ServerManager.Instance.RandomNumber(6 * mapMonster.Monster?.Level ?? 1,
                (int)(12 * mapMonster.Monster?.Level));
            if (Session.CurrentMapInstance?.Map.MapTypes.Any(s => s.MapTypeId == (short)MapTypeEnum.CometPlain) == true)
            {
                return ServerManager.Instance.RandomNumber(1, 1000);
            }

            return lowBaseGold * ServerManager.Instance.GoldRate;
        }

        private int GetHxp(NpcMonsterDTO monster, Group group)
        {
            int partySize = 1;
            float partyPenalty = 1f;

            if (group != null)
            {
                int levelSum = group.Characters.Sum(g => g.Character.HeroLevel);
                partySize = group.CharacterCount;
                partyPenalty = partySize > 2 ? 3f / levelSum : 4f / levelSum;
            }

            int heroXp = (int)Math.Round(monster.HeroXp * CharacterHelper.ExperiencePenalty(Level, monster.Level) * ServerManager.Instance.HeroXpRate * MapInstance.XpRate);

            if (partySize > 1 && group != null)
            {
                heroXp = (int)Math.Round(heroXp / (HeroLevel * partyPenalty));
            }

            return heroXp;
        }

        private int GetJxp(NpcMonsterDTO monster, Group group)
        {
            int partySize = 1;
            float partyPenalty = 1f;

            if (group != null)
            {
                int levelSum = group.Characters.Sum(g => g.Character.JobLevel);
                partySize = group.CharacterCount;
                partyPenalty = partySize > 2 ? 3f / levelSum : 4f / levelSum;
            }

            int jobxp = (int)Math.Round(monster.JobXP * CharacterHelper.ExperiencePenalty(JobLevel, monster.Level) *
                ServerManager.Instance.XpRate * MapInstance.XpRate);

            // divide jobexp by multiplication of partyPenalty with level e.g. 57 * 0,014...
            if (partySize > 1 && group != null)
            {
                jobxp = (int)Math.Round(jobxp / (JobLevel * partyPenalty));
            }

            return jobxp;
        }

        private long GetXp(NpcMonsterDTO monster, Group group)
        {
            int partySize = 1;
            double partyPenalty = 1d;
            int levelDifference = Level - monster.Level;

            if (group != null)
            {
                int levelSum = group.Characters.Sum(g => g.Character.Level);
                partySize = group.CharacterCount;
                partyPenalty = partySize > 2 ? 3f / levelSum : 4f / levelSum;
            }

            long xpcalculation = levelDifference < 5 ? monster.XP : monster.XP / 3 * 2;

            long xp = (long)Math.Round(xpcalculation * CharacterHelper.ExperiencePenalty(Level, monster.Level) *
                ServerManager.Instance.XpRate * MapInstance.XpRate);

            // bonus percentage calculation for level 1 - 5 and difference of levels bigger or equal
            // to 4
            if (levelDifference < -20)
            {
                xp /= 10;
            }

            if (Level <= 5 && levelDifference < -4)
            {
                xp += xp / 2;
            }

            if (monster.Level >= 75)
            {
                xp *= 2;
            }

            if (monster.Level >= 100)
            {
                xp *= 2;
                if (Level < 96)
                {
                    xp = 1;
                }
            }

            if (partySize > 1 && group != null)
            {
                xp = (long)Math.Round(xp / (Level * partyPenalty));
            }

            return xp;
        }

        private int HealthHpLoad()
        {
            int regen = GetBuff(BCardType.CardType.Recovery, (byte)AdditionalTypes.Recovery.HPRecoveryIncreased)[0];
            regen -= GetBuff(BCardType.CardType.Recovery, (byte)AdditionalTypes.Recovery.HPRecoveryDecreased)[0];
            if (IsSitting)
            {
                return CharacterHelper.Instance.HpHealth[(byte)Class] + regen;
            }

            return (DateTime.Now - LastDefence).TotalSeconds > 4
                ? CharacterHelper.Instance.HpHealthStand[(byte)Class] + regen
                : 0;
        }

        private int HealthMpLoad()
        {
            int regen = GetBuff(BCardType.CardType.Recovery, (byte)AdditionalTypes.Recovery.MPRecoveryIncreased)[0];
            regen -= GetBuff(BCardType.CardType.Recovery, (byte)AdditionalTypes.Recovery.MPRecoveryDecreased)[0];
            if (IsSitting)
            {
                return CharacterHelper.Instance.MpHealth[(byte)Class] + regen;
            }

            return (DateTime.Now - LastDefence).TotalSeconds > 4
                ? CharacterHelper.Instance.MpHealthStand[(byte)Class] + regen
                : 0;
        }

        private double HeroXpLoad() => HeroLevel == 0 ? 1 : CharacterHelper.Instance.HeroXpData[HeroLevel - 1];

        private double JobXpLoad() => Class == (byte)ClassType.Adventurer
            ? CharacterHelper.Instance.FirstJobXpData[JobLevel - 1]
            : CharacterHelper.Instance.SecondJobXpData[JobLevel - 1];

        private double SpxpLoad() => SpInstance != null
            ? CharacterHelper.Instance.SpxpData[SpInstance.SpLevel == 0 ? 0 : SpInstance.SpLevel - 1]
            : 0;

        private double XpLoad() => CharacterHelper.Instance.XpData[Level - 1];

        public string GenerateRaid(int type, bool exit)
        {
            string result = string.Empty;
            switch (type)
            {
                case 0:
                    result = "raid 0";
                    Group?.Characters?.ToList().ForEach(s => { result += $" {s.Character?.CharacterId}"; });
                    break;
                case 2:
                    result = $"raid 2 {(exit ? "-1" : $"{CharacterId}")}";
                    break;
                case 1:
                    result = $"raid 1 {(exit ? 0 : 1)}";
                    break;
                case 3:
                    result = "raid 3";
                    Group?.Characters?.Where(p => p.Character != null).ToList().ForEach(s =>
                    {
                        if (s.Character != null)
                        {
                            result +=
                                $" {s.Character?.CharacterId}.{(int)(s.Character?.Hp / s.Character?.HpLoad() * 100)}.{(int)(s.Character.Mp / s.Character.MpLoad() * 100)}";
                        }
                    });
                    break;
                case 4:
                    result = "raid 4";
                    break;
                case 5:
                    result = "raid 5 1";
                    break;
            }

            return result;
        }

        public void AddStaticBuff(StaticBuffDTO staticBuff, bool isPermaBuff = false)
        {
            var bf = new Buff.Buff(staticBuff.CardId, Session.Character.Level, isPermaBuff)
            {
                Start = DateTime.Now,
                StaticBuff = true
            };
            Buff.Buff oldbuff = Buff.FirstOrDefault(s => s.Card.CardId == staticBuff.CardId);
            if (oldbuff?.IsPermaBuff == true)
            {
                return;
            }

            if (staticBuff.RemainingTime <= 0)
            {
                bf.RemainingTime = (int)(bf.Card.Duration * 0.6 <= 0 ? 3600 : bf.Card.Duration * 0.6);
                AddBuff(bf);
            }
            else if (staticBuff.RemainingTime > 0)
            {
                bf.RemainingTime = staticBuff.CardId == 340
                    ? TotalTime += staticBuff.RemainingTime
                    : staticBuff.RemainingTime;
                AddBuff(bf);
            }
            else if (oldbuff != null)
            {
                RemoveBuff(bf.Card.CardId);
                bf.RemainingTime = (int)(bf.Card.Duration * 0.6) + oldbuff.RemainingTime;
                AddBuff(bf);
            }
            else
            {
                bf.RemainingTime = (int)(bf.Card.Duration * 0.6);
                AddBuff(bf);
            }

            Session.SendPacket(bf.RemainingTime == -1
                ? $"vb {bf.Card.CardId} 1 -1"
                : $"vb {bf.Card.CardId} 1 {bf.RemainingTime * 10}");
            Session.SendPacket(Session.Character.GenerateSay(
                string.Format(Language.Instance.GetMessageFromKey("UNDER_EFFECT"), bf.Card.Name), 12));
        }

        public string GenerateTaPs()
        {
            List<ArenaTeamMember> arenateam = ServerManager.Instance.ArenaTeams
                .FirstOrDefault(s => s != null && s.Any(o => o?.Session == Session))?.OrderBy(s => s.ArenaTeamType)
                .ToList();
            string groups = string.Empty;
            if (arenateam == null)
            {
                return $"ta_ps {groups.TrimEnd(' ')}";
            }

            ArenaTeamType type = arenateam.FirstOrDefault(s => s.Session == Session)?.ArenaTeamType ??
                ArenaTeamType.ERENIA;

            for (byte i = 0; i < 6; i++)
            {
                ArenaTeamMember arenamembers = arenateam.FirstOrDefault(s =>
                    (i < 3 ? s.ArenaTeamType == type : s.ArenaTeamType != type) && s.Order == (i % 3));
                if (arenamembers != null)
                {
                    groups +=
                        $"{arenamembers.Session.Character.CharacterId}.{(int)(arenamembers.Session.Character.Hp / arenamembers.Session.Character.HpLoad() * 100)}.{(int)(arenamembers.Session.Character.Mp / arenamembers.Session.Character.MpLoad() * 100)}.0 ";
                }
                else
                {
                    groups += "-1.-1.-1.-1.-1 ";
                }
            }

            return $"ta_ps {groups.TrimEnd(' ')}";
        }

        public string GenerateTaP(byte tatype, bool showOponent)
        {
            List<ArenaTeamMember> arenateam = ServerManager.Instance.ArenaTeams
                .FirstOrDefault(s => s != null && s.Any(o => o != null && o.Session == Session))
                ?.OrderBy(s => s.ArenaTeamType).ToList();
            var type = ArenaTeamType.ERENIA;
            string groups = string.Empty;
            if (arenateam == null)
            {
                return
                    $"ta_p {tatype} {(byte)type} {5} {5} {groups.TrimEnd(' ')}";
            }

            type = arenateam.FirstOrDefault(s => s.Session == Session)?.ArenaTeamType ?? ArenaTeamType.ERENIA;

            for (byte i = 0; i < 6; i++)
            {
                ArenaTeamMember arenamembers = arenateam.FirstOrDefault(s =>
                    (i < 3 ? s.ArenaTeamType == type : s.ArenaTeamType != type) && s.Order == (i % 3));
                if (arenamembers != null && (i <= 2 || showOponent))
                {
                    groups +=
                        $"{(arenamembers.Dead ? 0 : 1)}.{arenamembers.Session.Character.CharacterId}.{(byte)arenamembers.Session.Character.Class}.{(byte)arenamembers.Session.Character.Gender}.{(byte)arenamembers.Session.Character.Morph} ";
                }
                else
                {
                    groups += "-1.-1.-1.-1.-1 ";
                }
            }

            return
                $"ta_p {tatype} {(byte)type} {5 - arenateam.Where(s => s.ArenaTeamType == type).Sum(s => s.SummonCount)} {5 - arenateam.Where(s => s.ArenaTeamType != type).Sum(s => s.SummonCount)} {groups.TrimEnd(' ')}";
        }

        public int[] GetWeaponSoftDamage()
        {
            int increase = 0;
            int increaseChance = 0;

            WearableInstance prim = Inventory.PrimaryWeapon;
            WearableInstance sec = Inventory.SecondaryWeapon;

            if (prim != null)
            {
                foreach (BCard bcard in prim.Item.BCards.Where(
                    s => s != null && s.Type.Equals((byte)BCardType.CardType.IncreaseDamage) &&
                        s.SubType.Equals((byte)AdditionalTypes.IncreaseDamage.IncreasingPropability)))
                {
                    increaseChance += bcard.FirstData;
                    increase += bcard.SecondData;
                }
            }

            if (sec != null)
            {
                foreach (BCard bcard in sec.Item.BCards.Where(
                    s => s != null && s.Type.Equals((byte)BCardType.CardType.IncreaseDamage) &&
                        s.SubType.Equals((byte)AdditionalTypes.IncreaseDamage.IncreasingPropability)))
                {
                    increaseChance += bcard.FirstData;
                    increase += bcard.SecondData;
                }
            }

            return new[] { increaseChance, increase };
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public int GetMostValueEquipmentBuff(BCardType.CardType type, byte subtype)
        {
            return BattleEntity.StaticBcards.Where(s => s.Type == (byte)type && s.SubType.Equals(subtype))
                .OrderByDescending(s => s.FirstData).FirstOrDefault()?.FirstData ?? 0;
        }

        /// <summary>
        ///     Get Stuff Buffs
        ///     Useful for Stats for example
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        /// <param name="pvp"></param>
        /// <param name="affectingOpposite"></param>
        /// <returns></returns>
        private int[] GetStuffBuff(BCardType.CardType type, byte subtype, bool pvp, bool affectingOpposite = false)
        {
            int value1 = 0;
            int value2 = 0;
            foreach (BCard entry in BattleEntity.StaticBcards.Where(
                s => s.Type.Equals((byte)type) && s.SubType.Equals((byte)(subtype / 10))))
            {
                if (entry.IsLevelScaled)
                {
                    value1 += entry.FirstData * Level;
                }
                else
                {
                    value1 += entry.FirstData;
                }

                value2 += entry.SecondData;
            }

            return new[] { value1, value2 };
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GenerateTaM(int type)
        {
            ConcurrentBag<ArenaTeamMember> tm =
                ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(o => o.Session == Session));
            int score1 = 0;
            int score2 = 0;
            if (tm == null)
            {
                return
                    $"ta_m {type} {score1} {score2} {(type == 3 ? MapInstance.InstanceBag.Clock.DeciSecondRemaining / 10 : 0)} 0";
            }

            ArenaTeamMember tmem = tm.FirstOrDefault(s => s.Session == Session);
            IEnumerable<long> ids = tm.Where(s => tmem != null && tmem.ArenaTeamType != s.ArenaTeamType)
                .Select(s => s.Session.Character.CharacterId);
            score1 = MapInstance.InstanceBag.DeadList.Count(s => ids.Contains(s));
            score2 = MapInstance.InstanceBag.DeadList.Count(s => !ids.Contains(s));
            return
                $"ta_m {type} {score1} {score2} {(type == 3 ? MapInstance.InstanceBag.Clock.DeciSecondRemaining / 10 : 0)} 0";
        }

        public string GenerateTaF(byte victoriousteam)
        {
            ConcurrentBag<ArenaTeamMember> tm =
                ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(o => o.Session == Session));
            int score1 = 0;
            int score2 = 0;
            int life1 = 0;
            int life2 = 0;
            int call1 = 0;
            int call2 = 0;
            var atype = ArenaTeamType.ERENIA;
            if (tm == null)
            {
                return $"ta_f 0 {victoriousteam} {(byte)atype} {score1} {life1} {call1} {score2} {life2} {call2}";
            }

            ArenaTeamMember tmem = tm.FirstOrDefault(s => s.Session == Session);
            if (tmem == null)
            {
                return $"ta_f 0 {victoriousteam} {(byte)atype} {score1} {life1} {call1} {score2} {life2} {call2}";
            }

            atype = tmem.ArenaTeamType;
            IEnumerable<long> ids = tm.Where(s => tmem.ArenaTeamType == s.ArenaTeamType)
                .Select(s => s.Session.Character.CharacterId);
            tm.RemoveWhere(s => tmem.ArenaTeamType != s.ArenaTeamType, out ConcurrentBag<ArenaTeamMember> t);
            ConcurrentBag<ArenaTeamMember> oposit = t;
            tm.RemoveWhere(s => tmem.ArenaTeamType == s.ArenaTeamType, out ConcurrentBag<ArenaTeamMember> t2);
            ConcurrentBag<ArenaTeamMember> own = t2;
            score1 = 3 - MapInstance.InstanceBag.DeadList.Count(s => ids.Contains(s));
            score2 = 3 - MapInstance.InstanceBag.DeadList.Count(s => !ids.Contains(s));
            life1 = 3 - own.Count(s => s.Dead);
            life2 = 3 - oposit.Count(s => s.Dead);
            call1 = 5 - own.Sum(s => s.SummonCount);
            call2 = 5 - oposit.Sum(s => s.SummonCount);
            return $"ta_f 0 {victoriousteam} {(byte)atype} {score1} {life1} {call1} {score2} {life2} {call2}";
        }

        public void LeaveTalentArena(bool surrender = false)
        {
            ArenaMember memb = ServerManager.Instance.ArenaMembers.FirstOrDefault(s => s.Session == Session);
            if (memb != null)
            {
                if (memb.GroupId != null)
                {
                    lock(ServerManager.Instance.ArenaMembers)
                    {
                        ServerManager.Instance.ArenaMembers.Where(s => s.GroupId == memb.GroupId).ToList().ForEach(s =>
                        {
                            if (ServerManager.Instance.ArenaMembers.Count(g => g.GroupId == memb.GroupId) == 2)
                            {
                                s.GroupId = null;
                            }

                            s.Time = 300;
                            s.Session.SendPacket(s.Session.Character.GenerateBsInfo(1, 2, s.Time, 8));
                            s.Session.SendPacket(
                                s.Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ARENA_TEAM_LEAVE"),
                                    11));
                        });
                    }
                }

                ServerManager.Instance.ArenaMembers.Remove(memb);
                Session.SendPacket(Session.Character.GenerateBsInfo(2, 2, 0, 0));
            }

            ConcurrentBag<ArenaTeamMember> tm =
                ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(o => o.Session == Session));
            Session.SendPacket(Session.Character.GenerateTaM(1));
            if (tm == null)
            {
                return;
            }

            ArenaTeamMember tmem = tm.FirstOrDefault(s => s.Session == Session);
            if (tmem != null)
            {
                tmem.Dead = true;
                if (surrender)
                {
                    Session.Character.TalentSurrender++;
                }

                tm.ToList().ForEach(s =>
                {
                    if (s.ArenaTeamType == tmem.ArenaTeamType)
                    {
                        s.Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                            string.Format(Language.Instance.GetMessageFromKey("ARENA_TALENT_LEFT"),
                                Session.Character.Name), 0));
                    }

                    s.Session.SendPacket(s.Session.Character.GenerateTaP(2, true));
                });
                Session.SendPacket(Session.Character.GenerateTaP(1, true));
                Session.SendPacket("ta_sv 1");
                Session.SendPacket("taw_sv 1");
            }

            List<BuffType> bufftodisable = new List<BuffType> { BuffType.Bad, BuffType.Good, BuffType.Neutral };
            Session.Character.DisableBuffs(bufftodisable);
            Session.Character.Hp = (int)Session.Character.HpLoad();
            Session.Character.Mp = (int)Session.Character.MpLoad();
            ServerManager.Instance.ArenaTeams.Remove(tm);
            tm.RemoveWhere(s => s.Session != Session, out tm);
            if (tm.Count > 0)
            {
                ServerManager.Instance.ArenaTeams.Add(tm);
            }
        }

        public void TeleportOnMap(short x, short y)
        {
            Session.Character.PositionX = x;
            Session.Character.PositionY = y;
            Session.CurrentMapInstance.Broadcast($"tp {1} {CharacterId} {x} {y} 0");
            Session.SendPacket(GenerateCond());
        }

        #endregion
    }
}