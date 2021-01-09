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
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Npc;
using OpenNos.GameObject.Shops;
using OpenNos.GameObject.Skills;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;
using WingsEmu.Packets.ServerPackets;
using WingsEmu.Pathfinder.PathFinder;

namespace OpenNos.GameObject.Maps
{
    public class MapNpc : MapNpcDTO, IBattleEntity
    {
        #region Members

        public NpcMonster Npc;
        private int _movetime;
        private Random _random;
        private const int _maxDistance = 20;

        #endregion

        #region Properties

        #region BattleEntityProperties

        public BattleEntity BattleEntity { get; set; }

        public void AddBuff(Buff.Buff indicator) => BattleEntity.AddBuff(indicator);
        public void RemoveBuff(short cardId, bool removePermaBuff) => BattleEntity.RemoveBuff(cardId, removePermaBuff);

        public void RemoveBuff(short cardId) => RemoveBuff(cardId, false);

        public int[] GetBuff(BCardType.CardType type, byte subtype) => BattleEntity.GetBuff(type, subtype);

        public bool HasBuff(BCardType.CardType type, byte subtype, bool removeWeaponEffects = false) =>
            BattleEntity.HasBuff(type, subtype, removeWeaponEffects);

        public bool HasBuff(BuffType type) => throw new NotImplementedException();

        public ConcurrentBag<Buff.Buff> Buffs => BattleEntity.Buffs;

        #endregion

        public FactionType Faction => FactionType.Neutral;

        public int DealtDamage { get; set; }

        public bool ShouldRespawn { get; set; }

        public bool EffectActivated { get; set; }

        public short FirstX { get; set; }

        public short FirstY { get; set; }

        public bool IsHostile { get; set; }

        public bool IsMate { get; set; }

        public bool IsProtected { get; set; }

        public DateTime Death { get; set; }

        public DateTime LastEffect { get; private set; }

        public DateTime LastMove { get; private set; }

        public DateTime LastSkill { get; set; }

        public IDisposable LifeEvent { get; set; }

        public MapInstance MapInstance { get; set; }

        public List<Node> Path { get; set; }

        public List<Recipe> Recipes { get; set; }

        public Shop Shop { get; set; }

        public IBattleEntity Target { get; set; }

        public List<TeleporterDTO> Teleporters { get; set; }

        public IDisposable Life { get; set; }

        public bool IsOut { get; set; }

        public ConcurrentBag<NpcMonsterSkill> Skills { get; set; } = new ConcurrentBag<NpcMonsterSkill>();

        public int CurrentHp { get; set; }

        public int CurrentMp { get; set; }

        public bool IsAlive { get; set; }

        public int MaxHp => Npc.MaxHP;

        #endregion

        #region Methods

        /// <summary>
        ///     Follow the Monsters target to it's position.
        /// </summary>
        /// <param name="targetSession">The TargetSession to follow</param>
        private void FollowTarget()
        {
            if (Npc == null || !IsAlive || HasBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible))
            {
                return;
            }

            if (!Target?.IsTargetable(SessionType()) ?? true)
            {
                RemoveTarget();
                return;
            }

            if (Path.Count == 0)
            {
                Path = BestFirstSearch.TracePathJagged(new Node { X = MapX, Y = MapY }, Target.GetBrushFire(),
                    MapInstance.Map.Grid);
            }

            Move(); // follow the target
        }

        public EffectPacket GenerateEff(int effectid) => new EffectPacket
        {
            EffectType = 2,
            CharacterId = MapNpcId,
            Id = effectid
        };

        public string GenerateSay(string message, int type) => $"say 2 {MapNpcId} 2 {message}";

        public string GenerateIn()
        {
            NpcMonster npcinfo = ServerManager.Instance.GetNpc(NpcVNum);
            if (npcinfo == null || IsDisabled)
            {
                return string.Empty;
            }

            IsOut = false;
            return
                $"in 2 {NpcVNum} {MapNpcId} {MapX} {MapY} {Position} {(int)(CurrentHp / (float)Npc.MaxHP * 100)} {(int)(CurrentMp / (float)Npc.MaxMP * 100)} {Dialog} 0 0 -1 1 {(IsSitting ? 1 : 0)} -1 - 0 -1 0 0 0 0 0 0 0 0";
        }

        public string GenerateOut()
        {
            NpcMonster npcinfo = ServerManager.Instance.GetNpc(NpcVNum);
            if (npcinfo == null || IsDisabled)
            {
                return string.Empty;
            }

            IsOut = true;
            return $"out 2 {MapNpcId}";
        }

        public string GetNpcDialog() => $"npc_req 2 {MapNpcId} {Dialog}";

        public void Initialize(MapInstance currentMapInstance)
        {
            MapInstance = currentMapInstance;
            Initialize();
        }

