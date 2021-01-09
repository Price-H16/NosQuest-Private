﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Npc;
using OpenNos.GameObject.Skills;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;
using WingsEmu.Packets.ServerPackets;
using WingsEmu.Pathfinder.PathFinder;

namespace OpenNos.GameObject.Maps
{
    public class MapMonster : MapMonsterDTO, IBattleEntity
    {
        #region Instantiation

        public MapMonster()
        {
            OnNoticeEvents = new ConcurrentBag<EventContainer>();
            ReflectiveBuffs = new ConcurrentDictionary<short, int?>();
        }

        #endregion

        #region Members

        private int _movetime;
        private Random _random;
        private const int _maxDistance = 25;

        #endregion

        #region Properties

        #region BattleEntityProperties

        public bool IsMateTrainer { get; set; }

        public BattleEntity BattleEntity { get; set; }

        public void AddBuff(Buff.Buff indicator) => BattleEntity.AddBuff(indicator);

        public void RemoveBuff(short cardId, bool removePermaBuff) => BattleEntity.RemoveBuff(cardId, removePermaBuff);

        public void RemoveBuff(short cardId) => RemoveBuff(cardId, false);

        public int[] GetBuff(BCardType.CardType type, byte subtype) => BattleEntity.GetBuff(type, subtype);

        public bool HasBuff(BCardType.CardType type, byte subtype, bool removeWeaponEffects = false) =>
            BattleEntity.HasBuff(type, subtype, removeWeaponEffects);

        public bool HasBuff(BuffType type) => BattleEntity.HasBuff(type);

        public ConcurrentBag<Buff.Buff> Buffs => BattleEntity.Buffs;

        public int MaxHp => Monster.MaxHP;

        #endregion

        public ConcurrentDictionary<short, int?> ReflectiveBuffs { get; set; }

        public int DealtDamage { get; set; }

        public int CurrentHp { get; set; }

        public int CurrentMp { get; set; }

        public ConcurrentDictionary<IBattleEntity, long> DamageList { get; private set; }

        public DateTime Death { get; set; }

        public bool IsAlive { get; set; }

        public bool IsFactionTargettable(FactionType faction) => !(MonsterVNum == 679 & faction == FactionType.Angel | MonsterVNum == 680 & faction == FactionType.Demon);

        public FactionType Faction { get; set; }

        public bool IsInvicible => MonsterVNum == 679 || MonsterVNum == 680;

        public bool IsBonus { get; set; }

        public bool IsBoss { get; set; }

        public byte NoticeRange { get; set; }

        public bool IsHostile { get; set; }

        public bool IsTarget { get; set; }

        public DateTime LastEffect { get; set; }

        public DateTime LastMove { get; set; }

        public DateTime LastSkill { get; set; }

        public IDisposable LifeEvent { get; set; }

        public MapInstance MapInstance { get; set; }

        public NpcMonster Monster { get; private set; }

        public ConcurrentBag<EventContainer> OnNoticeEvents { get; set; }

        public ZoneEvent MoveEvent { get; set; }

        public List<Node> Path { get; set; }

        public bool? ShouldRespawn { get; set; }

        public ConcurrentBag<NpcMonsterSkill> Skills { get; set; } = new ConcurrentBag<NpcMonsterSkill>();

        public bool Started { get; internal set; }

        public IBattleEntity Target { get; set; }

        public short FirstX { get; set; }

        public short FirstY { get; set; }

        public bool IsPercentage { get; set; }

        public int TakesDamage { get; set; }

        public int GiveDamagePercent { get; set; }

        public IDisposable Life { get; set; }

        #endregion

        #region Methods

        public string GenerateDm(ushort damage) => $"dm 3 {MapMonsterId} {damage}";

        public string GenerateRc(int heal) => $"rc 3 {MapMonsterId} {heal} 0";

