﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.Core.Handling;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using OpenNos.GameObject;
using OpenNos.GameObject.Character;
using OpenNos.GameObject.Event.TalentArena;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Npc;
using OpenNos.GameObject.Quests;
using WingsEmu.Communication;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Character;
using WingsEmu.Packets;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Packets.Enums;
using WingsEmu.Packets.ServerPackets;
using HeroPacket = WingsEmu.Packets.ClientPackets.HeroPacket;
using NcifPacket = WingsEmu.Packets.ClientPackets.NcifPacket;
using SayPacket = WingsEmu.Packets.ClientPackets.SayPacket;

namespace WingsEmu.PacketHandlers
{
    public class BasicPacketHandler : IPacketHandler
    {
        #region Instantiation

        public BasicPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     csp packet
        /// </summary>
        /// <param name="cspPacket"></param>
        public void MessageBubble(CspPacket cspPacket)
        {
            Session.Character.MapInstance?.Broadcast($"csp {cspPacket.CharacterId} {cspPacket.Message}");
        }

        /// <summary>
        ///     gop packet
        /// </summary>
        /// <param name="characterOptionPacket"></param>
        public void CharacterOptionChange(CharacterOptionPacket characterOptionPacket)
        {
            if (characterOptionPacket == null)
            {
                return;
            }

            switch (characterOptionPacket.Option)
            {
                case CharacterOption.BuffBlocked:
                    Session.Character.BuffBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.BuffBlocked ? "BUFF_BLOCKED" : "BUFF_UNLOCKED"), 0));
                    break;