        public override void Initialize()
        {
            _random = new Random(MapNpcId);
            Life = null;
            Npc = ServerManager.Instance.GetNpc(NpcVNum);
            LastEffect = LastMove = Death = DateTime.Now;
            BattleEntity = new BattleEntity(this);
            Npc.Skills.ForEach(s => Skills.Add(s));
            IsHostile = Npc.IsHostile;
            FirstX = MapX;
            EffectActivated = true;
            ShouldRespawn = true;
            IsAlive = true;
            CurrentHp = Npc.MaxHP;
            CurrentMp = Npc.MaxMP;
            FirstY = MapY;
            EffectDelay = 4000;
            _movetime = ServerManager.Instance.RandomNumber(500, 3000);
            Path = new List<Node>();
            Recipes = ServerManager.Instance.GetReceipesByMapNpcId(MapNpcId);
            Target = null;
            Teleporters = ServerManager.Instance.GetTeleportersByNpcVNum((short)MapNpcId);
            Shop shop = ServerManager.Instance.GetShopByMapNpcId(MapNpcId);
            if (shop == null)
            {
                return;
            }

            shop.Initialize();
            Shop = shop;
        }

        public void RunDeathEvent()
        {
            MapInstance.InstanceBag.NpcsKilled++;
            BattleEntity.OnDeathEvents.ToList().ForEach(e =>
            {
                if (e.EventActionType == EventActionType.THROWITEMS)
                {
                    Tuple<int, short, byte, int, int> evt = (Tuple<int, short, byte, int, int>)e.Parameter;
                    e.Parameter =
                        new Tuple<int, short, byte, int, int>(MapNpcId, evt.Item2, evt.Item3, evt.Item4, evt.Item5);
                }

                EventHelper.Instance.RunEvent(e);
            });
            BattleEntity.OnDeathEvents.Clear();
        }

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
                            NpcLife();
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

        private string GenerateMv2() => $"mv 2 {MapNpcId} {MapX} {MapY} {Npc.Speed}";

        public void ShowEffect()
        {
            if (!((DateTime.Now - LastEffect).TotalMilliseconds >= EffectDelay))
            {
                return;
            }

            if (IsMate || IsProtected)
            {
                MapInstance.Broadcast(GenerateEff(825), MapX, MapY);
            }

            if (Effect > 0 && EffectActivated)
            {
                MapInstance.Broadcast(GenerateEff(Effect), MapX, MapY);
            }

            LastEffect = DateTime.Now;
        }

        private void NpcLife()
        {
            ShowEffect();
            if (!IsHostile)
            {
                Move();
                return;
            }

            if (!IsAlive) // Respawn
            {
                if (!ShouldRespawn)
                {
                    Life.Dispose();
                }
                else if ((DateTime.Now - Death).TotalSeconds >= Npc.RespawnTime / 10d)
                {
                    Respawn();
                }
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
                if (Life != null && CurrentMp >= (npcMonsterSkill?.Skill.MpCost ?? CurrentMp) &&
                    Map.GetDistance(GetPos(), Target.GetPos()) <= (npcMonsterSkill?.Skill.Range + 1 ?? Npc.BasicRange))
                {
                    TargetHit(npcMonsterSkill);
                    return;
                }

                FollowTarget();
            }
        }


        private void Move()
        {
            if (Npc == null || !IsAlive || !IsMoving || Npc.Speed <= 0 ||
                HasBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible))
            {
                return;
            }

            if (Path.Count == 0 && (DateTime.Now - LastMove).TotalMilliseconds > _movetime && Target == null) // Basic Move
            {
                short mapX = FirstX, mapY = FirstY;
                if (MapInstance.Map?.GetFreePosition(ref mapX, ref mapY,
                    (byte)ServerManager.Instance.RandomNumber(0, 2), (byte)_random.Next(0, 2)) ?? false)
                {
                    int distance = Map.GetDistance(new MapCell { X = mapX, Y = mapY }, GetPos());
                    double value = 1000d * distance / (2 * Npc.Speed);
                    Observable.Timer(TimeSpan.FromMilliseconds(value)).Subscribe(x =>
                    {
                        MapX = mapX;
                        MapY = mapY;
                    });
                    LastMove = DateTime.Now.AddMilliseconds(value);
                    MapInstance.Broadcast(new BroadcastPacket(null, GenerateMv2(), ReceiverType.All));
                }
            }
            else if (DateTime.Now > LastMove && Path.Count > 0) // Follow target || move back to original pos
            {
                byte speedIndex = (byte)(Npc.Speed / 2 < 1 ? 1 : Npc.Speed / 2);
                int maxindex = Path.Count > speedIndex ? speedIndex : Path.Count;
                short smapX = Path[maxindex - 1].X;
                short smapY = Path[maxindex - 1].Y;
                double waitingtime = Map.GetDistance(new MapCell { X = smapX, Y = smapY }, GetPos()) / (double)Npc.Speed;
                MapInstance.Broadcast(new BroadcastPacket(null, $"mv 2 {MapNpcId} {smapX} {smapY} {Npc.Speed}",
                    ReceiverType.All, xCoordinate: smapX, yCoordinate: smapY));
                LastMove = DateTime.Now.AddSeconds(waitingtime > 1 ? 1 : waitingtime);
                Observable.Timer(TimeSpan.FromMilliseconds((int)((waitingtime > 1 ? 1 : waitingtime) * 1000)))
                    .Subscribe(x =>
                    {
                        MapX = smapX;
                        MapY = smapY;
                    });
                if (Target != null && Map.GetDistance(new MapCell { X = FirstX, Y = FirstY }, GetPos()) > _maxDistance
                ) // Remove Target if distance between target & monster is > max Distance
                {
                    RemoveTarget();
                    return;
                }

                Path?.RemoveRange(0, maxindex);
            }