        public EffectPacket GenerateEff(int effectid) => new EffectPacket
        {
            EffectType = 3,
            CharacterId = MapMonsterId,
            Id = effectid
        };

        public string GenerateIn()
        {
            if (IsAlive && !IsDisabled)
            {
                return
                    $"in 3 {MonsterVNum} {MapMonsterId} {MapX} {MapY} {Position} {(int)(CurrentHp / (float)Monster.MaxHP * 100)} {(int)(CurrentMp / (float)Monster.MaxMP * 100)} 0 0 0 -1 {(Monster.NoAggresiveIcon ? (byte)InRespawnType.NoEffect : (byte)InRespawnType.TeleportationEffect)} 0 -1 - 0 -1 0 0 0 0 0 0 0 0";
            }

            return string.Empty;
        }

        public string GenerateOut() => $"out 3 {MapMonsterId}";

        public string GenerateSay(string message, int type) => $"say 3 {MapMonsterId} {type} {message}";

        public void Initialize(MapInstance currentMapInstance)
        {
            MapInstance = currentMapInstance;
            Initialize();
        }

        public override void Initialize()
        {
            FirstX = MapX;
            FirstY = MapY;
            Life = null;
            LastSkill = LastMove = LastEffect = DateTime.Now;
            Target = null;
            Path = new List<Node>();
            IsAlive = true;
            ShouldRespawn = ShouldRespawn ?? true;
            Monster = ServerManager.Instance.GetNpc(MonsterVNum);
            IsHostile = Monster.IsHostile;
            CurrentHp = Monster.MaxHP;
            CurrentMp = Monster.MaxMP;
            Monster.Skills.ForEach(s => Skills.Add(s));
            BattleEntity = new BattleEntity(this);
            DamageList = new ConcurrentDictionary<IBattleEntity, long>();
            _random = new Random(MapMonsterId);
            _movetime = ServerManager.Instance.RandomNumber(400, 3200);
            IsPercentage = Monster.IsPercent;
            TakesDamage = Monster.TakeDamages;
            GiveDamagePercent = Monster.GiveDamagePercentage;
            Faction = MonsterVNum == 679 ? FactionType.Angel :
                MonsterVNum == 680 ? FactionType.Demon : FactionType.Neutral;
        }

        /// <summary>
        ///     Check if the Monster is in the given Range.
        /// </summary>
        /// <param name="mapX">The X coordinate on the Map of the object to check.</param>
        /// <param name="mapY">The Y coordinate on the Map of the object to check.</param>
        /// <param name="distance">The maximum distance of the object to check.</param>
        /// <returns>True if the Monster is in range, False if not.</returns>
        public bool IsInRange(short mapX, short mapY, byte distance) => Map.GetDistance(new MapCell { X = mapX, Y = mapY }, GetPos()) <= distance + 1;

