﻿// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Maps;
using WingsEmu.Packets.Enums;
using WingsEmu.Packets.ServerPackets;
using WingsEmu.Pathfinder.PathFinder;

namespace OpenNos.GameObject.Battle
{
    public interface IBattleEntity
    {
        #region Properties

        FactionType Faction { get; }

        int CurrentHp { get; set; }

        int DealtDamage { get; set; }

        int MaxHp { get; }

        MapInstance MapInstance { get; }

        BattleEntity BattleEntity { get; set; }

        #endregion

        #region Methods

        Node[][] GetBrushFire();

        MapCell GetPos();

        EffectPacket GenerateEff(int effectid);

        AttackType GetAttackType(Skill skill = null);

        SessionType SessionType();


        /// BUFFS
        /// <summary>
        ///     BUFFS
        /// </summary>
        /// <param name="type"></param>
        /// <param name="indicator"></param>
        /// <returns></returns>
        void AddBuff(Buff.Buff indicator);

        void RemoveBuff(short cardId, bool removePermaBuff = false);

        int[] GetBuff(BCardType.CardType type, byte subtype);

        bool HasBuff(BCardType.CardType type, byte subtype, bool removeWeaponEffects = false);

        bool HasBuff(BuffType type);


        /*
         * DAMAGES
         * 
         */

        void GetDamage(int damage, IBattleEntity entity, bool canKill = true);

        void GenerateDeath(IBattleEntity killer = null);

        void GenerateRewards(IBattleEntity target);

        bool IsTargetable(SessionType type, bool isPvP = false);

        long GetId();

        object GetSession();

        #endregion
    }
}