            HostilityTarget();
        }

        internal void HostilityTarget()
        {
            if (Target != null)
            {
                return;
            }

            IBattleEntity target = MapInstance.BattleEntities.FirstOrDefault(e =>
                e.IsTargetable(SessionType()) && Map.GetDistance(GetPos(), e.GetPos()) < Npc.NoticeRange);
            Target = target ?? Target;
        }

        /// <summary>
        ///     Remove the current Target from Npc.
        /// </summary>
        internal void RemoveTarget()
        {
            Target = null;
            Path = BestFirstSearch.FindPathJagged(new Node { X = MapX, Y = MapY }, new Node { X = FirstX, Y = FirstY },
                MapInstance.Map.Grid); // Path To origins
        }

        /// <summary>
        ///     Start the Respawn
        /// </summary>
        private void Respawn()
        {
            if (Npc == null)
            {
                return;
            }

            IsAlive = true;
            Target = null;
            CurrentHp = Npc.MaxHP;
            CurrentMp = Npc.MaxMP;
            MapX = FirstX;
            MapY = FirstY;
            Path = new List<Node>();
            MapInstance.Broadcast(GenerateIn());
            Npc.BCards.ForEach(s => s.ApplyBCards(this));
        }

        public void TargetHit(NpcMonsterSkill npcMonsterSkill)
        {
            if (Npc == null ||
                !((DateTime.Now - LastSkill).TotalMilliseconds >= 1000 + Npc.BasicCooldown * 250) &&
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
                    $"ct 2 {MapNpcId} {(byte)Target.SessionType()} {Target.GetId()} {npcMonsterSkill.Skill.CastAnimation} {npcMonsterSkill.Skill.CastEffect} {npcMonsterSkill.Skill.SkillVNum}");
            }

            LastMove = DateTime.Now;
            BattleEntity.TargetHit(Target, TargetHitType.SingleTargetHit, npcMonsterSkill?.Skill,
                Npc.BasicSkill);
        }

        public MapCell GetPos() => new MapCell { X = MapX, Y = MapY };

        public object GetSession() => this;

        public AttackType GetAttackType(Skill skill = null) => (AttackType)Npc.AttackClass;

        public bool IsTargetable(SessionType type, bool isPvP = false) =>
            type == WingsEmu.Packets.Enums.SessionType.Monster && IsHostile && IsAlive && CurrentHp > 0;

        public Node[][] GetBrushFire() =>
            BestFirstSearch.LoadBrushFireJagged(new GridPos { X = MapX, Y = MapY }, MapInstance.Map.Grid);

        public SessionType SessionType() => WingsEmu.Packets.Enums.SessionType.MateAndNpc;

        public long GetId() => MapNpcId;

        public void GenerateDeath(IBattleEntity killer)
        {
            if (CurrentHp > 0)
            {
                return;
            }

            IsAlive = false;
            CurrentHp = 0;
            CurrentMp = 0;
            Death = DateTime.Now;
            LastMove = DateTime.Now.AddMilliseconds(500);
            BattleEntity.Buffs.Clear();
            Target = null;
        }

        public void GenerateRewards(IBattleEntity target)
        {
            RemoveTarget();
        }

        public void GetDamage(int damage, IBattleEntity entity, bool canKill = true)
        {
            if (CurrentHp <= 0)
            {
                return;
            }

            CurrentHp -= damage;
            CurrentHp = CurrentHp <= 0 ? !canKill ? 1 : 0 : CurrentHp;
            if (CurrentHp <= 0)
            {
                GenerateDeath(entity);
                entity.GenerateRewards(this);
            }
        }

        #endregion
    }
}