        /// <summary>
        ///     Start life
        /// </summary>
        public void StartLife()
        {
            if (MapInstance?.IsSleeping == false && Life == null)
            {
                Life = Observable.Interval(TimeSpan.FromMilliseconds(400)).Subscribe(x =>
                {
                    try
                    {
                        if (MapInstance?.IsSleeping == false)
                        {
                            MonsterLife();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                });
            }
        }

        public void StopLife()
        {
            Life?.Dispose();
            Life = null;
        }

        /// <summary>
        ///     Get the Nearest Oponent
        /// </summary>
        internal void GetNearestOponent()
        {
            Target = DamageList.Keys.ToList().OrderBy(e => Map.GetDistance(GetPos(), e.GetPos()))
                .FirstOrDefault(e => e.IsTargetable(SessionType()));
        }

        /// <summary>
        ///     Hostility on actual Target
        /// </summary>
        internal void HostilityTarget()
        {
            if (!IsHostile || Target != null)
            {
                return;
            }

            IBattleEntity target = MapInstance.BattleEntities.FirstOrDefault(e =>
                e.IsTargetable(SessionType()) && IsFactionTargettable(e.Faction) &&
                Map.GetDistance(GetPos(), e.GetPos()) < (NoticeRange == 0 ? Monster.NoticeRange : NoticeRange));

            if (target == null || MoveEvent != null)
            {
                return;
            }

            if (OnNoticeEvents.Count > 0)
            {
                OnNoticeEvents.ToList().ForEach(e => { EventHelper.Instance.RunEvent(e, monster: this); });
                OnNoticeEvents.Clear();
                return;
            }

            Target = target;
            if (!Monster.NoAggresiveIcon && LastEffect.AddSeconds(5) < DateTime.Now &&
                target.GetSession() is Character.Character character)
            {
                character?.Session.SendPacket(GenerateEff(5000));
            }
        }

        /// <summary>
        ///     Remove the current Target from Monster.
        /// </summary>
        internal void RemoveTarget()
        {
            GetNearestOponent();
            if (Target != null)
            {
                Path.Clear();
                return;
            }

            Path = BestFirstSearch.FindPathJagged(new Node { X = MapX, Y = MapY }, new Node { X = FirstX, Y = FirstY },
                MapInstance.Map.Grid); // Path To origins
        }

        /// <summary>
        ///     Follow the Monsters target to it's position.
        /// </summary>
        /// <param name="targetSession">The TargetSession to follow</param>
        private void FollowTarget()
        {
            if (Monster == null || !IsAlive || HasBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible) ||
                !IsMoving)
            {
                return;
            }

            if (!Target?.IsTargetable(SessionType()) ?? true)
            {
                RemoveTarget();
                return;
            }

            if (Path.Count == 0 && Target.MapInstance != null)
            {
                Path = BestFirstSearch.TracePathJagged(new Node { X = MapX, Y = MapY }, Target.GetBrushFire(),
                    MapInstance.Map.Grid);
            }

            Move(); // follow the target
        }

        /// <summary>
        ///     Generate the mv 3 packet
        /// </summary>
        /// <returns>string mv 3 packet</returns>
        private string GenerateMv3() => $"mv 3 {MapMonsterId} {MapX} {MapY} {Monster.Speed}";

        /// <summary>
        ///     Handle any kind of Monster interaction
        /// </summary>
        private void MonsterLife()
        {
            if (Monster == null || MapInstance == null)
            {
                return;
            }

            ShowEffect();
            if (!IsAlive) // Respawn
            {
                if (ShouldRespawn == null || !ShouldRespawn.Value)
                {
                    Life.Dispose();
                }
                else if ((DateTime.Now - Death).TotalSeconds >= Monster.RespawnTime / 10d)
                {
                    Respawn();
                }

                return;
            }

            if (Target == null) // basic move
            {
                Move();
                return;
            }

            lock(Target)
            {
                NpcMonsterSkill npcMonsterSkill = null;
                if (ServerManager.Instance.RandomNumber(0, 10) > 8 && Skills != null)
                {
                    npcMonsterSkill = Skills
                        .Where(s => (DateTime.Now - s.LastSkillUse).TotalMilliseconds >= 100 * s.Skill.Cooldown)
                        .OrderBy(s => _random.Next()).FirstOrDefault();
                }

                if (npcMonsterSkill?.Skill.TargetType == 1 && npcMonsterSkill?.Skill.HitType == 0)
                {
                    TargetHit(npcMonsterSkill);
                }

                // check if target is in range & if monster has enough mp to use the skill
                if (IsAlive && Life != null && Target != null && CurrentMp >= (npcMonsterSkill?.Skill?.MpCost ?? CurrentMp) &&
                    Map.GetDistance(GetPos(), Target.GetPos()) <=
                    (npcMonsterSkill?.Skill.Range + 1 ?? Monster?.BasicRange))
                {
                    TargetHit(npcMonsterSkill);
                    return;
                }

                FollowTarget();
            }
        }


        /// <summary>
        ///     Broadcast effects applied on the current MapInstance
        /// </summary>
        public void ShowEffect()
        {
            if (!((DateTime.Now - LastEffect).TotalSeconds >= 5))
            {
                return;
            }

            if (IsTarget)
            {
                MapInstance.Broadcast(GenerateEff(824));
            }

            if (IsBonus)
            {
                MapInstance.Broadcast(GenerateEff(826));
            }

            LastEffect = DateTime.Now;
        }

        /// <summary>
        ///     Generate rboss packet
        /// </summary>
        /// <returns>string rboss 3 packet</returns>
        public string GenerateBoss() => $"rboss 3 {MapMonsterId} {CurrentHp} {Monster.MaxHP}";

        private void Move()
        {
            if (Monster == null || !IsAlive || HasBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible))
            {
                return;
            }

            if (IsMoving && Monster.Speed > 0)
            {
                if (Path.Count == 0 && (DateTime.Now - LastMove).TotalMilliseconds > _movetime && Target == null
                ) // Basic Move
                {
                    short mapX = FirstX, mapY = FirstY;
                    if (MapInstance.Map?.GetFreePosition(ref mapX, ref mapY,
                        (byte)ServerManager.Instance.RandomNumber(0, 2), (byte)_random.Next(0, 2)) ?? false)
                    {
                        int distance = Map.GetDistance(new MapCell { X = mapX, Y = mapY }, GetPos());
                        double value = 1000d * distance / (2 * Monster.Speed);
                        Observable.Timer(TimeSpan.FromMilliseconds(value)).Subscribe(x =>
                        {
                            MapX = mapX;
                            MapY = mapY;
                        });
                        LastMove = DateTime.Now.AddMilliseconds(value);
                        MapInstance.Broadcast(new BroadcastPacket(null, GenerateMv3(), ReceiverType.All));
                    }
                }
                else if (DateTime.Now > LastMove && Path.Count > 0) // Follow target || move back to original pos
                {
                    byte speedIndex = (byte)(Monster.Speed / 2 < 1 ? 1 : Monster.Speed / 2);
                    int maxindex = Path.Count > speedIndex ? speedIndex : Path.Count;
                    short smapX = Path[maxindex - 1].X;
                    short smapY = Path[maxindex - 1].Y;
                    double waitingtime = Map.GetDistance(new MapCell { X = smapX, Y = smapY }, GetPos()) /
                        (double)Monster.Speed;
                    MapInstance.Broadcast(new BroadcastPacket(null,
                        $"mv 3 {MapMonsterId} {smapX} {smapY} {Monster.Speed}", ReceiverType.All, xCoordinate: smapX,
                        yCoordinate: smapY));
                    LastMove = DateTime.Now.AddSeconds(waitingtime > 1 ? 1 : waitingtime);
                    Observable.Timer(TimeSpan.FromMilliseconds((int)((waitingtime > 1 ? 1 : waitingtime) * 1000)))
                        .Subscribe(x =>
                        {
                            MapX = smapX;
                            MapY = smapY;
                        });
                    if (Target != null && (int)Path[0].F > _maxDistance
                    ) // Remove Target if distance between target & monster is > max Distance
                    {
                        RemoveTarget();
                        return;
                    }

                    if (maxindex <= Path.Count)
                    {
                        Path.RemoveRange(0, maxindex);
                    }
                }
            }

            HostilityTarget();
        }

