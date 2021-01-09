﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs.Character;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject
{
    public class Group
    {
        #region Instantiation

        public Group(GroupType type)
        {
            Characters = new ConcurrentBag<ClientSession>();
            GroupId = ServerManager.Instance.GetNextGroupId();
            _order = 0;
            GroupType = type;
        }

        #endregion

        #region Members

        private int _order;

        private readonly object _syncObj = new object();

        public GroupType GroupType { get; set; }

        public ScriptedInstance Raid { get; set; }

        #endregion

        #region Properties

        public int CharacterCount => Characters.Count;

        public ConcurrentBag<ClientSession> Characters { get; set; }

        public long GroupId { get; set; }

        public byte SharingMode { get; set; }

        #endregion

        #region Methods

        public List<string> GeneratePst(ClientSession player)
        {
            List<string> str = new List<string>();
            int i = 0;
            foreach (ClientSession session in Characters)
            {
                if (session == player)
                {
                    str.AddRange(player.Character.Mates.Where(s => s.IsTeamMember).OrderByDescending(s => s.MateType)
                        .Select(mate =>
                            $"pst 2 {mate.MateTransportId} {(mate.MateType == MateType.Partner ? "0" : "1")} {mate.Hp / mate.MaxHp * 100} {mate.Mp / mate.MaxMp * 100} {mate.Hp} {mate.Mp} 0 0 0{mate.Buffs.Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}"));
                    i = session.Character.Mates.Count(s => s.IsTeamMember);
                    str.Add(
                        $"pst 1 {session.Character.CharacterId} {++i} {(int)(session.Character.Hp / session.Character.HpLoad() * 100)} {(int)(session.Character.Mp / session.Character.MpLoad() * 100)} {session.Character.HpLoad()} {session.Character.MpLoad()} {(byte)session.Character.Class} {(byte)session.Character.Gender} {(session.Character.UseSp ? session.Character.Morph : 0)}");
                    continue;
                }

                str.Add(
                    $"pst 1 {session.Character.CharacterId} {++i} {(int)(session.Character.Hp / session.Character.HpLoad() * 100)} {(int)(session.Character.Mp / session.Character.MpLoad() * 100)} {session.Character.HpLoad()} {session.Character.MpLoad()} {(byte)session.Character.Class} {(byte)session.Character.Gender} {(session.Character.UseSp ? session.Character.Morph : 0)}{session.Character.Buff.Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}")}");
            }

            return str;
        }

        public long? GetNextOrderedCharacterId(Character.Character character)
        {
            lock(_syncObj)
            {
                _order++;
                List<ClientSession> sessions =
                    Characters.Where(s => Map.GetDistance(s.Character, character) < 50).ToList();
                if (_order > sessions.Count - 1) // if order wents out of amount of ppl, reset it -> zero based index
                {
                    _order = 0;
                }

                if (sessions.Count == 0) // group seems to be empty
                {
                    return null;
                }

                return sessions[_order].Character.CharacterId;
            }
        }

        public bool IsMemberOfGroup(long characterId)
        {
            return Characters != null && Characters.Any(s => s?.Character?.CharacterId == characterId);
        }

        public bool IsMemberOfGroup(ClientSession session)
        {
            return Characters != null &&
                Characters.Any(s => s?.Character?.CharacterId == session.Character.CharacterId);
        }

        public string GenerateRdlst()
        {
            string result =
                $"rdlst{(GroupType == GroupType.GiantTeam ? "f" : "")} {Raid?.LevelMinimum} {Raid?.LevelMaximum} 0";
            Characters?.ToList().ForEach(session =>
                result +=
                    $" {session.Character.Level}.{(session.Character.UseSp || session.Character.IsVehicled ? session.Character.Morph : -1)}.{(short)session.Character.Class}.{Raid?.FirstMap?.InstanceBag.DeadList.Count(s => s == session.Character.CharacterId) ?? 0}.{session.Character.Name}.{(short)session.Character.Gender}.{session.Character.CharacterId}.{session.Character.HeroLevel}");

            return result;
        }

        public void JoinGroup(long characterId)
        {
            ClientSession session = ServerManager.Instance.GetSessionByCharacterId(characterId);
            if (session != null)
            {
                JoinGroup(session);
            }
        }

        public void CheckRelations()
        {
            foreach (ClientSession session in Characters.Where(s => s != null))
            {
                foreach (CharacterRelationDTO relation in session.Character.CharacterRelations)
                {
                    if (relation.RelationType != CharacterRelationType.Spouse)
                    {
                        continue;
                    }

                    if (relation.CharacterId == session.Character.CharacterId)
                    {
                        if (Characters.All(s => s.Character.CharacterId != relation.RelatedCharacterId))
                        {
                            continue;
                        }

                        session.Character.AddBuff(new Buff.Buff(319, isPermaBuff: true));
                        session.Character.GenerateEff(881);
                        Characters.FirstOrDefault(s => s.Character.CharacterId == relation.RelatedCharacterId)?.Character.AddBuff(new Buff.Buff(319, isPermaBuff: true));
                        Characters.FirstOrDefault(s => s.Character.CharacterId == relation.RelatedCharacterId)?.Character.GenerateEff(881);
                    }

                    else if (relation.RelatedCharacterId == session.Character.CharacterId)
                    {
                        if (Characters.All(s => s.Character.CharacterId != relation.CharacterId))
                        {
                            continue;
                        }

                        session.Character.AddBuff(new Buff.Buff(319, isPermaBuff: true));
                        session.Character.GenerateEff(881);
                        Characters.FirstOrDefault(s => s.Character.CharacterId == relation.CharacterId)?.Character.AddBuff(new Buff.Buff(319, isPermaBuff: true));
                        Characters.FirstOrDefault(s => s.Character.CharacterId == relation.CharacterId)?.Character.GenerateEff(881);
                    }
                }
            }
        }

        public void JoinGroup(ClientSession session)
        {
            if (session.Character.LastUnregister.AddSeconds(1) > DateTime.Now)
            {
                return;
            }

            session.Character.Group = this;
            session.Character.LastGroupJoin = DateTime.Now;
            Characters.Add(session);
            if (GroupType == GroupType.Group)
            {
                CheckRelations();
            }
        }

        public void LeaveGroup(ClientSession session)
        {
            session.Character.Group = null;
            if (IsLeader(session) && GroupType != GroupType.Group && Characters.Count > 1)
            {
                Characters.ToList().ForEach(s =>
                    s.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("TEAM_LEADER_CHANGE"),
                            Characters.OrderBy(m => m?.Character.LastGroupJoin).FirstOrDefault()?.Character?.Name),
                        0)));
            }

            Characters.RemoveWhere(s => s?.Character.CharacterId != session.Character.CharacterId,
                out ConcurrentBag<ClientSession> sessions);
            Characters = sessions;
            session.Character.RemoveBuff(319);
            session.Character.WeddingEffect?.Dispose();
        }

        public bool IsLeader(ClientSession session)
        {
            if (Characters.Count == 0)
            {
                return false;
            }

            ClientSession sess = Characters.OrderBy(s => s.Character.LastGroupJoin).ElementAtOrDefault(0);
            return sess != null && sess == session;
        }

        public string GeneraterRaidmbf(MapInstance mapInstance) =>
            $"raidmbf {mapInstance?.MonsterLocker?.Initial} {mapInstance?.MonsterLocker?.Current} {mapInstance?.ButtonLocker?.Initial} {mapInstance?.ButtonLocker?.Current} {Raid?.FirstMap?.InstanceBag?.Lives - Raid?.FirstMap?.InstanceBag?.DeadList.Count()} {Raid?.FirstMap?.InstanceBag?.Lives} 25";

        #endregion
    }
}