                case CharacterOption.EmoticonsBlocked:
                    Session.Character.EmoticonsBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.EmoticonsBlocked ? "EMO_BLOCKED" : "EMO_UNLOCKED"), 0));
                    break;

                case CharacterOption.ExchangeBlocked:
                    Session.Character.ExchangeBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.ExchangeBlocked ? "EXCHANGE_BLOCKED" : "EXCHANGE_UNLOCKED"), 0));
                    break;

                case CharacterOption.FriendRequestBlocked:
                    Session.Character.FriendRequestBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.FriendRequestBlocked ? "FRIEND_REQ_BLOCKED" : "FRIEND_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupRequestBlocked:
                    Session.Character.GroupRequestBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.GroupRequestBlocked ? "GROUP_REQ_BLOCKED" : "GROUP_REQ_UNLOCKED"),
                        0));
                    break;

                case CharacterOption.PetAutoRelive:
                    Session.Character.IsPetAutoRelive = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.IsPetAutoRelive ? "PET_AUTO_RELIVE_ENABLED" : "PET_AUTO_RELIVE_DISABLED"), 0));
                    break;

                case CharacterOption.PartnerAutoRelive:
                    Session.Character.IsPartnerAutoRelive = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.IsPartnerAutoRelive ? "PARTNER_AUTO_RELIVE_ENABLED" : "PARTNER_AUTO_RELIVE_DISABLED"), 0));
                    break;

                case CharacterOption.HeroChatBlocked:
                    Session.Character.HeroChatBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.HeroChatBlocked ? "HERO_CHAT_BLOCKED" : "HERO_CHAT_UNLOCKED"),
                        0));
                    break;

                case CharacterOption.HpBlocked:
                    Session.Character.HpBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.HpBlocked ? "HP_BLOCKED" : "HP_UNLOCKED"), 0));
                    break;

                case CharacterOption.MinilandInviteBlocked:
                    Session.Character.MinilandInviteBlocked = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.MinilandInviteBlocked ? "MINI_INV_BLOCKED" : "MINI_INV_UNLOCKED"),
                        0));
                    break;

                case CharacterOption.MouseAimLock:
                    Session.Character.MouseAimLock = characterOptionPacket.IsActive;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.MouseAimLock ? "MOUSE_LOCKED" : "MOUSE_UNLOCKED"), 0));
                    break;

                case CharacterOption.QuickGetUp:
                    Session.Character.QuickGetUp = characterOptionPacket.IsActive;
                    Session.SendPacket(
                        UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.QuickGetUp ? "QUICK_GET_UP_ENABLED" : "QUICK_GET_UP_DISABLED"), 0));
                    break;

                case CharacterOption.WhisperBlocked:
                    Session.Character.WhisperBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey(Session.Character.WhisperBlocked ? "WHISPER_BLOCKED" : "WHISPER_UNLOCKED"), 0));
                    break;

                case CharacterOption.FamilyRequestBlocked:
                    Session.Character.FamilyRequestBlocked = characterOptionPacket.IsActive == false;
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(
                        Language.Instance.GetMessageFromKey(Session.Character.FamilyRequestBlocked ? "FAMILY_REQ_LOCKED" : "FAMILY_REQ_UNLOCKED"), 0));
                    break;

                case CharacterOption.GroupSharing:
                    Group grp = ServerManager.Instance.Groups.FirstOrDefault(g => g.IsMemberOfGroup(Session.Character.CharacterId));
                    if (grp == null)
                    {
                        return;
                    }

                    if (grp.IsLeader(Session))
                    {
                        Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_MASTER"), 0));
                        return;
                    }

                    if (characterOptionPacket.IsActive == false)
                    {
                        Group group = ServerManager.Instance.Groups.FirstOrDefault(s => s.IsMemberOfGroup(Session.Character.CharacterId));
                        if (group != null)
                        {
                            group.SharingMode = 1;
                        }

                        Session.CurrentMapInstance?.Broadcast(Session, UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING"), 0), ReceiverType.Group);
                    }
                    else
                    {
                        Group group = ServerManager.Instance.Groups.FirstOrDefault(s => s.IsMemberOfGroup(Session.Character.CharacterId));
                        if (group != null)
                        {
                            group.SharingMode = 0;
                        }

                        Session.CurrentMapInstance?.Broadcast(Session, UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("SHARING_BY_ORDER"), 0), ReceiverType.Group);
                    }

                    break;
            }

            Session.SendPacket(Session.Character.GenerateStat());
        }

        /// <summary>
        ///     compl packet
        /// </summary>
        /// <param name="complimentPacket"></param>
        public void Compliment(ComplimentPacket complimentPacket)
        {
            if (complimentPacket == null)
            {
                return;
            }

            long complimentedCharacterId = complimentPacket.CharacterId;
            if (Session.Character.Level >= 30)
            {
                GeneralLogDTO dto = Session.Character.GeneralLogs.LastOrDefault(s => s.LogData == "World" && s.LogType == "Connection");
                GeneralLogDTO lastcompliment = Session.Character.GeneralLogs.LastOrDefault(s => s.LogData == "World" && s.LogType == "Compliment");
                if (dto != null && dto.Timestamp.AddMinutes(60) <= DateTime.Now)
                {
                    if (lastcompliment == null || lastcompliment.Timestamp.AddDays(1) <= DateTime.Now.Date)
                    {
                        short? compliment = ServerManager.Instance.GetProperty<short?>(complimentedCharacterId, nameof(Character.Compliment));
                        compliment++;
                        ServerManager.Instance.SetProperty(complimentedCharacterId, nameof(Character.Compliment), compliment);
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_GIVEN"), ServerManager.Instance.GetProperty<string>(complimentedCharacterId, nameof(Character.Name))), 12));
                        Session.Character.GeneralLogs.Add(new GeneralLogDTO
                        {
                            AccountId = Session.Account.AccountId,
                            CharacterId = Session.Character.CharacterId,
                            IpAddress = Session.IpAddress,
                            LogData = "World",
                            LogType = "Compliment",
                            Timestamp = DateTime.Now
                        });

                        Session.CurrentMapInstance?.Broadcast(Session,
                            Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_RECEIVED"), Session.Character.Name), 12), ReceiverType.OnlySomeone,
                            characterId: complimentedCharacterId);
                    }
                    else
                    {
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_COOLDOWN"), 11));
                    }
                }
                else
                {
                    if (dto != null)
                    {
                        Session.SendPacket(Session.Character.GenerateSay(
                            string.Format(Language.Instance.GetMessageFromKey("COMPLIMENT_LOGIN_COOLDOWN"), (dto.Timestamp.AddMinutes(60) - DateTime.Now).Minutes), 11));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("COMPLIMENT_NOT_MINLVL"), 11));
            }
        }

        /// <summary>
        ///     dir packet
        /// </summary>
        /// <param name="directionPacket"></param>
        public void Dir(DirectionPacket directionPacket)
        {
            if (directionPacket.CharacterId != Session.Character.CharacterId)
            {
                return;
            }

            Session.Character.Direction = directionPacket.Direction;
            Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateDir());
        }

        /// <summary>
        ///     pcl packet
        /// </summary>
        /// <param name="getGiftPacket"></param>
        public void GetGift(GetGiftPacket getGiftPacket)
        {
            int giftId = getGiftPacket.GiftId;
            if (!Session.Character.MailList.ContainsKey(giftId))
            {
                return;
            }

            MailDTO mail = Session.Character.MailList[giftId];

            if (getGiftPacket.Type == 4 && mail.AttachmentVNum != null)
            {
                if (Session.Character.Inventory.CanAddItem((short)mail.AttachmentVNum))
                {
                    ItemInstance newInv = Session.Character.Inventory
                        .AddNewToInventory((short)mail.AttachmentVNum, mail.AttachmentAmount, upgrade: mail.AttachmentUpgrade, rare: (sbyte)mail.AttachmentRarity).FirstOrDefault();
                    if (newInv == null)
                    {
                        return;
                    }

                    if (newInv.Item.ItemType == ItemType.Shell)
                    {
                        byte[] incompleteShells = { 25, 30, 40, 55, 60, 65, 70, 75, 80, 85 };
                        int rand = ServerManager.Instance.RandomNumber(0, 101);
                        if (!ShellGeneratorHelper.Instance.ShellTypes.TryGetValue(newInv.ItemVNum, out byte shellType))
                        {
                            return;
                        }

                        bool isIncomplete = shellType == 8 || shellType == 9;

                        if (rand < 84)
                        {
                            if (isIncomplete)
                            {
                                newInv.Upgrade = incompleteShells[ServerManager.Instance.RandomNumber(0, 6)];
                            }
                            else
                            {
                                newInv.Upgrade = (byte)ServerManager.Instance.RandomNumber(50, 75);
                            }
                        }
                        else if (rand <= 99)
                        {
                            if (isIncomplete)
                            {
                                newInv.Upgrade = 75;
                            }
                            else
                            {
                                newInv.Upgrade = (byte)ServerManager.Instance.RandomNumber(75, 79);
                            }
                        }
                        else
                        {
                            if (isIncomplete)
                            {
                                newInv.Upgrade = (byte)(ServerManager.Instance.RandomNumber() > 50 ? 85 : 80);
                            }
                            else
                            {
                                newInv.Upgrade = (byte)ServerManager.Instance.RandomNumber(80, 90);
                            }
                        }
                    }

                    if (newInv.Rare != 0)
                    {
                        var wearable = newInv as WearableInstance;
                        wearable?.SetRarityPoint();
                    }

                    var log = new GeneralLogDTO
                    {
                        AccountId = Session.Account.AccountId,
                        CharacterId = Session.Character.CharacterId,
                        LogType = "GETGIFT",
                        LogData = $"{mail.AttachmentVNum} x {mail.AttachmentAmount}",
                        IpAddress = Session.IpAddress,
                        Timestamp = DateTime.Now
                    };
                    DaoFactory.Instance.GeneralLogDao.Insert(log);

                    Session.SendPacket(Session.Character.GenerateSay($"{Language.Instance.GetMessageFromKey("ITEM_GIFTED")}: {newInv.Item.Name} x {mail.AttachmentAmount}", 12));

                    Session.Character.MailList.Remove(giftId);

                    Session.SendPacket($"parcel 2 1 {giftId}");
                    if (Session.Character.MailList.ContainsKey(giftId))
                    {
                        Session.Character.MailList.Remove(giftId);
                    }
                }
                else
                {
                    Session.SendPacket("parcel 5 1 0");
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"), 0));
                }
            }
            else if (getGiftPacket.Type == 5)
            {
                Session.SendPacket($"parcel 7 1 {giftId}");

                if (Session.Character.MailList.ContainsKey(giftId))
                {
                    Session.Character.MailList.Remove(giftId);
                }
            }
        }

        /// <summary>
        ///     ncif packet
        /// </summary>
        /// <param name="ncifPacket"></param>
        public void GetNamedCharacterInformation(NcifPacket ncifPacket)
        {
            switch (ncifPacket.Type)
            {
                // characters
                case 1:
                    Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(ncifPacket.TargetId)?.Character?.GenerateStatInfo());
                    break;

                // npcs/mates
                case 2:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Npcs.Where(n => n.MapNpcId == (int)ncifPacket.TargetId).ToList().ForEach(npc =>
                        {
                            NpcMonster npcinfo = ServerManager.Instance.GetNpc(npc.NpcVNum);
                            if (npcinfo == null)
                            {
                                return;
                            }

                            Session.SendPacket($"st 2 {ncifPacket.TargetId} {npcinfo.Level} {npcinfo.HeroLevel} 100 100 50000 50000");
                        });
                        Parallel.ForEach(Session.CurrentMapInstance.Sessions, session =>
                        {
                            Mate mate = session.Character.Mates.FirstOrDefault(s => s.MateTransportId == (int)ncifPacket.TargetId);
                            if (mate != null)
                            {
                                Session.SendPacket(mate.GenerateStatInfo());
                            }
                        });
                    }

                    break;

                // monsters
                case 3:
                    if (Session.HasCurrentMapInstance)
                    {
                        Session.CurrentMapInstance.Monsters.Where(m => m.MapMonsterId == (int)ncifPacket.TargetId).ToList().ForEach(monster =>
                        {
                            NpcMonster monsterinfo = ServerManager.Instance.GetNpc(monster.MonsterVNum);
                            if (monsterinfo == null)
                            {
                                return;
                            }

                            Session.Character.LastMonsterId = monster.MapMonsterId;
                            Session.SendPacket(
                                $"st 3 {ncifPacket.TargetId} {monsterinfo.Level} {monsterinfo.HeroLevel} {(int)((float)monster.CurrentHp / (float)monster.Monster.MaxHP * 100)} {(int)((float)monster.CurrentMp / (float)monster.Monster.MaxMP * 100)} {monster.CurrentHp} {monster.CurrentMp}{monster.Buffs.Where(s => !s.StaticBuff).Aggregate(string.Empty, (current, buff) => current + $" {buff.Card.CardId}.{buff.Level}")}");
                        });
                    }

                    break;
            }
        }

        /// <summary>
        ///     npinfo packet
        /// </summary>
        /// <param name="npinfoPacket"></param>
        public void GetStats(NpinfoPacket npinfoPacket)
        {
            List<string> scnPackets = Session.Character.GenerateScN();
            //Session.SendPacket(Session.Character.GenerateStatChar());
            if (npinfoPacket.Page == Session.Character.ScPage)
            {
                Session.Character.ScPage = npinfoPacket.Page;
                Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
                Session.SendPackets(Session.Character.GenerateScP(npinfoPacket.Page));
                foreach (string packet in scnPackets)
                {
                    Session.SendPacket(packet);
                }

                return;
            }

            Session.Character.ScPage = npinfoPacket.Page;
            Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());
            Session.SendPackets(Session.Character.GenerateScP(npinfoPacket.Page));
            foreach (string packet in scnPackets)
            {
                Session.SendPacket(packet);
            }
        }

        /// <summary>
        ///     btk packet
        /// </summary>
        /// <param name="btkPacket"></param>
        public void FriendTalk(BtkPacket btkPacket)
        {
            if (string.IsNullOrEmpty(btkPacket.Message))
            {
                return;
            }

            string message = btkPacket.Message;
            if (message.Length > 60)
            {
                message = message.Substring(0, 60);
            }

            message = message.Trim();

            CharacterDTO character = DaoFactory.Instance.CharacterDao.LoadById(btkPacket.CharacterId);
            if (character == null)
            {
                return;
            }

            //session is not on current server, check api if the target character is on another server
            try
            {
                int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = character.CharacterId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = PacketFactory.Serialize(Session.Character.GenerateTalk(message)),
                    Type = MessageType.PrivateChat
                });
                if (!sentChannelId.HasValue) //character is even offline on different world
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_OFFLINE")));
                }
                else
                {
                    LogHelper.Instance.InsertChatLog(ChatType.Friend, Session.Character.CharacterId, message, Session.IpAddress);
                }
            }
            catch (Exception)
            {
                // FDP
            }
        }

        /// <summary>
        ///     fdel packet
        /// </summary>
        /// <param name="fDelPacket"></param>
        public void FriendDelete(FDelPacket fDelPacket)
        {
            Session.Character.DeleteRelation(fDelPacket.CharacterId);
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("FRIEND_DELETED")));
        }

        /// <summary>
        ///     fins packet
        /// </summary>
        /// <param name="fInsPacket"></param>
        public void FriendAdd(FInsPacket fInsPacket)
        {
            if (!Session.Character.IsFriendlistFull())
            {
                long characterId = fInsPacket.CharacterId;
                if (!Session.Character.IsFriendOfCharacter(characterId))
                {
                    if (!Session.Character.IsBlockedByCharacter(characterId))
                    {
                        if (!Session.Character.IsBlockingCharacter(characterId))
                        {
                            ClientSession otherSession = ServerManager.Instance.GetSessionByCharacterId(characterId);
                            if (otherSession == null)
                            {
                                return;
                            }

                            if (otherSession.Character.FriendRequestBlocked)
                            {
                                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_REQUEST_BLOCKED")}");
                                return;
                            }

                            if (otherSession.Character.FriendRequestCharacters.Contains(Session.Character.CharacterId))
                            {
                                switch (fInsPacket.Type)
                                {
                                    case 1:
                                        Session.Character.AddRelation(characterId, CharacterRelationType.Friend);
                                        Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                        otherSession.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_ADDED")}");
                                        break;

                                    case 2:
                                        otherSession.SendPacket(Language.Instance.GetMessageFromKey("FRIEND_REJECTED"));
                                        break;

                                    default:
                                        if (Session.Character.IsFriendlistFull())
                                        {
                                            Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                            otherSession.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                otherSession.SendPacket(UserInterfaceHelper.Instance.GenerateDialog(
                                    $"#fins^1^{Session.Character.CharacterId} #fins^2^{Session.Character.CharacterId} {string.Format(Language.Instance.GetMessageFromKey("FRIEND_ADD"), Session.Character.Name)}"));
                                Session.Character.FriendRequestCharacters.Add(characterId);
                            }
                        }
                        else
                        {
                            Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKING")}");
                        }
                    }
                    else
                    {
                        Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")}");
                    }
                }
                else
                {
                    Session.SendPacket($"info {Language.Instance.GetMessageFromKey("ALREADY_FRIEND")}");
                }
            }
            else
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("FRIEND_FULL")}");
            }
        }

        /// <summary>
        ///     bldel packet
        /// </summary>
        /// <param name="blDelPacket"></param>
        public void BlacklistDelete(BlDelPacket blDelPacket)
        {
            Session.Character.DeleteBlackList(blDelPacket.CharacterId);
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_DELETED")));
        }

        /// <summary>
        ///     blins packet
        /// </summary>
        /// <param name="blInsPacket"></param>
        public void BlacklistAdd(BlInsPacket blInsPacket)
        {
            Session.Character.AddRelation(blInsPacket.CharacterId, CharacterRelationType.Blocked);
            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_ADDED")));
            Session.SendPacket(Session.Character.GenerateBlinit());
        }

        /// <summary>
        ///     hero packet
        /// </summary>
        /// <param name="heroPacket"></param>
        public void Hero(HeroPacket heroPacket)
        {
            if (string.IsNullOrEmpty(heroPacket.Message))
            {
                return;
            }

            if (Session.Character.IsReputHero() >= 3)
            {
                heroPacket.Message = heroPacket.Message.Trim();
                ServerManager.Instance.Broadcast(Session, $"msg 5 [{Session.Character.Name}]:{heroPacket.Message}", ReceiverType.AllNoHeroBlocked);
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_HERO"), 11));
            }
        }

        /// <summary>
        ///     RstartPacket packet
        /// </summary>
        /// <param name="rStartPacket"></param>
        public void GetRStart(RStartPacket rStartPacket)
        {
            if (rStartPacket.Type != 1)
            {
                return;
            }

            Session.CurrentMapInstance.InstanceBag.Lock = true;
            Preq(new PreqPacket());
        }

        /// <summary>
        ///     PreqPacket packet
        /// </summary>
        /// <param name="packet"></param>
        public void Preq(PreqPacket packet)
        {
            double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
            double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
            if (!(timeSpanSinceLastPortal >= 4) && Session.CurrentMapInstance?.MapInstanceType != MapInstanceType.RaidInstance || !Session.HasCurrentMapInstance)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_MOVE"), 10));
                return;
            }

            Parallel.ForEach(Session.CurrentMapInstance.Portals.Concat(Session.Character.GetExtraPortal()), portal =>
            {
                if (Session.Character.PositionY < portal.SourceY - 1 || Session.Character.PositionY > portal.SourceY + 1 || Session.Character.PositionX < portal.SourceX - 1 ||
                    Session.Character.PositionX > portal.SourceX + 1)
                {
                    return;
                }

                switch (portal.Type)
                {
                    case (sbyte)PortalType.MapPortal:
                    case (sbyte)PortalType.TSNormal:
                    case (sbyte)PortalType.Open:
                    case (sbyte)PortalType.Miniland:
                    case (sbyte)PortalType.TSEnd:
                    case (sbyte)PortalType.Exit:
                    case (sbyte)PortalType.Effect:
                    case (sbyte)PortalType.ShopTeleport:
                        break;
                    case (sbyte)PortalType.Raid:
                        if (Session.Character.MapInstance.MapInstanceType == MapInstanceType.RaidInstance)
                        {
                            break;
                        }

                        if (Session.Character.Group?.Raid != null)
                        {
                            Session.SendPacket(Session.Character.Group.IsLeader(Session)
                                ? $"qna #mkraid^0^275 {Language.Instance.GetMessageFromKey("DO_YOU_WANT_RAID")}"
                                : Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ONLY_TEAM_LEADER_CAN_START"), 10));
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NEED_TEAM"), 1));
                        }

                        return;
                    case (sbyte)PortalType.BlueRaid:
                    case (sbyte)PortalType.DarkRaid:
                        if (Session.Character.Family == null)
                        {
                            break;
                        }

                        ScriptedInstance raid = Session.Character.Family.Act4Raid;
                        if ((byte)Session.Character.Faction == (portal.Type - 9) && raid?.FirstMap != null)
                        {
                            if (raid.LevelMinimum > Session.Character.Level)
                            {
                                Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LOW_RAID_LEVEL"), raid.LevelMinimum), 10));
                                return;
                            }

                            Session.Character.LastPortal = currentRunningSeconds;
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, raid.FirstMap.MapInstanceId, raid.StartX, raid.StartY);
                        }
                        else
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                        }

                        return;
                    default:
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                        return;
                }

                switch (Session.CurrentMapInstance.MapInstanceType)
                {
                    case MapInstanceType.TimeSpaceInstance when !Session.CurrentMapInstance.InstanceBag.Lock:
                        if (Session.Character.CharacterId == Session.CurrentMapInstance.InstanceBag.Creator)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateDialog($"#rstart^1 rstart {Language.Instance.GetMessageFromKey("ASK_ENTRY_IN_FIRST_ROOM")}"));
                        }

                        return;
                    case MapInstanceType.RaidInstance:
                        ClientSession leader = Session?.Character?.Group?.Characters?.OrderBy(s => s.Character.LastGroupJoin).ElementAt(0);
                        if (leader != null && Session.Character.CharacterId != leader.Character.CharacterId && leader.CurrentMapInstance.MapInstanceId != portal.DestinationMapInstanceId &&
                            ServerManager.Instance.GetMapInstance(portal.DestinationMapInstanceId).Monsters.Any(m => m.IsBoss))
                        {
                            ServerManager.Instance.ChangeMapInstance(leader.Character.CharacterId, portal.DestinationMapInstanceId, portal.DestinationX, portal.DestinationY);
                        }
                        //Yes, that's fucking disgusting.
                        else if (Session.Character?.Family?.Act4Raid?.FirstMap == Session.CurrentMapInstance && Session.Character?.Family?.Act4Raid?.StartX == portal.SourceX
                            && Session?.Character?.Family?.Act4Raid?.StartY == portal.SourceY)
                        {
                            ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, ServerManager.Instance.Act4Maps.First(m => m.Map.MapId == 134).MapInstanceId,
                                142, 100);
                            return;
                        }

                        break;
                    case MapInstanceType.ArenaInstance:
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                        return;

                    case MapInstanceType.Act4Instance:
                        switch (Session.Character.Faction)
                        {
                            case FactionType.Angel:
                                if (portal.DestinationMapId == 131)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                                    return;
                                }
                                else if (portal.DestinationMapId == 153)
                                {
                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, portal.DestinationMapInstanceId, 46, 171);
                                    return;
                                }

                                break;
                            case FactionType.Demon:
                                if (portal.DestinationMapId == 130)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PORTAL_BLOCKED"), 10));
                                    return;
                                }
                                else if (portal.DestinationMapId == 153)
                                {
                                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, portal.DestinationMapInstanceId, 135, 171);
                                    return;
                                }

                                break;
                        }

                        break;
                }

                portal.OnTraversalEvents.ForEach(e => { EventHelper.Instance.RunEvent(e); });
                if (portal.DestinationMapInstanceId == default)
                {
                    return;
                }

                Session.SendPacket(Session.CurrentMapInstance.GenerateRsfn());

                Session.Character.LastPortal = currentRunningSeconds;

                if (ServerManager.Instance.GetMapInstance(portal.SourceMapInstanceId).MapInstanceType != MapInstanceType.BaseMapInstance &&
                    ServerManager.Instance.GetMapInstance(portal.DestinationMapInstanceId).MapInstanceType == MapInstanceType.BaseMapInstance)
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId, Session.Character.MapId, Session.Character.MapX, Session.Character.MapY);
                }
                else if (portal.DestinationMapInstanceId == Session.Character.Miniland.MapInstanceId)
                {
                    ServerManager.Instance.JoinMiniland(Session, Session);
                }
                else if (portal.DestinationMapId == (short)SpecialMapIdType.Lobby)
                {
                    ServerManager.Instance.TeleportToLobby(Session);
                }
                else
                {
                    if (portal.DestinationX == -1 && portal.DestinationY == -1)
                    {
                        ServerManager.Instance.TeleportOnRandomPlaceInMap(Session, portal.DestinationMapInstanceId);
                        return;
                    }

                    if (portal.DestinationMapInstanceId == Session.Character.MapInstanceId)
                    {
                        Session.Character.TeleportOnMap(portal.DestinationX, portal.DestinationY);
                        return;
                    }

                    ServerManager.Instance.ChangeMapInstance(Session.Character.CharacterId, portal.DestinationMapInstanceId, portal.DestinationX, portal.DestinationY);
                }
            });
        }

        /// <summary>
        ///     pulse packet
        /// </summary>
        /// <param name="pulsepacket"></param>
        public void Pulse(PulsePacket pulsepacket)
        {
            Session.Character.LastPulse += 60;
            if (pulsepacket.Tick != Session.Character.LastPulse)
            {
                Session.Disconnect();
            }

            Session.Character.DeleteTimeout();

            try
            {
                CommunicationServiceClient.Instance.PulseAccount(Session.Account.AccountId);
            }
            catch
            {
                Session.Disconnect();
            }
        }

        public void QtPacket(QtPacket qtPacket)
        {
            switch (qtPacket.Type)
            {
                // On Target Dest
                case 1:
                    Session.Character.IncrementQuests(QuestType.GoTo, Session.CurrentMapInstance.Map.MapId, Session.Character.PositionX, Session.Character.PositionY);
                    break;

                // Give Up Quest
                case 3:
                    CharacterQuest charQuest = Session.Character.Quests?.FirstOrDefault(q => q.QuestNumber == qtPacket.Data);
                    if (charQuest == null || charQuest.IsMainQuest)
                    {
                        return;
                    }

                    Session.Character.RemoveQuest(charQuest.QuestId, true);
                    break;

                // Ask for rewards
                case 4:
                    break;
            }
        }

        /// <summary>
        ///     req_info packet
        /// </summary>
        /// <param name="reqInfoPacket"></param>
        public void ReqInfo(ReqInfoPacket reqInfoPacket)
        {
            switch (reqInfoPacket.Type)
            {
                case 12:
                    //For some reason, that seems to work for every Item ... Wtf Entwell
                    Session.SendPacket("r_info 170 2");
                    break;
                case 6:
                    if (reqInfoPacket.MateVNum.HasValue)
                    {
                        Mate mate = Session.CurrentMapInstance.Sessions.FirstOrDefault(s => s.Character?.Mates != null && s.Character.Mates.Any(o => o.MateTransportId == reqInfoPacket.MateVNum.Value))
                            ?.Character.Mates.Find(o => o.MateTransportId == reqInfoPacket.MateVNum.Value);
                        Session.SendPacket(mate?.GenerateEInfo());
                    }

                    break;
                case 5:
                    NpcMonster npc = ServerManager.Instance.GetNpc((short)reqInfoPacket.TargetVNum);
                    if (npc != null)
                    {
                        Session.SendPacket(npc.GenerateEInfo());
                    }

                    break;
                default:
                    Session.SendPacket(ServerManager.Instance.GetSessionByCharacterId(reqInfoPacket.TargetVNum)?.Character?.GenerateReqInfo());
                    break;
            }
        }

        /// <summary>
        ///     rest packet
        /// </summary>
        /// <param name="sitpacket"></param>
        public void Rest(SitPacket sitpacket)
        {
            if (Session.Character.MeditationDictionary.Count != 0)
            {
                Session.Character.MeditationDictionary.Clear();
            }

            sitpacket?.Users?.ForEach(u =>
            {
                if (u.UserType == UserType.Player)
                {
                    Session.Character.Rest();
                }
                else
                {
                    Session.CurrentMapInstance.Broadcast(Session.Character.Mates.FirstOrDefault(s => s.MateTransportId == (int)u.UserId && u.UserType != UserType.Player)?.GenerateRest());
                }
            });
        }

        /// <summary>
        ///     revival packet
        /// </summary>
        /// <param name="revivalPacket"></param>
        public void Revive(RevivalPacket revivalPacket)
        {
            if (Session.Character.Hp > 0)
            {
                return;
            }

            switch (revivalPacket.Type)
            {
                case 0:
                    switch (Session.CurrentMapInstance.MapInstanceType)
                    {
                        case MapInstanceType.LodInstance:
                            const int saver = 1211;
                            if (Session.Character.Inventory.CountItem(saver) < 1)
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_SAVER"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                            }
                            else
                            {
                                Session.Character.Inventory.RemoveItemAmount(saver);
                                Session.Character.Hp = (int)Session.Character.HpLoad();
                                Session.Character.Mp = (int)Session.Character.MpLoad();
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }

                            break;
                        default:
                            const int seed = 1012;
                            if (Session.Character.Inventory.CountItem(seed) < 10 && Session.Character.Level > 20)
                            {
                                Session.SendPacket(UserInterfaceHelper.Instance.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_POWER_SEED"), 0));
                                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_SEED_SAY"), 0));
                            }
                            else
                            {
                                if (Session.Character.Level > 20)
                                {
                                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("SEED_USED"), 10), 10));
                                    Session.Character.Inventory.RemoveItemAmount(seed, 10);
                                    Session.Character.Hp = (int)(Session.Character.HpLoad() / 2);
                                    Session.Character.Mp = (int)(Session.Character.MpLoad() / 2);
                                }
                                else
                                {
                                    Session.Character.Hp = (int)Session.Character.HpLoad();
                                    Session.Character.Mp = (int)Session.Character.MpLoad();
                                }

                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                                Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                                Session.SendPacket(Session.Character.GenerateStat());
                            }

                            break;
                    }

                    break;

                case 1:
                    ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    break;

                case 2:
                    if (Session.Character.Gold >= 100)
                    {
                        Session.Character.Hp = (int)Session.Character.HpLoad();
                        Session.Character.Mp = (int)Session.Character.MpLoad();
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateTp());
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateRevive());
                        Session.SendPacket(Session.Character.GenerateStat());
                        Session.Character.Gold -= 100;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.Character.LastPvpRevive = DateTime.Now;
                    }
                    else
                    {
                        ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
                    }

                    break;
            }
        }

        /// <summary>
        ///     say packet
        /// </summary>
        /// <param name="sayPacket"></param>
        public void Say(SayPacket sayPacket)
        {
            if (string.IsNullOrEmpty(sayPacket.Message) || Session?.CurrentMapInstance == null || Session.CurrentMapInstance.IsMute && Session.Character?.Authority < AuthorityType.GameMaster)
            {
                return;
            }

            PenaltyLogDTO penalty = Session.Account.PenaltyLogs.OrderByDescending(s => s.DateEnd).FirstOrDefault();
            string message = sayPacket.Message;
            if (Session.Character.IsMuted() && penalty != null)
            {
                if (Session.Character.Gender == GenderType.Female)
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Where(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2)
                            .Where(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o =>
                                o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_FEMALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
                else
                {
                    ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Where(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2)
                            .Where(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList().ForEach(o =>
                                o.Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MUTED_MALE"), 1));
                    }

                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 11));
                    Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("MUTE_TIME"), (penalty.DateEnd - DateTime.Now).ToString(@"hh\:mm\:ss")), 12));
                }
            }
            else
            {
                LogHelper.Instance.InsertChatLog(ChatType.General, Session.Character.CharacterId, message, Session.IpAddress);
                byte type = 0;
                ConcurrentBag<ArenaTeamMember> member = ServerManager.Instance.ArenaTeams.FirstOrDefault(s => s.Any(e => e.Session == Session));
                if (Session.Character.Authority == AuthorityType.Moderator)
                {
                    type = 12;
                    if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                    {
                        ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                        member.Where(s => member2 != null && s.ArenaTeamType == member2.ArenaTeamType && s != member2)
                            .Where(s => s.ArenaTeamType == member.FirstOrDefault(o => o.Session == Session)?.ArenaTeamType).ToList()
                            .ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), 1)));
                    }
                    else
                    {
                        Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), 1), ReceiverType.AllExceptMe);
                    }

                    message = $"[{Language.Instance.GetMessageFromKey("SUPPORT")} {Session.Character.Name}]: " + message;
                }

                if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance && member != null)
                {
                    ArenaTeamMember member2 = member.FirstOrDefault(o => o.Session == Session);
                    member.Where(s => s.ArenaTeamType == member2?.ArenaTeamType && s != member2).ToList().ForEach(o => o.Session.SendPacket(Session.Character.GenerateSay(message.Trim(), type)));
                }
                else if (Session.CurrentMapInstance.MapInstanceType == MapInstanceType.Act4Instance || Session.CurrentMapInstance.MapInstanceType == MapInstanceType.CaligorInstance)
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), type), ReceiverType.AllExceptMeAct4);
                }
                else
                {
                    Session.CurrentMapInstance?.Broadcast(Session, Session.Character.GenerateSay(message.Trim(), type), ReceiverType.AllExceptMe);
                }
            }
        }

        /// <summary>
        ///     pst packet
        /// </summary>
        /// <param name="pstpacket"></param>
        public void SendMail(PstPacket pstpacket)
        {
            if (pstpacket.Data != null)
            {
                CharacterDTO receiver = DaoFactory.Instance.CharacterDao.LoadByName(pstpacket.Receiver);
                if (receiver != null)
                {
                    string[] datasplit = pstpacket.Data.Split(' ');
                    var headWearable = Session.Character.Inventory.LoadBySlotAndType<WearableInstance>((byte)EquipmentType.Hat, InventoryType.Wear);
                    byte color = headWearable != null && headWearable.Item.IsColored ? (byte)headWearable.Design : (byte)Session.Character.HairColor;
                    var mailcopy = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = true,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0 ? (short)-1 : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };
                    var mail = new MailDTO
                    {
                        AttachmentAmount = 0,
                        IsOpened = false,
                        Date = DateTime.Now,
                        Title = datasplit[0],
                        Message = datasplit[1],
                        ReceiverId = receiver.CharacterId,
                        SenderId = Session.Character.CharacterId,
                        IsSenderCopy = false,
                        SenderClass = Session.Character.Class,
                        SenderGender = Session.Character.Gender,
                        SenderHairColor = Enum.IsDefined(typeof(HairColorType), color) ? (HairColorType)color : 0,
                        SenderHairStyle = Session.Character.HairStyle,
                        EqPacket = Session.Character.GenerateEqListForPacket(),
                        SenderMorphId = Session.Character.Morph == 0 ? (short)-1 : (short)(Session.Character.Morph > short.MaxValue ? 0 : Session.Character.Morph)
                    };

                    DaoFactory.Instance.MailDao.InsertOrUpdate(ref mail);
                    CommunicationServiceClient.Instance.SendMail(ServerManager.Instance.ServerGroup, mail);

                    Session.Character.MailList.Add((Session.Character.MailList.Any() ? Session.Character.MailList.OrderBy(s => s.Key).Last().Key : 0) + 1, mailcopy);
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAILED"), 11));
                    Session.SendPacket(Session.Character.GeneratePost(mailcopy, 2));
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
                }
            }

            if (pstpacket.Unknow1.HasValue)
            {
                return;
            }

            {
                if (!int.TryParse(pstpacket.Id.ToString(), out int id) || !byte.TryParse(pstpacket.Type.ToString(), out byte type))
                {
                    return;
                }

                switch (pstpacket.Argument)
                {
                    case 3:
                        if (Session.Character.MailList.ContainsKey(id))
                        {
                            if (!Session.Character.MailList[id].IsOpened)
                            {
                                Session.Character.MailList[id].IsOpened = true;
                            }

                            Session.SendPacket(Session.Character.GeneratePostMessage(Session.Character.MailList[id], type));
                        }

                        break;
                    case 2:
                        if (Session.Character.MailList.ContainsKey(id))
                        {
                            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("MAIL_DELETED"), 11));
                            Session.SendPacket($"post 2 {type} {id}");

                            if (Session.Character.MailList.ContainsKey(id))
                            {
                                Session.Character.MailList.Remove(id);
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        ///     qset packet
        /// </summary>
        /// <param name="qSetPacket"></param>
        public void SetQuicklist(QSetPacket qSetPacket)
        {
            short data1 = 0, data2 = 0, type = qSetPacket.Type, q1 = qSetPacket.Q1, q2 = qSetPacket.Q2;
            if (qSetPacket.Data1.HasValue)
            {
                data1 = qSetPacket.Data1.Value;
            }

            if (qSetPacket.Data2.HasValue)
            {
                data2 = qSetPacket.Data2.Value;
            }

            switch (type)
            {
                case 0:
                case 1:

                    // client says qset 0 1 3 2 6 answer -> qset 1 3 0.2.6.0
                    Session.Character.QuicklistEntries.RemoveAll(n => n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));

                    Session.Character.QuicklistEntries.Add(new QuicklistEntryDTO
                    {
                        CharacterId = Session.Character.CharacterId,
                        Type = type,
                        Q1 = q1,
                        Q2 = q2,
                        Slot = data1,
                        Pos = data2,
                        Morph = Session.Character.UseSp ? (short)Session.Character.Morph : (short)0
                    });

                    Session.SendPacket($"qset {q1} {q2} {type}.{data1}.{data2}.0");
                    break;

                case 2:

                    // DragDrop / Reorder qset type to1 to2 from1 from2 vars -> q1 q2 data1 data2
                    QuicklistEntryDTO qlFrom =
                        Session.Character.QuicklistEntries.FirstOrDefault(n => n.Q1 == data1 && n.Q2 == data2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));

                    if (qlFrom != null)
                    {
                        QuicklistEntryDTO qlTo =
                            Session.Character.QuicklistEntries.FirstOrDefault(n => n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));

                        qlFrom.Q1 = q1;
                        qlFrom.Q2 = q2;

                        if (qlTo == null)
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket($"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // old 'from' is now empty.
                            Session.SendPacket($"qset {data1} {data2} 7.7.-1.0");
                        }
                        else
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket($"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // 'from' is now 'to' because they exchanged
                            qlTo.Q1 = data1;
                            qlTo.Q2 = data2;
                            Session.SendPacket($"qset {qlTo.Q1} {qlTo.Q2} {qlTo.Type}.{qlTo.Slot}.{qlTo.Pos}.0");
                        }
                    }

                    break;

                case 3:

                    // Remove from Quicklist
                    Session.Character.QuicklistEntries.RemoveAll(n => n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp ? n.Morph == Session.Character.Morph : n.Morph == 0));
                    Session.SendPacket($"qset {q1} {q2} 7.7.-1.0");
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        ///     game_start packet
        /// </summary>
        /// <param name="gameStartPacket"></param>
        public void StartGame(GameStartPacket gameStartPacket)
        {
            if (Session.IsOnMap || !Session.HasSelectedCharacter)
            {
                // character should have been selected in SelectCharacter
                return;
            }

            Session.CurrentMapInstance = Session.Character.MapInstance;

            Session.Character.SaveObs = Observable.Interval(TimeSpan.FromMinutes(5)).Subscribe(s =>
            {
                if (Session?.IsConnected == true)
                {
                    Session.Character.Save();
                }
            });

            if (Session.Account.Authority < AuthorityType.GameMaster && ServerManager.Instance.AntiBotEnabled)
            {
                Session.Character.AntiBotCount = ServerManager.Instance.MaxCodeAttempts;
                Observable.Interval(ServerManager.Instance.AutoKickInterval).Subscribe(s =>
                {
                    Session.Character.AntiBotIdentificator = (short)ServerManager.Instance.RandomNumber(1000, 10000);
                    Session.SendPacket($"evnt 3 0 {ServerManager.Instance.TimeBeforeAutoKick.Minutes} {ServerManager.Instance.TimeBeforeAutoKick.Minutes}");
                    Session.SendPacket($"say 1 0 10 Entrez $Bot {Session.Character.AntiBotIdentificator} pour continuer a jouer.");
                    Session.SendPacket($"msg 2 Entrez $Bot {Session.Character.AntiBotIdentificator} pour continuer a jouer.");

                    Session.Character.AntiBotMessageInterval = Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(t =>
                    {
                        Session.SendPacket($"say 1 0 10 Entrez $Bot {Session.Character.AntiBotIdentificator} pour continuer a jouer.");
                        Session.SendPacket($"msg 2 Entrez $Bot {Session.Character.AntiBotIdentificator} pour continuer a jouer.");
                    });

                    Session.Character.AntiBotObservable = Observable.Timer(ServerManager.Instance.TimeBeforeAutoKick).Subscribe(v =>
                    {
                        LogHelper.Instance.InsertAntiBotLog(Session, true);
                        Session.Character.AntiBotMessageInterval?.Dispose();
                        Session?.Disconnect();
                        CommunicationServiceClient.Instance.KickSession(Session.Account.AccountId, Session.SessionId);
                    });
                });
            }

            if (false && Session.Character.GeneralLogs.Count(s => s.LogType == "Connection") < 2)
            {
                Session.SendPacket("scene 40");
            }

            if (ServerManager.Instance.MessageOfTheDay)
            {
                Session.SendPacket(Session.Character.GenerateSay("--------------[NosWings : Reborn]--------------", 10));
                Session.SendPacket(Session.Character.GenerateSay($"Xp : {ServerManager.Instance.XpRate}", 11));
                Session.SendPacket(Session.Character.GenerateSay($"Drop : {ServerManager.Instance.DropRate}", 11));
                Session.SendPacket(Session.Character.GenerateSay($"Gold : {ServerManager.Instance.GoldRate}", 11));
                Session.SendPacket(Session.Character.GenerateSay($"Fairy : {ServerManager.Instance.FairyXpRate}", 11));
                Session.SendPacket(Session.Character.GenerateSay("Website : https://noswings.com/", 11));
                Session.SendPacket(Session.Character.GenerateSay("-----------------------------------------------", 10));
            }

            Session.Character.LoadSpeed();
            Session.Character.LoadSkills();
            Session.Character.LoadPassive();
            Session.SendPacket(Session.Character.GenerateTit());
            Session.SendPacket(Session.Character.GenerateSpPoint());
            Session.SendPacket("rsfi 1 1 0 9 0 9");
            Session.Character.Quests?.Where(q => q?.Quest?.TargetMap != null).ToList().ForEach(qst => Session.SendPacket(qst.Quest.TargetPacket()));
            if (Session.Character.Hp <= 0)
            {
                ServerManager.Instance.ReviveFirstPosition(Session.Character.CharacterId);
            }
            else
            {
                if (Session.Character.MapId == (short)SpecialMapIdType.Lobby)
                {
                    ServerManager.Instance.TeleportToLobby(Session);
                }
                else
                {
                    ServerManager.Instance.ChangeMap(Session.Character.CharacterId);
                }
            }

            Session.SendPacket(Session.Character.GenerateSki());
            Session.SendPacket($"fd {Session.Character.Reput} 0 {(int)Session.Character.Dignity} {Math.Abs(Session.Character.GetDignityIco())}");
            Session.SendPacket(Session.Character.GenerateFd());
            Session.SendPacket("rage 0 250000");
            Session.SendPacket("rank_cool 0 0 18000");
            var specialistInstance = Session.Character.Inventory.LoadBySlotAndType<SpecialistInstance>(8, InventoryType.Wear);
            StaticBonusDTO medal =
                Session.Character.StaticBonusList.FirstOrDefault(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);
            if (medal != null)
            {
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("LOGIN_MEDAL"), 12));
            }

            if (Session.Character.StaticBonusList.Any(s => s.StaticBonusType == StaticBonusType.PetBasket))
            {
                Session.SendPacket("ib 1278 1");
            }

            if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.CleftOfDarkness))
            {
                Session.SendPacket("bc 0 0 0");
            }

            if (specialistInstance != null)
            {
                Session.SendPacket(Session.Character.GenerateSpPoint());
            }

            Session.SendPacket("scr 0 0 0 0 0 0");
            for (int i = 0; i < 10; i++)
            {
                Session.SendPacket($"bn {i} {Language.Instance.GetMessageFromKey($"BN{i}")}");
            }

            Session.SendPacket(Session.Character.GenerateExts());
            Session.SendPacket(Session.Character.GenerateMlinfo());
            Session.SendPacket(UserInterfaceHelper.Instance.GeneratePClear());

            Session.SendPacket(Session.Character.GeneratePinit());
            Session.SendPackets(Session.Character.Mates.Where(s => s.IsTeamMember)
                .OrderBy(s => s.MateType)
                .Select(s => s.GeneratePst()));

            Session.SendPacket("zzim");
            Session.SendPacket($"twk 2 {Session.Character.CharacterId} {Session.Account.Name} {Session.Character.Name} shtmxpdlfeoqkr");

            // qstlist target sqst bf
            Session.SendPacket("act6");
            Session.SendPacket(Session.Character.GenerateFaction());
            // MATES
            Session.SendPackets(Session.Character.GenerateScP());
            Session.SendPackets(Session.Character.GenerateScN());