        /// <summary>
        ///     Start the Respawn
        /// </summary>
        private void Respawn()
        {
            if (Monster == null)
            {
                return;
            }

            DamageList = new ConcurrentDictionary<IBattleEntity, long>();
            IsAlive = true;
            Target = null;
            CurrentHp = Monster.MaxHP;
            CurrentMp = Monster.MaxMP;
            MapX = FirstX;
            MapY = FirstY;
            Path = new List<Node>();
            MapInstance.Broadcast(GenerateIn());
            Monster.BCards.ForEach(s => s.ApplyBCards(this, this));
        }

        public void TargetHit(NpcMonsterSkill npcMonsterSkill)
        {
            if (Monster == null ||
                !((DateTime.Now - LastSkill).TotalMilliseconds >= 1000 + Monster.BasicCooldown * 250) &&
                npcMonsterSkill == null ||
                HasBuff(BCardType.CardType.SpecialAttack, (byte)AdditionalTypes.SpecialAttack.NoAttack))
            {
                return;
            }

            LastSkill = DateTime.Now;
            if (npcMonsterSkill != null)
            {
                if (CurrentMp < npcMonsterSkill.Skill.MpCost)
                {
                    FollowTarget();
                    return;
                }

                npcMonsterSkill.LastSkillUse = DateTime.Now;
                CurrentMp -= npcMonsterSkill.Skill.MpCost;
                MapInstance.Broadcast(
                    $"ct 3 {MapMonsterId} {(byte)Target.SessionType()} {Target.GetId()} {npcMonsterSkill.Skill.CastAnimation} {npcMonsterSkill.Skill.CastEffect} {npcMonsterSkill.Skill.SkillVNum}");
            }

            LastMove = DateTime.Now;
            BattleEntity.TargetHit(Target, TargetHitType.SingleTargetHit, npcMonsterSkill?.Skill,
                Monster.BasicSkill);
        }

