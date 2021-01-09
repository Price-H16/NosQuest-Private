﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.GameObject.Buff;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Skills;
using WingsEmu.DTOs;

namespace OpenNos.GameObject.Npc
{
    public class NpcMonster : NpcMonsterDTO
    {
        #region Properties

        public List<DropDTO> Drops { get; set; }

        public short FirstX { get; set; }

        public short FirstY { get; set; }

        public List<BCard> BCards { get; set; }

        public DateTime LastEffect { get; private set; }

        public DateTime LastMove { get; private set; }

        public List<NpcMonsterSkill> Skills { get; set; }

        public List<TeleporterDTO> Teleporters { get; set; }

        #endregion

        #region Methods

        public string GenerateEInfo() =>
            $"e_info 10 {NpcMonsterVNum} {Level} {Element} {AttackClass} {ElementRate} {AttackUpgrade} {DamageMinimum} {DamageMaximum} {Concentrate} {CriticalChance} {CriticalRate} {DefenceUpgrade} {CloseDefence} {DefenceDodge} {DistanceDefence} {DistanceDefenceDodge} {MagicDefence} {FireResistance} {WaterResistance} {LightResistance} {DarkResistance} {MaxHP} {MaxMP} -1 {Name.Replace(' ', '^')}";

        public float GetRes(int skillelement)
        {
            switch (skillelement)
            {
                case 0:
                    return FireResistance / 100;

                case 1:
                    return WaterResistance / 100;

                case 2:
                    return LightResistance / 100;

                case 3:
                    return DarkResistance / 100;

                default:
                    return 0f;
            }
        }

        /// <summary>
        ///     Intializes the GameObject, will be injected by AutoMapper after Entity -&gt; GO mapping
        /// </summary>
        public override void Initialize()
        {
            Teleporters = ServerManager.Instance.GetTeleportersByNpcVNum(NpcMonsterVNum);
            Drops = ServerManager.Instance.GetDropsByMonsterVNum(NpcMonsterVNum);
            LastEffect = LastMove = DateTime.Now;
            Skills = ServerManager.Instance.GetNpcMonsterSkillsByMonsterVNum(NpcMonsterVNum);
        }

        #endregion
    }
}