#pragma warning disable 618
            Session.Character.GenerateStartupInventory();
#pragma warning restore 618

            Session.SendPacket(Session.Character.GenerateGold());
            Session.SendPackets(Session.Character.GenerateQuicklist());

            string clinit = ServerManager.Instance.TopComplimented.Aggregate("clinit",
                (current, character) => current + $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Compliment}|{character.Name}");
            string flinit = ServerManager.Instance.TopReputation.Aggregate("flinit",
                (current, character) => current + $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Reput}|{character.Name}");
            string kdlinit = ServerManager.Instance.TopPoints.Aggregate("kdlinit",
                (current, character) => current + $" {character.CharacterId}|{character.Level}|{character.HeroLevel}|{character.Act4Points}|{character.Name}");

            Session.SendPacket(Session.Character.GenerateFinit());
            Session.SendPacket(Session.Character.GenerateBlinit());
            Session.SendPacket(clinit);
            Session.SendPacket(flinit);
            Session.SendPacket(kdlinit);

            Session.Character.LastPvpRevive = DateTime.Now;

            long? familyId = DaoFactory.Instance.FamilyCharacterDao.LoadByCharacterId(Session.Character.CharacterId)?.FamilyId;
            if (familyId != null)
            {
                Session.Character.Family = ServerManager.Instance.FamilyList.FirstOrDefault(s => s.Value.FamilyId == familyId.Value).Value;
            }

            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null)
            {
                // TODO IMPROVE THAT
                if (Session.Character.Family.FamilyFaction != (byte)Session.Character.Faction)
                {
                    Session.Character.ChangeFaction((FactionType)Session.Character.Family.FamilyFaction);
                }

                Session.SendPacket(Session.Character.GenerateGInfo());
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
                Session.SendPackets(Session.Character.GetFamilyHistory());
                Session.SendPacket(Session.Character.GenerateFamilyMember());
                Session.SendPacket(Session.Character.GenerateFamilyMemberMessage());
                Session.SendPacket(Session.Character.GenerateFamilyMemberExp());
                if (!string.IsNullOrWhiteSpace(Session.Character.Family.FamilyMessage))
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo("--- Family Message ---\n" + Session.Character.Family.FamilyMessage));
                }
            }

            IEnumerable<PenaltyLogDTO> warning = DaoFactory.Instance.PenaltyLogDao.LoadByAccount(Session.Character.AccountId).Where(p => p.Penalty == PenaltyType.Warning);
            IEnumerable<PenaltyLogDTO> penaltyLogDtos = warning as IList<PenaltyLogDTO> ?? warning.ToList();
            if (penaltyLogDtos.Any())
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(string.Format(Language.Instance.GetMessageFromKey("WARNING_INFO"), penaltyLogDtos.Count())));
            }

            // finfo - friends info
            List<MailDTO> mails = DaoFactory.Instance.MailDao.LoadByCharacterId(Session.Character.CharacterId).ToList();
            foreach (MailDTO mail in mails)
            {
                Session.Character.GenerateMail(mail);
            }

            int giftcount = mails.Count(mail => !mail.IsSenderCopy && mail.ReceiverId == Session.Character.CharacterId && mail.AttachmentVNum != null && !mail.IsOpened);
            int mailcount = mails.Count(mail => !mail.IsSenderCopy && mail.ReceiverId == Session.Character.CharacterId && mail.AttachmentVNum == null && !mail.IsOpened);
            if (giftcount > 0)
            {
                Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("GIFTED"), giftcount), 11));
            }

            if (mailcount > 0)
            {
                Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NEW_MAIL"), mailcount), 10));
            }

            Session.Character.DeleteTimeout();

            foreach (StaticBuffDTO sb in DaoFactory.Instance.StaticBuffDao.LoadByCharacterId(Session.Character.CharacterId))
            {
                Session.Character.AddStaticBuff(sb);
            }

            if (Session.Character.MapInstance.Map.MapTypes.Any(m => m.MapTypeId == (short)MapTypeEnum.Act4 || m.MapTypeId == (short)MapTypeEnum.Act42))
            {
                Session.Character.ConnectAct4();
            }

            if (Session.Character.Quests.Any())
            {
                Session.SendPacket(Session.Character.GenerateQuestsPacket());
            }

            Session.Character.GeneratePairy();
            Session.Character.GenerateStatChar();
            Session.SendPacket(Session.Character.GenerateSay("Use $Help to see available commands !", 12));
        }

        /// <summary>
        ///     walk packet
        /// </summary>
        /// <param name="walkPacket"></param>
        public void Walk(WalkPacket walkPacket)
        {
            if (Session.Character.HasBuff(BCardType.CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible))
            {
                return;
            }

            if (Session.Character.MeditationDictionary.Count != 0)
            {
                Session.Character.MeditationDictionary.Clear();
            }

            double currentRunningSeconds = (DateTime.Now - Process.GetCurrentProcess().StartTime.AddSeconds(-50)).TotalSeconds;
            double timeSpanSinceLastPortal = currentRunningSeconds - Session.Character.LastPortal;
            int distance = Map.GetDistance(new MapCell { X = Session.Character.PositionX, Y = Session.Character.PositionY },
                new MapCell { X = walkPacket.XCoordinate, Y = walkPacket.YCoordinate });

            if (!Session.HasCurrentMapInstance || Session.CurrentMapInstance.Map.IsBlockedZone(walkPacket.XCoordinate, walkPacket.YCoordinate) || Session.Character.IsChangingMapInstance ||
                Session.Character.HasShopOpened)
            {
                return;
            }

            if ((Session.Character.Speed >= walkPacket.Speed || Session.Character.LastSpeedChange.AddSeconds(5) > DateTime.Now) && !(distance > 60 && timeSpanSinceLastPortal > 10))
            {
                if (Session.Character.MapInstance?.MapInstanceType == MapInstanceType.BaseMapInstance || Session.Character.MapInstance?.MapInstanceType == MapInstanceType.LobbyMapInstance)
                {
                    Session.Character.MapX = walkPacket.XCoordinate;
                    Session.Character.MapY = walkPacket.YCoordinate;
                }

                Session.Character.PositionX = walkPacket.XCoordinate;
                Session.Character.PositionY = walkPacket.YCoordinate;

                if (Session.Character.LastMonsterAggro.AddSeconds(5) > DateTime.Now)
                {
                    Session.Character.UpdateBushFire();
                }

                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateMv());
                Session.SendPacket(Session.Character.GenerateCond());
                Session.Character.LastMove = DateTime.Now;

                Session.CurrentMapInstance?.OnAreaEntryEvents?.Where(s => s.InZone(Session.Character.PositionX, Session.Character.PositionY)).ToList()
                    .ForEach(e => { e.Events.ToList().ForEach(evt => EventHelper.Instance.RunEvent(evt)); });
                Session.CurrentMapInstance?.OnAreaEntryEvents?.ToList().RemoveAll(s => s.InZone(Session.Character.PositionX, Session.Character.PositionY));

                if (Session.CurrentMapInstance?.OnMoveOnMapEvents.Count <= 0)
                {
                    return;
                }

                Session.CurrentMapInstance?.OnMoveOnMapEvents?.ToList().ForEach(e => { EventHelper.Instance.RunEvent(e); });
                Session.CurrentMapInstance?.OnMoveOnMapEvents?.Clear();
            }
        }

        /// <summary>
        ///     / packet
        /// </summary>
        /// <param name="whisperPacket"></param>
        public void Whisper(WhisperPacket whisperPacket)
        {
            try
            {
                if (string.IsNullOrEmpty(whisperPacket.Message))
                {
                    return;
                }

                string characterName = whisperPacket.Message.Split(' ')[whisperPacket.Message.StartsWith("GM ") ? 1 : 0];
                if (characterName == null)
                {
                    return;
                }

                string whPrefix = "[" + Language.Instance.GetMessageFromKey("SUPPORT") + "]";
                if (characterName.StartsWith(whPrefix))
                {
                    characterName = characterName.Remove(0, whPrefix.Length);
                }

                string message = string.Empty;
                string[] packetsplit = whisperPacket.Message.Split(' ');
                for (int i = packetsplit[0] == "GM" ? 2 : 1; i < packetsplit.Length; i++)
                {
                    message += packetsplit[i] + " ";
                }

                if (message.Length > 60)
                {
                    message = message.Substring(0, 60);
                }

                message = message.Trim();

                Session.SendPacket(Session.Character.GenerateSpk(message, 5));

                CharacterDTO receiver = DaoFactory.Instance.CharacterDao.LoadByName(characterName);
                if (receiver == null)
                {
                    return;
                }

                if (receiver.CharacterId == Session.Character.CharacterId)
                {
                    return;
                }

                if (Session.Character.IsBlockedByCharacter(receiver.CharacterId))
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                    return;
                }

                int? sentChannelId = CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
                {
                    DestinationCharacterId = receiver.CharacterId,
                    SourceCharacterId = Session.Character.CharacterId,
                    SourceWorldId = ServerManager.Instance.WorldId,
                    Message = Session.Character.GenerateSpk(message, Session.Account.Authority == AuthorityType.GameMaster ? 15 : 5),
                    Type = packetsplit[0] == "GM" ? MessageType.WhisperGM : MessageType.Whisper
                });
                if (sentChannelId == null)
                {
                    Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("USER_NOT_CONNECTED")));
                }
                else
                {
                    LogHelper.Instance.InsertChatLog(ChatType.Whisper, Session.Character.CharacterId, message, Session.IpAddress);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error("Whisper failed.", e);
            }
        }

        #endregion
    }
}