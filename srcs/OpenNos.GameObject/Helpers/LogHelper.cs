﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.DAL;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.Packets;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Helpers
{
    public class LogHelper
    {
        public LogHelper()
        {
            QuestLogList = new List<QuestLogDTO>();
            RaidLogList = new List<RaidLogDTO>();
            LogCommandsList = new List<LogCommandsDTO>();
            ChatLogList = new List<LogChatDTO>();
        }

        public void InsertCommandLog(long characterId, PacketDefinition commandPacket, string ipAddress)
        {
            string withoutHeaderpacket = string.Empty;
            string[] packet = commandPacket.OriginalContent.Split(' ');
            for (int i = 1; i < packet.Length; i++)
            {
                withoutHeaderpacket += $" {packet[i]}";
            }

            var command = new LogCommandsDTO
            {
                CharacterId = characterId,
                Command = commandPacket.OriginalHeader,
                Data = withoutHeaderpacket,
                IpAddress = ipAddress,
                Timestamp = DateTime.Now
            };
            DaoFactory.Instance.LogCommandsDao.InsertOrUpdate(ref command);
        }

        public void InsertChatLog(ChatType type, long characterId, string message, string ipAddress)
        {
            var log = new LogChatDTO
            {
                CharacterId = characterId,
                ChatMessage = message,
                IpAddress = ipAddress,
                ChatType = (byte)type,
                Timestamp = DateTime.Now
            };
            DaoFactory.Instance.LogChatDao.InsertOrUpdate(ref log);
        }

        public void InsertQuestLog(long characterId, string ipAddress, long questId, DateTime lastDaily)
        {
            var log = new QuestLogDTO
            {
                CharacterId = characterId,
                IpAddress = ipAddress,
                QuestId = questId,
                LastDaily = lastDaily
            };
            DaoFactory.Instance.QuestLogDao.InsertOrUpdate(ref log);
        }

        public void InsertRaidLog(long characterId, long raidId, DateTime time)
        {
            var log = new RaidLogDTO
            {
                CharacterId = characterId,
                RaidId = raidId,
                Time = time
            };
            DaoFactory.Instance.RaidLogDao.InsertOrUpdate(ref log);
        }

        public void InsertFamilyRaidLog(long familyId, long raidId, DateTime time)
        {
            var log = new RaidLogDTO
            {
                FamilyId = familyId,
                RaidId = raidId,
                Time = time
            };
            DaoFactory.Instance.RaidLogDao.InsertOrUpdate(ref log);
        }

        public void InsertUpgradeLog(ClientSession session, string upgradeType, bool? hasAmulet, bool success, ItemInstance item)
        {
            var log = new UpgradeLogDTO
            {
                AccountId = session.Account.AccountId,
                CharacterId = session.Character.CharacterId,
                CharacterName = session.Character.Name,
                UpgradeType = upgradeType,
                HasAmulet = hasAmulet,
                Date = DateTime.Now,
                Success = success,
                ItemVnum = item.ItemVNum,
                ItemName = item.Item.Name
            };
            DaoFactory.Instance.UpgradeLogDao.InsertOrUpdate(ref log);
        }

        public void InsertExchangeLog(ClientSession session, ClientSession targetSession, short itemVnum, short itemAmount, long gold)
        {
            var log = new ExchangeLogDTO
            {
                AccountId = session.Account.AccountId,
                CharacterId = session.Character.CharacterId,
                CharacterName = session.Character.Name,
                TargetAccountId = targetSession.Account.AccountId,
                TargetCharacterId = targetSession.Character.CharacterId,
                TargetCharacterName = targetSession.Character.Name,
                ItemVnum = itemVnum,
                ItemAmount = itemAmount,
                Gold = gold,
                Date = DateTime.Now
            };
            DaoFactory.Instance.ExchangeLogDao.InsertOrUpdate(ref log);
        }

        public void InsertAntiBotLog(ClientSession session, bool timeout)
        {
            var log = new AntiBotLogDTO
            {
                CharacterId = session.Character.CharacterId,
                CharacterName = session.Character.Name,
                DateTime = DateTime.Now,
                Timeout = timeout
            };
            DaoFactory.Instance.AntiBotLogDao.InsertOrUpdate(ref log);
        }

        #region Properties

        public List<QuestLogDTO> QuestLogList;
        public List<RaidLogDTO> RaidLogList;
        public List<LogCommandsDTO> LogCommandsList;
        public List<LogChatDTO> ChatLogList;

        #endregion

        #region Singleton

        private static LogHelper _instance;

        public static LogHelper Instance => _instance ?? (_instance = new LogHelper());

        #endregion
    }
}