        public MapCell GetPos() => new MapCell { X = MapX, Y = MapY };

        public object GetSession() => this;

        public AttackType GetAttackType(Skill skill = null) => (AttackType)Monster.AttackClass;

        public bool IsTargetable(SessionType type, bool isPvP = false) =>
            type != WingsEmu.Packets.Enums.SessionType.Monster && IsAlive && CurrentHp > 0;

        public Node[][] GetBrushFire() =>
            BestFirstSearch.LoadBrushFireJagged(new GridPos { X = MapX, Y = MapY }, MapInstance.Map.Grid);

        public SessionType SessionType() => WingsEmu.Packets.Enums.SessionType.Monster;

        public long GetId() => MapMonsterId;

        public void GetDamage(int damage, IBattleEntity entity, bool canKill = true)
        {
            if (CurrentHp <= 0 || !IsAlive)
            {
                return;
            }

            canKill = !IsInvicible && canKill; // Act4 Guardians
            CurrentHp -= damage;
            CurrentHp = CurrentHp <= 0 ? !canKill ? 1 : 0 : CurrentHp;
            BattleEntity.OnHitEvents.ToList().ForEach(e => { EventHelper.Instance.RunEvent(e, monster: this); });
            if (CurrentHp <= 0)
            {
                GenerateDeath(entity);
            }
        }

        public void GenerateDeath(IBattleEntity killer = null)
        {
            if (!IsAlive || IsInvicible) // Act4 Guardians
            {
                CurrentHp = IsInvicible ? 1 : CurrentHp;
                return;
            }

            IsAlive = false;
            CurrentHp = 0;
            CurrentMp = 0;
            Death = DateTime.Now;
            LastMove = DateTime.Now.AddMilliseconds(500);
            BattleEntity.Buffs.Clear();
            Target = null;
            if (MapInstance?.InstanceBag != null)
            {
                MapInstance.InstanceBag.Combo += IsBonus ? 1 : 0;
                MapInstance.InstanceBag.Point +=
                    EventHelper.Instance.CalculateComboPoint(MapInstance.InstanceBag.Combo + (IsBonus ? 1 : 0));
                MapInstance.InstanceBag.MonstersKilled++;
            }

            BattleEntity.OnDeathEvents.ToList().ForEach(e => { EventHelper.Instance.RunEvent(e, monster: this); });
            killer?.GenerateRewards(this);
        }

        public void GenerateRewards(IBattleEntity target)
        {
            DamageList.TryRemove(target, out long value);
            RemoveTarget();
        }

        #endregion
    }
}