﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Npc;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Buff
{
    public class BCard : BCardDTO
    {
        #region Methods

        public void ApplyBCards(IBattleEntity session, IBattleEntity caster = null, short? partnerBuffLevel = null)
        {
            Mate mate = session is Mate ? (Mate)session.GetSession() : null;
            Character.Character character = session is Character.Character ? (Character.Character)session.GetSession() : null;
            switch ((BCardType.CardType)Type)
            {
                case BCardType.CardType.Buff:
                    /*
                     * // ANTI-DEBUFFS
                       ReducedMinorBleeding = 55,
                       ReducedSeriousBleeding = 56,
                       ReducedAllBleeding = 57,
                       ReducedSmallBlackout = 58,
                       ReducedAllBlackout = 59,
                       ReducedHandOfDeath = 60,
                       ReducedFrozenChance = 61,
                       ReducedBlindChance = 62,
                       ReducedArrestationChance = 63,
                       ReducedDefenseReduction = 64,
                       ReducedShockChance = 65,
                       ReducedRigidityChance = 66,
                       SReducedAllNegative = 67,
                     */
                    int antiDebuffBonus = 0;
                    var b = new Buff(SecondData);
                    foreach (EquipmentOptionDTO eqopt in session.BattleEntity.ShellOptionArmor)
                    {
                        switch ((ShellOptionType)eqopt.Type)
                        {
                            case ShellOptionType.SReducedAllNegative:
                                antiDebuffBonus += eqopt.Value;
                                break;
                        }
                    }

                    if (ServerManager.Instance.RandomNumber() < antiDebuffBonus && b?.Card?.BuffType == BuffType.Bad)
                    {
                        break;
                    }

                    if (ServerManager.Instance.RandomNumber() < FirstData)
                    {
                        session?.BattleEntity.AddBuff(new Buff(SecondData + (partnerBuffLevel ?? 0),
                            caster?.BattleEntity.Level ?? session.BattleEntity.Level, entity: caster));
                    }

                    break;

                case BCardType.CardType.Move:
                    if (character == null)
                    {
                        break;
                    }

                    character.LastSpeedChange = DateTime.Now;
                    character.LoadSpeed();
                    character.Session.SendPacket(character.GenerateCond());
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.Move.MoveSpeedDecreased:
                            Card speedDebuff = ServerManager.Instance.GetCardByCardId(CardId);
                            if (speedDebuff == null)
                            {
                                return;
                            }

                            character.Speed /= (byte)(100 / FirstData);
                            Observable.Timer(TimeSpan.FromSeconds(speedDebuff.Duration * 0.1)).Subscribe(s => { character.LoadSpeed(); });
                            break;
                    }

                    break;

                case BCardType.CardType.Summons:
                    NpcMonster npcMonster = session.GetSession() is MapMonster mob ? mob.Monster :
                        session.GetSession() is MapNpc npc ? npc.Npc : null;
                    ConcurrentBag<ToSummon> summonParameters = new ConcurrentBag<ToSummon>();

                    switch ((AdditionalTypes.Summons)SubType)
                    {
                        case AdditionalTypes.Summons.Summons:
                            for (int i = 0; i < FirstData; i++)
                            {
                                MapCell cell = session.GetPos();
                                cell.Y += (short)ServerManager.Instance.RandomNumber(-3, 3);
                                cell.X += (short)ServerManager.Instance.RandomNumber(-3, 3);
                                summonParameters.Add(new ToSummon((short)SecondData, cell, null, true,
                                    (byte)Math.Abs(ThirdData)));
                            }

                            EventHelper.Instance.RunEvent(new EventContainer(session.MapInstance,
                                EventActionType.SPAWNMONSTERS, summonParameters));
                            break;

                        case AdditionalTypes.Summons.SummonTrainingDummy:
                            if (npcMonster != null && session.BattleEntity.OnHitEvents.All(s =>
                                s?.EventActionType != EventActionType.SPAWNMONSTERS))
                            {
                                summonParameters.Add(new ToSummon((short)SecondData, session.GetPos(), null, true,
                                    (byte)Math.Abs(ThirdData)));
                                session.BattleEntity.OnHitEvents.Add(new EventContainer(session.MapInstance,
                                    EventActionType.SPAWNMONSTERS, summonParameters));
                            }

                            break;

                        case AdditionalTypes.Summons.SummonUponDeathChance:
                        case AdditionalTypes.Summons.SummonUponDeath:
                            if (npcMonster != null &&
                                session.BattleEntity.OnDeathEvents.All(s =>
                                    s?.EventActionType != EventActionType.SPAWNMONSTERS))
                            {
                                for (int i = 0; i < FirstData; i++)
                                {
                                    MapCell cell = session.GetPos();
                                    cell.Y += (short)i;
                                    summonParameters.Add(new ToSummon((short)SecondData, cell, null, true,
                                        (byte)Math.Abs(ThirdData)));
                                }

                                session.BattleEntity.OnDeathEvents.Add(new EventContainer(session.MapInstance,
                                    EventActionType.SPAWNMONSTERS, summonParameters));
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.SpecialAttack:
                    break;

                case BCardType.CardType.SpecialDefence:
                    break;

                case BCardType.CardType.AttackPower:
                    break;

                case BCardType.CardType.Target:
                    break;

                case BCardType.CardType.Critical:
                    break;

                case BCardType.CardType.SpecialCritical:
                    break;

                case BCardType.CardType.Element:
                    break;

                case BCardType.CardType.IncreaseDamage:
                    break;

                case BCardType.CardType.Defence:
                    break;

                case BCardType.CardType.DodgeAndDefencePercent:
                    break;

                case BCardType.CardType.Block:
                    break;

                case BCardType.CardType.Absorption:
                    break;

                case BCardType.CardType.ElementResistance:
                    break;

                case BCardType.CardType.EnemyElementResistance:
                    break;

                case BCardType.CardType.Damage:
                    break;

                case BCardType.CardType.GuarantedDodgeRangedAttack:
                    break;

                case BCardType.CardType.Morale:
                    break;

                case BCardType.CardType.Casting:
                    break;

                case BCardType.CardType.Reflection:
                    break;

                case BCardType.CardType.DrainAndSteal:
                    if (ServerManager.Instance.RandomNumber() < FirstData)
                    {
                        return;
                    }

                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DrainAndSteal.LeechEnemyHP:
                            int heal = 0;
                            switch (session)
                            {
                                case MapMonster toDrain when caster is Character.Character drainer:
                                    heal = drainer.Level * SecondData;
                                    drainer.Hp = (int)(heal + drainer.Hp > drainer.HpLoad() ? drainer.HpLoad() : drainer.Hp + heal);
                                    drainer.MapInstance?.Broadcast(drainer.GenerateRc((int)(heal + drainer.Hp > drainer.HpLoad() ? drainer.HpLoad() - drainer.Hp : heal)));
                                    toDrain.CurrentHp -= heal;
                                    drainer.Session.SendPacket(drainer.GenerateStat());
                                    if (toDrain.CurrentHp <= 0)
                                    {
                                        toDrain.CurrentHp = 1;
                                    }

                                    break;
                                case Character.Character characterDrained when caster is Character.Character drainerCharacter:
                                    heal = drainerCharacter.Level * SecondData;
                                    drainerCharacter.Hp = (int)(heal + drainerCharacter.Hp > drainerCharacter.HpLoad() ? drainerCharacter.HpLoad() : drainerCharacter.Hp + heal);
                                    drainerCharacter.MapInstance?.Broadcast(
                                        drainerCharacter.GenerateRc((int)(heal + drainerCharacter.Hp > drainerCharacter.HpLoad() ? drainerCharacter.HpLoad() - drainerCharacter.Hp : heal)));
                                    characterDrained.Hp -= heal;
                                    characterDrained.Session.SendPacket(characterDrained.GenerateStat());
                                    drainerCharacter.Session.SendPacket(drainerCharacter.GenerateStat());
                                    if (characterDrained.Hp <= 0)
                                    {
                                        characterDrained.Hp = 1;
                                    }

                                    break;
                                case Character.Character characterDrained when caster is MapMonster drainerMapMonster:
                                    heal = drainerMapMonster.Monster.Level * SecondData;
                                    drainerMapMonster.CurrentHp = heal + drainerMapMonster.CurrentHp > drainerMapMonster.MaxHp ? drainerMapMonster.MaxHp : drainerMapMonster.CurrentHp + heal;
                                    drainerMapMonster.MapInstance?.Broadcast(drainerMapMonster.GenerateRc(heal + drainerMapMonster.CurrentHp > drainerMapMonster.MaxHp
                                        ? drainerMapMonster.MaxHp - drainerMapMonster.CurrentHp
                                        : heal));
                                    characterDrained.Hp -= heal;
                                    characterDrained.Session.SendPacket(characterDrained.GenerateStat());
                                    if (characterDrained.Hp <= 0)
                                    {
                                        characterDrained.Hp = 1;
                                    }

                                    break;
                            }

                            break;
                        case (byte)AdditionalTypes.DrainAndSteal.LeechEnemyMP:
                            int mpDrain = 0;
                            switch (session)
                            {
                                case MapMonster toDrain when caster is Character.Character drainer:
                                    mpDrain = drainer.Level * SecondData;
                                    drainer.Mp = (int)(mpDrain + drainer.Mp > drainer.MpLoad() ? drainer.MpLoad() : drainer.Mp + mpDrain);
                                    toDrain.CurrentMp -= mpDrain;
                                    drainer.Session.SendPacket(drainer.GenerateStat());
                                    if (toDrain.CurrentMp <= 0)
                                    {
                                        toDrain.CurrentMp = 1;
                                    }

                                    break;
                                case Character.Character characterDrained when caster is Character.Character drainerCharacter:
                                    mpDrain = drainerCharacter.Level * SecondData;
                                    drainerCharacter.Mp = (int)(mpDrain + drainerCharacter.Mp > drainerCharacter.MpLoad() ? drainerCharacter.MpLoad() : drainerCharacter.Mp + mpDrain);
                                    characterDrained.Mp -= mpDrain;
                                    characterDrained.Session.SendPacket(characterDrained.GenerateStat());
                                    drainerCharacter.Session.SendPacket(drainerCharacter.GenerateStat());
                                    if (characterDrained.Mp <= 0)
                                    {
                                        characterDrained.Mp = 1;
                                    }

                                    break;
                                case Character.Character characterDrained when caster is MapMonster drainerMapMonster:
                                    // TODO: Add a MaxMp property to MapMonsters
                                    /*
                                    mpDrain = drainerMapMonster.Monster.Level * SecondData;
                                    drainerMapMonster.CurrentMp = (mpDrain + drainerMapMonster.CurrentMp > drainerMapMonster.MaxHp ? drainerMapMonster.MaxHp : drainerMapMonster.CurrentHp + mpDrain);
                                    drainerMapMonster.MapInstance?.Broadcast(drainerMapMonster.GenerateRc((mpDrain + drainerMapMonster.CurrentHp > drainerMapMonster.MaxHp ? drainerMapMonster.MaxHp - drainerMapMonster.CurrentHp : mpDrain)));
                                    characterDrained.Hp -= mpDrain;
                                    characterDrained.MapInstance?.Broadcast(characterDrained.GenerateStat());
                                    if (characterDrained.Hp <= 0)
                                    {
                                        characterDrained.Hp = 1;
                                    }*/
                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.HealingBurningAndCasting:
                    var subtype = (AdditionalTypes.HealingBurningAndCasting)SubType;
                    switch (subtype)
                    {
                        case AdditionalTypes.HealingBurningAndCasting.RestoreHP:
                        case AdditionalTypes.HealingBurningAndCasting.RestoreHPWhenCasting:
                            Card hpCard = ServerManager.Instance.GetCardByCardId(CardId);
                            IDisposable observable = null;
                            IDisposable mateObservable = null;
                            if (session is Character.Character receiver)
                            {
                                if (hpCard == null)
                                {
                                    int heal = FirstData;
                                    bool change = false;
                                    if (IsLevelScaled)
                                    {
                                        if (IsLevelDivided)
                                        {
                                            heal /= receiver.Level;
                                        }
                                        else
                                        {
                                            heal *= receiver.Level;
                                        }
                                    }

                                    if (receiver.Hp + heal < receiver.HpLoad())
                                    {
                                        receiver.Hp += heal;
                                        receiver.Session?.CurrentMapInstance?.Broadcast(receiver.GenerateRc(heal));
                                        change = true;
                                    }
                                    else
                                    {
                                        if (receiver.Hp != (int)receiver.HpLoad())
                                        {
                                            receiver.Session?.CurrentMapInstance?.Broadcast(
                                                receiver.GenerateRc((int)(receiver.HpLoad() - receiver.Hp)));
                                            change = true;
                                        }

                                        receiver.Hp = (int)receiver.HpLoad();
                                    }

                                    if (change)
                                    {
                                        receiver.Session?.SendPacket(receiver.GenerateStat());
                                    }

                                    break;
                                }

                                observable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                {
                                    if (receiver.Hp > 0)
                                    {
                                        int heal = FirstData;
                                        bool change = false;
                                        if (IsLevelScaled)
                                        {
                                            if (IsLevelDivided)
                                            {
                                                heal /= receiver.Level;
                                            }
                                            else
                                            {
                                                heal *= receiver.Level;
                                            }
                                        }

                                        if (receiver.Hp + heal < receiver.HpLoad())
                                        {
                                            receiver.Hp += heal;
                                            receiver.Session?.CurrentMapInstance?.Broadcast(receiver.GenerateRc(heal));
                                            change = true;
                                        }
                                        else
                                        {
                                            if (receiver.Hp != (int)receiver.HpLoad())
                                            {
                                                receiver.Session?.CurrentMapInstance?.Broadcast(
                                                    receiver.GenerateRc((int)(receiver.HpLoad() - receiver.Hp)));
                                                change = true;
                                            }

                                            receiver.Hp = (int)receiver.HpLoad();
                                        }

                                        if (change)
                                        {
                                            receiver.Session?.SendPacket(receiver.GenerateStat());
                                        }
                                    }
                                    else
                                    {
                                        observable?.Dispose();
                                    }
                                });

                                Observable.Timer(TimeSpan.FromSeconds(hpCard.Duration * 0.1)).Subscribe(s => { observable?.Dispose(); });
                            }

                            if (mate != null)
                            {
                                mateObservable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                {
                                    int heal = FirstData;
                                    if (IsLevelScaled)
                                    {
                                        if (IsLevelDivided)
                                        {
                                            heal /= mate.Level;
                                        }
                                        else
                                        {
                                            heal *= mate.Level;
                                        }
                                    }

                                    if (mate.Hp + heal < mate.HpLoad())
                                    {
                                        mate.Hp += heal;
                                    }
                                    else
                                    {
                                        mate.Hp = mate.HpLoad();
                                    }
                                });

                                if (hpCard == null)
                                {
                                    mateObservable?.Dispose();
                                    break;
                                }

                                Observable.Timer(TimeSpan.FromSeconds(hpCard.Duration * 0.1)).Subscribe(s => { mateObservable?.Dispose(); });
                            }

                            break;
                        case AdditionalTypes.HealingBurningAndCasting.RestoreMP:
                            Card restoreMpCard = ServerManager.Instance.GetCardByCardId(CardId);
                            IDisposable restoreCharMp = null;
                            IDisposable restoreMateMp;
                            if (session is Character.Character healReceiver)
                            {
                                if (restoreMpCard == null)
                                {
                                    int heal = FirstData;
                                    bool change = false;
                                    if (IsLevelScaled)
                                    {
                                        if (IsLevelDivided)
                                        {
                                            heal /= healReceiver.Level;
                                        }
                                        else
                                        {
                                            heal *= healReceiver.Level;
                                        }
                                    }

                                    if (healReceiver.Mp + heal < healReceiver.MpLoad())
                                    {
                                        healReceiver.Mp += heal;
                                        change = true;
                                    }
                                    else
                                    {
                                        if (healReceiver.Mp != (int)healReceiver.MpLoad())
                                        {
                                            change = true;
                                        }

                                        healReceiver.Mp = (int)healReceiver.MpLoad();
                                    }

                                    if (change)
                                    {
                                        healReceiver.Session?.SendPacket(healReceiver.GenerateStat());
                                    }

                                    break;
                                }

                                restoreCharMp = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                {
                                    if (healReceiver.Hp > 0)
                                    {
                                        int heal = FirstData;
                                        bool change = false;
                                        if (IsLevelScaled)
                                        {
                                            if (IsLevelDivided)
                                            {
                                                heal /= healReceiver.Level;
                                            }
                                            else
                                            {
                                                heal *= healReceiver.Level;
                                            }
                                        }

                                        if (healReceiver.Mp + heal < healReceiver.MpLoad())
                                        {
                                            healReceiver.Mp += heal;
                                            change = true;
                                        }
                                        else
                                        {
                                            if (healReceiver.Mp != (int)healReceiver.MpLoad())
                                            {
                                                change = true;
                                            }

                                            healReceiver.Mp = (int)healReceiver.MpLoad();
                                        }

                                        if (change)
                                        {
                                            healReceiver.Session?.SendPacket(healReceiver.GenerateStat());
                                        }
                                    }
                                    else
                                    {
                                        restoreCharMp?.Dispose();
                                    }
                                });

                                Observable.Timer(TimeSpan.FromSeconds(restoreMpCard.Duration * 0.1)).Subscribe(s => { restoreCharMp?.Dispose(); });
                            }

                            if (mate != null)
                            {
                                restoreMateMp = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1 <= 0 ? 2 : ThirdData + 1)).Subscribe(x =>
                                {
                                    int heal = FirstData;
                                    if (IsLevelScaled)
                                    {
                                        if (IsLevelDivided)
                                        {
                                            heal /= mate.Level;
                                        }
                                        else
                                        {
                                            heal *= mate.Level;
                                        }
                                    }

                                    if (mate.Mp + heal < mate.MpLoad())
                                    {
                                        mate.Mp += heal;
                                    }
                                    else
                                    {
                                        mate.Mp = mate.MpLoad();
                                    }
                                });

                                if (restoreMpCard == null)
                                {
                                    restoreMateMp?.Dispose();
                                    break;
                                }

                                Observable.Timer(TimeSpan.FromSeconds(restoreMpCard.Duration * 0.1)).Subscribe(s => { restoreMateMp?.Dispose(); });
                            }

                            break;
                        case AdditionalTypes.HealingBurningAndCasting.DecreaseHP:
                            int timer = ThirdData + 1;
                            IDisposable obs = null;
                            Card card = ServerManager.Instance.GetCardByCardId(CardId);
                            if (card == null)
                            {
                                Logger.Log.Warn("CardId was null, can't apply bcard.");
                                return;
                            }

                            if (IsLevelScaled)
                            {
                                int scale = FirstData + 1;
                                ushort damage = 0;
                                obs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                {
                                    switch (session)
                                    {
                                        case Character.Character receiverCharacter when caster is Character.Character senderCharacter:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = (ushort)(senderCharacter.Level * scale);
                                                receiverCharacter.Hp = receiverCharacter.Hp - damage <= 0 ? 1 : receiverCharacter.Hp - damage;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm(damage));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }

                                            break;
                                        case MapMonster receiverMonster when caster is Character.Character senderCharacter:
                                            if (receiverMonster.CurrentHp > 0)
                                            {
                                                damage = (ushort)(senderCharacter.Level * scale);
                                                receiverMonster.CurrentHp = receiverMonster.CurrentHp - damage <= 0 ? 1 : receiverMonster.CurrentHp - damage;
                                                receiverMonster.MapInstance?.Broadcast(receiverMonster.GenerateDm(damage));
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }

                                            break;
                                        case Character.Character receiverCharacter when caster is MapMonster senderMapMonster:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = (ushort)(senderMapMonster.Monster.Level * scale);
                                                receiverCharacter.Hp = receiverCharacter.Hp - damage <= 0 ? 1 : receiverCharacter.Hp - damage;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm(damage));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }

                                            break;
                                        case Character.Character receiverCharacter when caster is Mate senderMate:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = (ushort)(senderMate.Level * scale);
                                                receiverCharacter.Hp = receiverCharacter.Hp - damage <= 0 ? 1 : receiverCharacter.Hp - damage;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm(damage));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }

                                            break;
                                        case Mate receiverMate when caster is Character.Character senderCharacter:
                                            damage = (ushort)(senderCharacter.Level * scale);
                                            receiverMate.Hp = receiverMate.Hp - damage <= 0 ? 1 : receiverMate.Hp - damage;
                                            receiverMate.MapInstance?.Broadcast(receiverMate.GenerateDm(damage));
                                            receiverMate.Owner?.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                        case Mate receiverMate when caster is MapMonster senderMapMonster:
                                            damage = (ushort)(senderMapMonster.Monster.Level * scale);
                                            receiverMate.Hp = receiverMate.Hp - damage <= 0 ? 1 : receiverMate.Hp - damage;
                                            receiverMate.MapInstance?.Broadcast(receiverMate.GenerateDm(damage));
                                            receiverMate.Owner?.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                        case Character.Character receiverCharacter:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = (ushort)(receiverCharacter.Level * scale);
                                                receiverCharacter.Hp = receiverCharacter.Hp - damage <= 0 ? 1 : receiverCharacter.Hp - damage;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm(damage));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }

                                            break;
                                    }
                                });
                                Observable.Timer(TimeSpan.FromSeconds(card.Duration * 0.1)).Subscribe(s => { obs.Dispose(); });
                            }
                            else
                            {
                                ushort damage = (ushort)FirstData;
                                obs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                {
                                    switch (session)
                                    {
                                        case Character.Character receiverCharacter when caster is Character.Character senderCharacter:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = senderCharacter.Level;
                                                receiverCharacter.Hp = receiverCharacter.Hp - damage <= 0 ? 1 : receiverCharacter.Hp - damage;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm(damage));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }

                                            break;
                                        case MapMonster receiverMonster when caster is Character.Character senderCharacter:
                                            damage = senderCharacter.Level;
                                            receiverMonster.CurrentHp = receiverMonster.CurrentHp - damage <= 0 ? 1 : receiverMonster.CurrentHp - damage;
                                            receiverMonster.MapInstance?.Broadcast(receiverMonster.GenerateDm(damage));
                                            break;
                                        case Character.Character receiverCharacter when caster is MapMonster senderMapMonster:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = senderMapMonster.Monster.Level;
                                                receiverCharacter.Hp = receiverCharacter.Hp - damage <= 0 ? 1 : receiverCharacter.Hp - damage;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm(damage));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }

                                            break;
                                        case Mate receiverMate when caster is Character.Character senderCharacter:
                                            damage = senderCharacter.Level;
                                            receiverMate.Hp = receiverMate.Hp - damage <= 0 ? 1 : receiverMate.Hp - damage;
                                            receiverMate.MapInstance?.Broadcast(receiverMate.GenerateDm(damage));
                                            receiverMate.Owner?.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                        case Mate receiverMate when caster is MapMonster senderMapMonster:
                                            damage = senderMapMonster.Monster.Level;
                                            receiverMate.Hp = receiverMate.Hp - damage <= 0 ? 1 : receiverMate.Hp - damage;
                                            receiverMate.MapInstance?.Broadcast(receiverMate.GenerateDm(damage));
                                            receiverMate.Owner.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                    }
                                });
                                Observable.Timer(TimeSpan.FromSeconds(card.Duration * 0.1)).Subscribe(s => obs.Dispose());
                            }

                            break;
                        case AdditionalTypes.HealingBurningAndCasting.DecreaseMP:
                            int mpTimer = ThirdData + 1;
                            IDisposable mpObs = null;
                            Card mpCard = ServerManager.Instance.GetCardByCardId(CardId ?? -1);
                            if (mpCard == null)
                            {
                                Logger.Log.Warn("CardId was null, can't apply bcard.");
                                return;
                            }

                            if (IsLevelScaled)
                            {
                                int scale = FirstData + 1;
                                ushort damage = 0;
                                mpObs = Observable.Interval(TimeSpan.FromSeconds(mpTimer)).Subscribe(s =>
                                {
                                    switch (session)
                                    {
                                        case Character.Character receiverCharacter when caster is Character.Character senderCharacter:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = (ushort)(senderCharacter.Level * scale);
                                                receiverCharacter.Mp = receiverCharacter.Mp - damage <= 0 ? 1 : receiverCharacter.Mp - damage;
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                mpObs?.Dispose();
                                            }

                                            break;
                                        case MapMonster receiverMonster when caster is Character.Character senderCharacter:
                                            damage = (ushort)(senderCharacter.Level * scale);
                                            receiverMonster.CurrentMp = receiverMonster.CurrentMp - damage <= 0 ? 1 : receiverMonster.CurrentMp - damage;
                                            break;
                                        case Character.Character receiverCharacter when caster is MapMonster senderMapMonster:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = (ushort)(senderMapMonster.Monster.Level * scale);
                                                receiverCharacter.Mp = receiverCharacter.Mp - damage <= 0 ? 1 : receiverCharacter.Mp - damage;
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                mpObs?.Dispose();
                                            }

                                            break;
                                        case Mate receiverMate when caster is Character.Character senderCharacter:
                                            damage = (ushort)(senderCharacter.Level * scale);
                                            receiverMate.Mp = receiverMate.Mp - damage <= 0 ? 1 : receiverMate.Mp - damage;
                                            receiverMate.Owner?.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                        case Mate receiverMate when caster is MapMonster senderMapMonster:
                                            damage = (ushort)(senderMapMonster.Monster.Level * scale);
                                            receiverMate.Mp = receiverMate.Mp - damage <= 0 ? 1 : receiverMate.Mp - damage;
                                            receiverMate.Owner?.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                        case Character.Character receiverCharacter when caster is Mate senderMate:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = (ushort)(senderMate.Level * scale);
                                                receiverCharacter.Mp = receiverCharacter.Mp - damage <= 0 ? 1 : receiverCharacter.Mp - damage;
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                mpObs?.Dispose();
                                            }

                                            break;
                                    }
                                });
                                Observable.Timer(TimeSpan.FromSeconds(mpCard.Duration * 0.1)).Subscribe(s => mpObs.Dispose());
                            }
                            else
                            {
                                ushort damage = (ushort)FirstData;
                                obs = Observable.Interval(TimeSpan.FromSeconds(mpTimer)).Subscribe(s =>
                                {
                                    switch (session)
                                    {
                                        case Character.Character receiverCharacter when caster is Character.Character senderCharacter:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = senderCharacter.Level;
                                                receiverCharacter.Mp = receiverCharacter.Mp - damage <= 0 ? 1 : receiverCharacter.Mp - damage;
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                mpObs?.Dispose();
                                            }

                                            break;
                                        case MapMonster receiverMonster when caster is Character.Character senderCharacter:
                                            damage = senderCharacter.Level;
                                            receiverMonster.CurrentMp = receiverMonster.CurrentMp - damage <= 0 ? 1 : receiverMonster.CurrentMp - damage;
                                            break;
                                        case Character.Character receiverCharacter when caster is MapMonster senderMapMonster:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = senderMapMonster.Monster.Level;
                                                receiverCharacter.Mp = receiverCharacter.Mp - damage <= 0 ? 1 : receiverCharacter.Mp - damage;
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                mpObs?.Dispose();
                                            }

                                            break;
                                        case Mate receiverMate when caster is Character.Character senderCharacter:
                                            damage = senderCharacter.Level;
                                            receiverMate.Mp = receiverMate.Mp - damage <= 0 ? 1 : receiverMate.Mp - damage;
                                            receiverMate.Owner?.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                        case Mate receiverMate when caster is MapMonster senderMapMonster:
                                            damage = senderMapMonster.Monster.Level;
                                            receiverMate.Mp = receiverMate.Mp - damage <= 0 ? 1 : receiverMate.Mp - damage;
                                            receiverMate.Owner?.Session.SendPacket(receiverMate.GenerateStatInfo());
                                            break;
                                        case Character.Character receiverCharacter when caster is Mate senderMate:
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                damage = senderMate.Level;
                                                receiverCharacter.Mp = receiverCharacter.Mp - damage <= 0 ? 1 : receiverCharacter.Mp - damage;
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                mpObs?.Dispose();
                                            }

                                            break;
                                    }
                                });
                                Observable.Timer(TimeSpan.FromSeconds(mpCard.Duration * 0.1)).Subscribe(s => obs.Dispose());
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.HPMP:
                    break;

                case BCardType.CardType.SpecialisationBuffResistance:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.SpecialisationBuffResistance.RemoveBadEffects:
                            List<BuffType> buffsToDisable = new List<BuffType> { BuffType.Bad };
                            switch (session)
                            {
                                case Character.Character isCharacter:
                                {
                                    if (FirstData <= ServerManager.Instance.RandomNumber())
                                    {
                                        break;
                                    }

                                    isCharacter.DisableBuffs(buffsToDisable, FirstData);
                                }
                                    break;
                                case Mate isMate:
                                {
                                    if (FirstData <= ServerManager.Instance.RandomNumber())
                                    {
                                        break;
                                    }

                                    isMate.BattleEntity.DisableBuffs(buffsToDisable, FirstData);
                                }
                                    break;
                            }

                            break;
                        case (byte)AdditionalTypes.SpecialisationBuffResistance.RemoveGoodEffects:
                            List<BuffType> buffsToDisable2 = new List<BuffType> { BuffType.Good };
                            switch (session)
                            {
                                case Character.Character isCharacter:
                                {
                                    if (FirstData <= ServerManager.Instance.RandomNumber())
                                    {
                                        break;
                                    }

                                    isCharacter.DisableBuffs(buffsToDisable2, FirstData);
                                }
                                    break;
                                case Mate isMate:
                                {
                                    if (FirstData <= ServerManager.Instance.RandomNumber())
                                    {
                                        break;
                                    }

                                    isMate.BattleEntity.DisableBuffs(buffsToDisable2, FirstData);
                                }
                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.SpecialEffects:
                    Card speedCard = ServerManager.Instance.GetCardByCardId(CardId);
                    if (speedCard == null)
                    {
                        break;
                    }

                    if (session is Character.Character fun)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.SpecialEffects.ShadowAppears:
                                fun.Session.CurrentMapInstance?.Broadcast($"guri 0 1 {fun.CharacterId} {FirstData} {SecondData}");
                                Observable.Timer(TimeSpan.FromSeconds(speedCard.Duration * 0.1)).Subscribe(s =>
                                {
                                    fun.Session.CurrentMapInstance?.Broadcast($"guri 0 1 {fun.CharacterId} 0 {SecondData}");
                                });
                                break;
                        }
                    }

                    break;

                case BCardType.CardType.Capture:
                    if (session is MapMonster monsterToCapture && caster is Character.Character hunter)
                    {
                        if (monsterToCapture.Monster.RaceType == 1 &&
                            (hunter.MapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance ||
                                hunter.MapInstance?.MapInstanceType == MapInstanceType.TimeSpaceInstance))
                        {
                            if (monsterToCapture.Monster.Level < hunter.Level)
                            {
                                if (monsterToCapture.CurrentHp < monsterToCapture.Monster.MaxHP / 2)
                                {
                                    if (hunter.MaxMateCount > hunter.Mates.Count())
                                    {
                                        // Algo  
                                        int capturerate =
                                            100 - (monsterToCapture.CurrentHp / monsterToCapture.Monster.MaxHP + 1) / 2;
                                        if (ServerManager.Instance.RandomNumber() <= capturerate)
                                        {
                                            if (hunter.Quests.Any(q =>
                                                q.Quest.QuestType == (int)QuestType.Capture1 &&
                                                q.Quest.QuestObjectives.Any(d =>
                                                    d.Data == monsterToCapture.MonsterVNum)))
                                            {
                                                hunter.IncrementQuests(QuestType.Capture1,
                                                    monsterToCapture.MonsterVNum);
                                                return;
                                            }

                                            hunter.IncrementQuests(QuestType.Capture2, monsterToCapture.MonsterVNum);
                                            int level = monsterToCapture.Monster.Level - 15 < 1
                                                ? 1
                                                : monsterToCapture.Monster.Level - 15;
                                            Mate currentmate = hunter.Mates?.FirstOrDefault(m =>
                                                m.IsTeamMember && m.MateType == MateType.Pet);
                                            if (currentmate != null)
                                            {
                                                currentmate.RemoveTeamMember(); // remove current pet
                                                hunter.MapInstance?.Broadcast(currentmate.GenerateOut());
                                            }

                                            monsterToCapture.MapInstance?.DespawnMonster(monsterToCapture);
                                            NpcMonster mateNpc =
                                                ServerManager.Instance.GetNpc(monsterToCapture.MonsterVNum);
                                            mate = new Mate(hunter, mateNpc, (byte)level, MateType.Pet);
                                            hunter.Mates?.Add(mate);
                                            mate.RefreshStats();
                                            hunter.Session.SendPacket($"ctl 2 {mate.PetId} 3");
                                            hunter.MapInstance?.Broadcast(mate.GenerateIn());
                                            hunter.Session.SendPacket(hunter.GenerateSay(
                                                string.Format(Language.Instance.GetMessageFromKey("YOU_GET_PET"),
                                                    mate.Name), 0));
                                            hunter.Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
                                            hunter.Session.SendPackets(hunter.GenerateScP());
                                            hunter.Session.SendPackets(hunter.GenerateScN());
                                            hunter.Session.SendPacket(hunter.GeneratePinit());
                                            hunter.Session.SendPackets(hunter.Mates.Where(s => s.IsTeamMember)
                                                .OrderBy(s => s.MateType)
                                                .Select(s => s.GeneratePst()));
                                        }
                                        else
                                        {
                                            hunter.Session.SendPacket(
                                                UserInterfaceHelper.Instance.GenerateMsg(
                                                    Language.Instance.GetMessageFromKey("CAPTURE_FAILED"), 0));
                                        }
                                    }
                                    else
                                    {
                                        hunter.Session.SendPacket(
                                            UserInterfaceHelper.Instance.GenerateMsg(
                                                Language.Instance.GetMessageFromKey("MAX_MATES_COUNT"), 0));
                                    }
                                }
                                else
                                {
                                    hunter.Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                        Language.Instance.GetMessageFromKey("monsterToCapture_MUST_BE_LOW_HP"), 0));
                                }
                            }
                            else
                            {
                                hunter.Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("monsterToCapture_LVL_MUST_BE_LESS"), 0));
                            }
                        }
                        else
                        {
                            hunter.Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                                Language.Instance.GetMessageFromKey("monsterToCapture_CANNOT_BE_CAPTURED"), 0));
                        }
                    }

                    break;

                case BCardType.CardType.SpecialDamageAndExplosions:
                    break;

                case BCardType.CardType.SpecialEffects2:
                    if (session is Character.Character tp)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.SpecialEffects2.TeleportInRadius:
                                tp.TeleportInRadius(FirstData);
                                break;
                        }
                    }

                    if (caster is Character.Character teleportedUser)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.SpecialEffects2.TeleportInRadius:
                                teleportedUser.TeleportInRadius(FirstData);
                                break;
                        }
                    }

                    break;

                case BCardType.CardType.CalculatingLevel:
                    break;

                case BCardType.CardType.Recovery:
                    break;

                case BCardType.CardType.MaxHPMP:
                    break;

                case BCardType.CardType.MultAttack:
                    break;

                case BCardType.CardType.MultDefence:
                    break;

                case BCardType.CardType.TimeCircleSkills:
                    break;

                case BCardType.CardType.RecoveryAndDamagePercent:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.RecoveryAndDamagePercent.HPRecovered:
                            IDisposable obs = null;
                            switch (session)
                            {
                                case Character.Character receiverCharacter:
                                    if (IsLevelScaled)
                                    {
                                        Card hcard = ServerManager.Instance.GetCardByCardId(CardId);
                                        if (hcard == null)
                                        {
                                            break;
                                        }

                                        int bonus = receiverCharacter.Level / FirstData;
                                        int heal = (int)(receiverCharacter.HpLoad() * (bonus * 0.01));

                                        obs = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1 < 0 ? 2 : ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                receiverCharacter.Hp = (int)(receiverCharacter.Hp + heal > receiverCharacter.HpLoad() ? receiverCharacter.HpLoad() : receiverCharacter.Hp + heal);
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateRc(heal));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                            else
                                            {
                                                obs?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(hcard.Duration * 0.1)).Subscribe(s => { obs?.Dispose(); });
                                    }

                                    break;
                            }

                            break;
                        case (byte)AdditionalTypes.RecoveryAndDamagePercent.HPReduced:
                            switch (session)
                            {
                                case Character.Character receiverCharacter:
                                    int loss = (int)(receiverCharacter.HpLoad() * (FirstData * 0.01));
                                    IDisposable rObs;
                                    Card rCard = ServerManager.Instance.GetCardByCardId(CardId);

                                    if (rCard == null)
                                    {
                                        return;
                                    }

                                    if (rCard.Duration <= 0)
                                    {
                                        receiverCharacter.DotDebuff = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (receiverCharacter.Hp > 0)
                                            {
                                                receiverCharacter.Hp = receiverCharacter.Hp - loss <= 0 ? 1 : receiverCharacter.Hp - loss;
                                                receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm((ushort)loss));
                                                receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                            }
                                        });
                                        break;
                                    }

                                    rObs = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                    {
                                        if (receiverCharacter.Hp > 0)
                                        {
                                            receiverCharacter.Hp = receiverCharacter.Hp - loss <= 0 ? 1 : receiverCharacter.Hp - loss;
                                            receiverCharacter.MapInstance?.Broadcast(receiverCharacter.GenerateDm((ushort)loss));
                                            receiverCharacter.Session.SendPacket(receiverCharacter.GenerateStat());
                                        }
                                    });

                                    Observable.Timer(TimeSpan.FromSeconds(rCard.Duration * 0.1)).Subscribe(s => { rObs.Dispose(); });

                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.Count:
                    break;

                case BCardType.CardType.NoDefeatAndNoDamage:
                    switch (SubType)
                    {
                        //case (byte)AdditionalTypes.NoDefeatAndNoDamage.TransferAttackPower: // = Charge
                        case (byte)AdditionalTypes.NoDefeatAndNoDamage.NeverReceiveDamage:
                            switch (session)
                            {
                                case Character.Character receiverCharacter:
                                    receiverCharacter.HasGodMode = true;
                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.SpecialActions:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.SpecialActions.Hide:
                            switch (session)
                            {
                                case Character.Character receiverCharacter:
                                    receiverCharacter.Invisible = true;
                                    receiverCharacter.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                                        receiverCharacter.Session.CurrentMapInstance?.Broadcast(s.GenerateOut()));
                                    receiverCharacter.Session.CurrentMapInstance?.Broadcast(receiverCharacter.GenerateInvisible());
                                    break;
                            }

                            break;
                        case (byte)AdditionalTypes.SpecialActions.FocusEnemies:
                            long entityId;
                            UserType uType;
                            switch (caster)
                            {
                                case Character.Character senderCharacter:
                                    switch (session)
                                    {
                                        case MapMonster receiverMapMonster:
                                            entityId = receiverMapMonster.MapMonsterId;
                                            uType = UserType.Monster;
                                            break;
                                        case Character.Character receiverCharacter:
                                            entityId = receiverCharacter.CharacterId;
                                            uType = UserType.Player;
                                            break;
                                        case Mate receiverMate:
                                            entityId = receiverMate.MateTransportId;
                                            uType = UserType.Npc;
                                            break;
                                        default:
                                            return;
                                    }

                                    Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(s =>
                                    {
                                        senderCharacter.MapInstance?.Broadcast($"guri 3 {(short)uType} {entityId} {senderCharacter.PositionX} {senderCharacter.PositionY} 3 {SecondData} 2 -1");
                                    });
                                    break;
                                case MapMonster senderMapMonster:
                                    switch (session)
                                    {
                                        case MapMonster receiverMapMonster:
                                            entityId = receiverMapMonster.MapMonsterId;
                                            uType = UserType.Monster;
                                            break;
                                        case Character.Character receiverCharacter:
                                            entityId = receiverCharacter.CharacterId;
                                            uType = UserType.Player;
                                            break;
                                        case Mate receiverMate:
                                            entityId = receiverMate.MateTransportId;
                                            uType = UserType.Npc;
                                            break;
                                        default:
                                            return;
                                    }

                                    Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(s =>
                                    {
                                        senderMapMonster.MapInstance?.Broadcast($"guri 3 {(short)uType} {entityId} {senderMapMonster.MapX} {senderMapMonster.MapY} 3 {SecondData} 2 -1");
                                    });
                                    break;
                                case Mate senderMate:
                                    switch (session)
                                    {
                                        case MapMonster receiverMapMonster:
                                            entityId = receiverMapMonster.MapMonsterId;
                                            uType = UserType.Monster;
                                            break;
                                        case Character.Character receiverCharacter:
                                            entityId = receiverCharacter.CharacterId;
                                            uType = UserType.Player;
                                            break;
                                        case Mate receiverMate:
                                            entityId = receiverMate.MateTransportId;
                                            uType = UserType.Npc;
                                            break;
                                        default:
                                            return;
                                    }

                                    Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(s =>
                                    {
                                        senderMate.MapInstance?.Broadcast($"guri 3 {(short)uType} {entityId} {senderMate.MapX} {senderMate.MapY} 3 {SecondData} 2 -1");
                                    });
                                    break;
                            }

                            break;
                        case (byte)AdditionalTypes.SpecialActions.PushBack:
                        {
                            //Todo: review this clean
                            switch (session)
                            {
                                case MapMonster monster when caster is Character.Character pusher:
                                    if (!monster.MapInstance.Map.GetDefinedPosition(pusher.PositionX + FirstData, pusher.PositionY))
                                    {
                                        break;
                                    }

                                    pusher.MapInstance?.Broadcast($"guri 3 3 {monster.MapMonsterId} {pusher.PositionX + FirstData} {pusher.PositionY} 3 8 2 - 1");
                                    monster.MapX = pusher.PositionX += (short)FirstData;
                                    monster.MapY = pusher.PositionY;
                                    break;
                                case Character.Character target when caster is Character.Character pusher:
                                    if (!target.MapInstance.Map.GetDefinedPosition(pusher.PositionX + FirstData, pusher.PositionY))
                                    {
                                        break;
                                    }

                                    pusher.MapInstance?.Broadcast($"guri 3 1 {target.CharacterId} {pusher.PositionX + FirstData} {pusher.PositionY} 3 8 2 - 1");
                                    target.PositionX = pusher.PositionX += (short)FirstData;
                                    target.PositionY = pusher.PositionY;
                                    break;
                            }

                            break;
                        }
                    }

                    break;

                case BCardType.CardType.Mode:
                    break;

                case BCardType.CardType.NoCharacteristicValue:
                    break;

                case BCardType.CardType.LightAndShadow:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.LightAndShadow.RemoveBadEffects:
                            List<BuffType> buffsToDisable = new List<BuffType> { BuffType.Bad };
                            switch (session)
                            {
                                case Character.Character isCharacter:
                                    isCharacter.DisableBuffs(buffsToDisable, FirstData);
                                    break;
                                case Mate isMate:
                                    isMate.BattleEntity.DisableBuffs(buffsToDisable, FirstData);
                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.Item:
                    break;

                case BCardType.CardType.DebuffResistance:
                    break;

                case BCardType.CardType.SpecialBehaviour:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.SpecialBehaviour.InflictOnTeam:
                            int delay = ThirdData + 1;
                            IDisposable teamObs = null;
                            switch (session)
                            {
                                case MapMonster inRangeMapMonster:
                                {
                                    int range = FirstData;
                                    int timer = ThirdData + 1;
                                    Card buffCard = ServerManager.Instance.GetCardByCardId((short)SecondData);
                                    IEnumerable entitiesInRange = inRangeMapMonster.MapInstance?.GetListMonsterInRange(inRangeMapMonster.MapX, inRangeMapMonster.MapY, (byte)range);
                                    if (entitiesInRange == null || buffCard == null)
                                    {
                                        return;
                                    }

                                    teamObs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                    {
                                        foreach (MapMonster monster in entitiesInRange)
                                        {
                                            if (monster.Buffs.All(x => x.Card.CardId != buffCard.CardId))
                                            {
                                                monster.AddBuff(new Buff(SecondData, entity: caster));
                                            }
                                        }
                                    });

                                    Observable.Timer(TimeSpan.FromSeconds(buffCard.Duration * 0.1)).Subscribe(s => { teamObs.Dispose(); });
                                    break;
                                }

                                case Character.Character inRangeCharacter:
                                {
                                    int range = FirstData;
                                    int timer = ThirdData + 1;
                                    Card buffCard = ServerManager.Instance.GetCardByCardId((short)SecondData);
                                    IEnumerable entitiesInRange = inRangeCharacter.MapInstance?.GetCharactersInRange(inRangeCharacter.MapX, inRangeCharacter.MapY, (byte)range);
                                    if (entitiesInRange == null || buffCard == null)
                                    {
                                        return;
                                    }

                                    teamObs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                    {
                                        foreach (Character.Character characterInRange in entitiesInRange)
                                        {
                                            if (characterInRange.Buff.All(x => x.Card.CardId != buffCard.CardId))
                                            {
                                                characterInRange.AddBuff(new Buff(SecondData, entity: caster));
                                            }
                                        }
                                    });

                                    Observable.Timer(TimeSpan.FromSeconds(buffCard.Duration * 0.1)).Subscribe(s => { teamObs.Dispose(); });
                                    break;
                                }

                                case Mate inRangeMate:
                                {
                                    int range = FirstData;
                                    int timer = ThirdData + 1;
                                    Card buffCard = ServerManager.Instance.GetCardByCardId((short)SecondData);
                                    IEnumerable entitiesInRange = inRangeMate.MapInstance?.GetMatesInRange(inRangeMate.MapX, inRangeMate.MapY, (byte)range);
                                    if (entitiesInRange == null || buffCard == null)
                                    {
                                        return;
                                    }

                                    teamObs = Observable.Interval(TimeSpan.FromSeconds(timer)).Subscribe(s =>
                                    {
                                        foreach (Mate mateInRange in entitiesInRange)
                                        {
                                            if (mateInRange.Buffs.All(x => x.Card.CardId != buffCard.CardId))
                                            {
                                                mateInRange.AddBuff(new Buff(SecondData, entity: caster));
                                            }
                                        }
                                    });

                                    Observable.Timer(TimeSpan.FromSeconds(buffCard.Duration * 0.1)).Subscribe(s => { teamObs.Dispose(); });
                                    break;
                                }
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.Quest:
                    break;

                case BCardType.CardType.SecondSPCard:
                    break;

                case BCardType.CardType.SPCardUpgrade:
                    break;

                case BCardType.CardType.HugeSnowman:
                    break;

                case BCardType.CardType.Drain:
                    IDisposable drainObservable = null;
                    Card drainCard = ServerManager.Instance.GetCardByCardId(CardId);
                    int drain = 0;
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.Drain.TransferEnemyHP:
                            switch (session)
                            {
                                case MapMonster targetMonster when caster is Character.Character casterChar:
                                    if (IsLevelScaled)
                                    {
                                        if (drainCard == null)
                                        {
                                            break;
                                        }

                                        drain = casterChar.Level * FirstData;
                                        drainObservable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (targetMonster.CurrentHp > 0)
                                            {
                                                targetMonster.CurrentHp = targetMonster.CurrentHp - drain < 0 ? 1 : targetMonster.CurrentHp - drain;
                                                casterChar.Hp = (int)(casterChar.Hp + drain > casterChar.HpLoad() ? casterChar.HpLoad() : casterChar.Hp + drain);
                                                casterChar.MapInstance?.Broadcast(casterChar.GenerateRc(drain));
                                                casterChar.MapInstance?.Broadcast(targetMonster.GenerateDm((ushort)drain));
                                            }
                                            else
                                            {
                                                drainObservable?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(drainCard.Duration * 0.1)).Subscribe(s => { drainObservable?.Dispose(); });
                                    }

                                    break;
                                case Character.Character targetCharacter when caster is Character.Character casterChar:
                                    if (IsLevelScaled)
                                    {
                                        if (drainCard == null)
                                        {
                                            break;
                                        }

                                        drain = casterChar.Level * FirstData;
                                        drainObservable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (targetCharacter.Hp > 0)
                                            {
                                                targetCharacter.Hp = targetCharacter.Hp - drain < 0 ? 1 : targetCharacter.Hp - drain;
                                                casterChar.Hp = (int)(casterChar.Hp + drain > casterChar.HpLoad() ? casterChar.HpLoad() : casterChar.Hp + drain);
                                                casterChar.MapInstance?.Broadcast(casterChar.GenerateRc(drain));
                                                casterChar.MapInstance?.Broadcast(targetCharacter.GenerateDm((ushort)drain));
                                            }
                                            else
                                            {
                                                drainObservable?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(drainCard.Duration * 0.1)).Subscribe(s => { drainObservable?.Dispose(); });
                                    }

                                    break;

                                case Character.Character targetCharacter when caster is MapMonster casterMapMonster:
                                    if (IsLevelScaled)
                                    {
                                        if (drainCard == null)
                                        {
                                            break;
                                        }

                                        drain = casterMapMonster.Monster.Level * FirstData;
                                        drainObservable = Observable.Interval(TimeSpan.FromSeconds(ThirdData + 1)).Subscribe(s =>
                                        {
                                            if (targetCharacter.Hp > 0)
                                            {
                                                targetCharacter.Hp = targetCharacter.Hp - drain < 0 ? 1 : targetCharacter.Hp - drain;
                                                casterMapMonster.CurrentHp = casterMapMonster.CurrentHp + drain > casterMapMonster.MaxHp ? casterMapMonster.MaxHp : casterMapMonster.CurrentHp + drain;
                                                casterMapMonster.MapInstance?.Broadcast(casterMapMonster.GenerateRc(drain));
                                                casterMapMonster.MapInstance?.Broadcast(targetCharacter.GenerateDm((ushort)drain));
                                            }
                                            else
                                            {
                                                drainObservable?.Dispose();
                                            }
                                        });

                                        Observable.Timer(TimeSpan.FromSeconds(drainCard.Duration * 0.1)).Subscribe(s => { drainObservable?.Dispose(); });
                                    }

                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.BossMonstersSkill:
                    break;

                case BCardType.CardType.LordHatus:
                    break;

                case BCardType.CardType.LordCalvinas:
                    break;

                case BCardType.CardType.SESpecialist:
                    break;

                case BCardType.CardType.FourthGlacernonFamilyRaid:
                    break;

                case BCardType.CardType.SummonedMonsterAttack:
                    break;

                case BCardType.CardType.BearSpirit:
                    break;

                case BCardType.CardType.SummonSkill:
                    break;

                case BCardType.CardType.InflictSkill:
                    break;

                case BCardType.CardType.HideBarrelSkill:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.HideBarrelSkill.NoHPConsumption:
                            switch (session)
                            {
                                case Character.Character receiverCharacter:
                                    receiverCharacter.HasGodMode = true;
                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.FocusEnemyAttentionSkill:
                    break;

                case BCardType.CardType.TauntSkill:
                    /*switch (SubType)
                    {
                        case (byte)AdditionalTypes.TauntSkill.ReflectsMaximumDamageFromNegated:
                            switch (session)
                            {
                                case Character recevierCharacter:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }

                                    recevierCharacter.BattleEntity.IsReflecting = true;

                                    recevierCharacter.ReflectiveBuffs[CardId.Value] = FirstData;

                                    break;
                                case MapMonster receiverMapMonster:
                                    receiverMapMonster.BattleEntity.IsReflecting = true;
                                    if (!CardId.HasValue)
                                    {
                                        return;
                                    }

                                    receiverMapMonster.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                                case Mate receiverMate:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }
                                    receiverMate.BattleEntity.IsReflecting = true;

                                    receiverMate.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                            }
                            break;
                    }*/
                    break;

                case BCardType.CardType.FireCannoneerRangeBuff:
                    break;

                case BCardType.CardType.VulcanoElementBuff:
                    break;

                case BCardType.CardType.DamageConvertingSkill:
                    /*switch (SubType)
                    {
                        case (byte)AdditionalTypes.DamageConvertingSkill.ReflectMaximumReceivedDamage:
                            switch (session)
                            {
                                case Character recevierCharacter:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }
                                    recevierCharacter.BattleEntity.IsReflecting = true;

                                    recevierCharacter.ReflectiveBuffs[CardId.Value] = FirstData;

                                    break;
                                case MapMonster receiverMapMonster:
                                    if (!CardId.HasValue || CardId == 663)
                                    {
                                        return;
                                    }
                                    receiverMapMonster.BattleEntity.IsReflecting = true;

                                    receiverMapMonster.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                                case Mate receiverMate:
                                    if (!CardId.HasValue || CardId == 663 || receiverMate == null)
                                    {
                                        return;
                                    }
                                    receiverMate.BattleEntity.IsReflecting = true;

                                    receiverMate.ReflectiveBuffs[CardId.Value] = FirstData;
                                    break;
                            }
                            break;
                    }*/
                    break;

                case BCardType.CardType.MeditationSkill:
                    if (session.GetSession().GetType() == typeof(Character.Character))
                    {
                        if (SubType.Equals((byte)AdditionalTypes.MeditationSkill.CausingChance))
                        {
                            if (ServerManager.Instance.RandomNumber() < FirstData)
                            {
                                if (character == null)
                                {
                                    break;
                                }

                                if (SkillVNum.HasValue)
                                {
                                    Skill skill = ServerManager.Instance.GetSkill(SkillVNum.Value);
                                    Skill newSkill = ServerManager.Instance.GetSkill((short)SecondData);
                                    Observable.Timer(TimeSpan.FromMilliseconds(100)).Subscribe(observer =>
                                    {
                                        foreach (QuicklistEntryDTO quicklistEntry in character.QuicklistEntries.Where(s => s.Pos.Equals(skill.CastId)))
                                        {
                                            character.Session.SendPacket($"qset {quicklistEntry.Q1} {quicklistEntry.Q2} {quicklistEntry.Type}.{quicklistEntry.Slot}.{newSkill.CastId}.0");
                                        }

                                        character.Session.SendPacket($"mslot {newSkill.CastId} -1");
                                    });
                                    character.SkillComboCount++;
                                    character.LastSkillCombo = DateTime.Now;
                                    if (skill.CastId > 10)
                                    {
                                        // HACK this way
                                        Observable.Timer(TimeSpan.FromMilliseconds(skill.Cooldown * 100 + 500))
                                            .Subscribe(observer => { character.Session.SendPacket($"sr {skill.CastId}"); });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (character == null)
                            {
                                break;
                            }

                            switch (SubType)
                            {
                                case 21:
                                    character.MeditationDictionary[(short)SecondData] = DateTime.Now.AddSeconds(4);
                                    break;
                                case 31:
                                    character.MeditationDictionary[(short)SecondData] = DateTime.Now.AddSeconds(8);
                                    break;
                                case 41:
                                    character.MeditationDictionary[(short)SecondData] = DateTime.Now.AddSeconds(12);
                                    break;
                            }
                        }
                    }

                    break;

                case BCardType.CardType.FalconSkill:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.FalconSkill.CausingChanceLocation:
                            if (session is Character.Character trapper)
                            {
                                var trap = new MapMonster
                                {
                                    MonsterVNum = 1436,
                                    MapX = trapper.PositionX,
                                    MapY = trapper.PositionY,
                                    MapMonsterId = trapper.MapInstance.GetNextId(),
                                    IsHostile = false,
                                    IsMoving = false,
                                    ShouldRespawn = false
                                };

                                trapper.MapInstance?.AddMonster(trap);
                                trap.Initialize();
                                trapper.MapInstance?.Broadcast(trap.GenerateIn());

                                IDisposable dispo = null;

                                Thread.Sleep(1000);

                                dispo = Observable.Interval(TimeSpan.FromMilliseconds(250)).Subscribe(s =>
                                {
                                    if (trapper.MapInstance.IsPvp)
                                    {
                                        foreach (Character.Character trapped in trapper.MapInstance.GetCharactersInRange(trap.MapX, trap.MapY, 2).Where(p => p.CharacterId != trapper.CharacterId))
                                        {
                                            trapper.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, trap.MapMonsterId, 3, trap.MapMonsterId, 1250, 600, 11, 4270, trap.MapX,
                                                trap.MapY, true, 0, 0, -2, 0));
                                            trapped.AddBuff(new Buff(572, 1));
                                            trapped.AddBuff(new Buff(557, 1));
                                            trapper.MapInstance.RemoveMonster(trap);
                                            trapper.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, trap.MapMonsterId));
                                            dispo?.Dispose();
                                        }
                                    }

                                    foreach (MapMonster trappedMonster in trapper.MapInstance.GetListMonsterInRange(trap.MapX, trap.MapY, 2).Where(m => m.MapMonsterId != trap.MapMonsterId))
                                    {
                                        trapper.MapInstance.Broadcast(StaticPacketHelper.SkillUsed(UserType.Monster, trap.MapMonsterId, 3, trap.MapMonsterId, 1250, 600, 11, 4270, trap.MapX, trap.MapY,
                                            true, 0, 0, -2, 0));
                                        trappedMonster.AddBuff(new Buff(572, 1));
                                        trappedMonster.AddBuff(new Buff(557, 1));
                                        trapper.MapInstance.RemoveMonster(trap);
                                        trapper.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, trap.MapMonsterId));
                                        dispo?.Dispose();
                                    }
                                });

                                Observable.Timer(TimeSpan.FromSeconds(60)).Subscribe(s =>
                                {
                                    trapper.MapInstance.RemoveMonster(trap);
                                    trapper.MapInstance.Broadcast(StaticPacketHelper.Out(UserType.Monster, trap.MapMonsterId));
                                    dispo?.Dispose();
                                });
                            }

                            break;
                        case (byte)AdditionalTypes.FalconSkill.Hide:
                            if (character == null)
                            {
                                break;
                            }

                            character.Invisible = true;
                            character.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                                character.Session.CurrentMapInstance?.Broadcast(s.GenerateOut()));
                            character.Session.CurrentMapInstance?.Broadcast(character.GenerateInvisible());
                            break;
                    }

                    break;

                case BCardType.CardType.AbsorptionAndPowerSkill:
                    break;

                case BCardType.CardType.LeonaPassiveSkill:
                    break;

                case BCardType.CardType.FearSkill:
                    if (session is Character.Character Fear)
                    {
                        switch (SubType)
                        {
                            case (byte)AdditionalTypes.FearSkill.MoveAgainstWill:
                                Fear.Session.SendPacket($"rv_m {Fear.CharacterId} 1 1");
                                Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(s => { Fear.Session.CurrentMapInstance?.Broadcast($"rv_m {Fear.CharacterId} 1 0"); });
                                break;
                        }
                    }

                    break;

                case BCardType.CardType.SniperAttack:
                    break;

                case BCardType.CardType.FrozenDebuff:
                    break;

                case BCardType.CardType.JumpBackPush:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.JumpBackPush.JumpBackChance:
                            switch (session)
                            {
                                case MapMonster targetMob when caster is Character.Character pushedbackChar:
                                    pushedbackChar.PushBackToDirection(SecondData / 2);
                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.FairyXPIncrease:
                    break;

                case BCardType.CardType.SummonAndRecoverHP:
                    break;

                case BCardType.CardType.TeamArenaBuff:
                    break;

                case BCardType.CardType.ArenaCamera:
                    break;

                case BCardType.CardType.DarkCloneSummon:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DarkCloneSummon.ConvertDamageToHPChance:
                            switch (session)
                            {
                                case Character.Character thoughtCharacter:
                                    Card thoughtCard = ServerManager.Instance.GetCardByCardId(CardId);

                                    if (thoughtCard == null)
                                    {
                                        break;
                                    }

                                    thoughtCharacter.RetainedHp = thoughtCharacter.Hp;

                                    Observable.Timer(TimeSpan.FromSeconds(SecondData)).Subscribe(s =>
                                    {
                                        int total = thoughtCharacter.RetainedHp - thoughtCharacter.AccumulatedDamage;
                                        thoughtCharacter.Hp = total <= 0 ? 1 : total;
                                        thoughtCharacter.AccumulatedDamage = 0;
                                    });
                                    break;
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.AbsorbedSpirit:
                    break;

                case BCardType.CardType.AngerSkill:
                    break;

                case BCardType.CardType.MeteoriteTeleport:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.MeteoriteTeleport.CauseMeteoriteFall:
                            if (IsLevelScaled)
                            {
                                switch (session)
                                {
                                    case Character.Character meteorCharacter:
                                        if (!SkillVNum.HasValue)
                                        {
                                            break;
                                        }

                                        Skill sk = ServerManager.Instance.GetSkill(SkillVNum.Value);
                                        int amount = meteorCharacter.Level / 5 + 10;
                                        int delay = 500;
                                        for (int i = 0; i < amount; i++)
                                        {
                                            meteorCharacter.MapInstance?.SpawnMeteorsOnRadius(20, meteorCharacter.Session, sk);
                                            if (delay > 0)
                                            {
                                                Thread.Sleep(delay);
                                            }

                                            delay -= delay > 100 ? 20 : 0;
                                        }

                                        break;
                                }
                            }

                            break;
                    }

                    break;

                case BCardType.CardType.StealBuff:
                    break;

                case BCardType.CardType.EffectSummon:
                    break;

                case BCardType.CardType.DragonSkills:
                    switch (SubType)
                    {
                        case (byte)AdditionalTypes.DragonSkills.TransformationInverted:
                            if (session is Character.Character reversedMorph)
                            {
                                reversedMorph.Morph = (byte)BrawlerMorphType.Normal;
                                reversedMorph.Session.SendPacket(reversedMorph.GenerateCMode());
                                reversedMorph.Session.SendPacket(reversedMorph.GenerateEff(196));
                                reversedMorph.DragonModeObservable?.Dispose();
                                reversedMorph.RemoveBuff(676);
                            }

                            break;
                        case (byte)AdditionalTypes.DragonSkills.Transformation:
                            Card morphCard = ServerManager.Instance.GetCardByCardId(CardId);

                            if (morphCard == null)
                            {
                                return;
                            }

                            if (session is Character.Character morphedChar)
                            {
                                morphedChar.Morph = (byte)BrawlerMorphType.Dragon;
                                morphedChar.Session.SendPacket(morphedChar.GenerateCMode());
                                morphedChar.Session.SendPacket(morphedChar.GenerateEff(196));
                                morphedChar.DragonModeObservable?.Dispose();

                                morphedChar.DragonModeObservable = Observable.Timer(TimeSpan.FromSeconds(morphCard.Duration * 0.1)).Subscribe(s =>
                                {
                                    morphedChar.Morph = (byte)BrawlerMorphType.Normal;
                                    morphedChar.Session.SendPacket(morphedChar.GenerateCMode());
                                    morphedChar.Session.SendPacket(morphedChar.GenerateEff(196));
                                });
                            }

                            break;
                    }

                    break;

                default:
                    Logger.Error(new ArgumentOutOfRangeException($"Card Type {Type} not defined!"));
                    //throw new ArgumentOutOfRangeException();
                    break;
            }
        }
        
        #endregion
    }
}