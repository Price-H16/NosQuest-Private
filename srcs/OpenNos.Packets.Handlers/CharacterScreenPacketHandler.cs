// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using ChickenAPI.Core.Utils;
using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.Core.Logging;
using OpenNos.Core.Utilities;
using OpenNos.DAL;
using OpenNos.GameObject;
using OpenNos.GameObject.Character;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Quests;
using WingsEmu.Customization.NewCharCustomisation;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Character;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Packets.Enums;

namespace WingsEmu.PacketHandlers
{
    public class CharacterScreenPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CharacterScreenPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Char_NEW_JOB brawler creation packet
        /// </summary>
        /// <param name="brawlerCreatePacket"></param>
        public void CreateBrawler(BrawlerCreatePacket brawlerCreatePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            bool canCreate = DaoFactory.Instance.CharacterDao.LoadByAccount(Session.Account.AccountId).Any(character => character.Level >= 80);

            if (!canCreate)
            {
                Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo("You cannot create a Martial artist"));
                return;
            }

            long accountId = Session.Account.AccountId;
            byte slot = brawlerCreatePacket.Slot;
            string characterName = brawlerCreatePacket.Name;

            if (slot > 3 || DaoFactory.Instance.CharacterDao.LoadBySlot(accountId, slot) != null)
            {
                return;
            }

            if (characterName.Length <= 3 || characterName.Length >= 15)
            {
                return;
            }

            var rg = new Regex(@"^[\u0021-\u007E\u00A1-\u00AC\u00AE-\u00FF\u4E00-\u9FA5\u0E01-\u0E3A\u0E3F-\u0E5B\u002E]*$");
            if (rg.Matches(characterName).Count == 1)
            {
                CharacterDTO character = DaoFactory.Instance.CharacterDao.LoadByName(characterName);
                if (character == null || character.State == CharacterState.Inactive)
                {
                    CharacterDTO newCharacter = DependencyContainer.Instance.Get<BaseCharacter>().Character;
                    newCharacter.AccountId = accountId;
                    newCharacter.Class = ClassType.Wrestler;
                    newCharacter.Level = 81;
                    newCharacter.JobLevel = 1;
                    newCharacter.Gender = brawlerCreatePacket.Gender;
                    newCharacter.HairColor = brawlerCreatePacket.HairColor;
                    newCharacter.HairStyle = brawlerCreatePacket.HairStyle;
                    newCharacter.Name = characterName;
                    newCharacter.Slot = slot;
                    DaoFactory.Instance.CharacterDao.InsertOrUpdate(ref newCharacter);

                    // init quest 
                    var firstQuest = new CharacterQuestDTO { CharacterId = newCharacter.CharacterId, QuestId = 1997, IsMainQuest = true };
                    DaoFactory.Instance.CharacterQuestDao.InsertOrUpdate(firstQuest);


                    // init skills 
                    List<CharacterSkillDTO> wSkills = new List<CharacterSkillDTO>
                    {
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1525 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1526 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1527 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1528 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1529 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1530 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1531 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1532 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1533 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1534 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1535 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1536 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1537 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1538 },
                        new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 1539 }
                    };

                    DaoFactory.Instance.CharacterSkillDao.InsertOrUpdate(wSkills);

                    // init inventory 
                    var inventory = new Inventory((Character)newCharacter);
                    var startupInventory = DependencyContainer.Instance.Get<BaseInventory>();
                    if (startupInventory != null)
                    {
                        foreach (BaseInventory.StartupInventoryItem item in startupInventory.Items)
                        {
                            inventory.AddNewToInventory(item.Vnum, item.Quantity, item.InventoryType);
                        }
                    }

                    foreach (ItemInstance i in inventory.Select(s => s.Value))
                    {
                        DaoFactory.Instance.IteminstanceDao.InsertOrUpdate(i);
                    }

                    LoadCharacters(brawlerCreatePacket.OriginalContent);
                }
                else
                {
                    Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("ALREADY_TAKEN")}");
                }
            }
            else
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("INVALID_CHARNAME")}");
            }
        }

        /// <summary>
        ///     gbox Graphical Bank Packet
        /// </summary>
        /// <param name="gboxPacket"></param>
        public void BankAction(GboxPacket gboxPacket)
        {
            if (Session.Character.InExchangeOrTrade)
            {
                return;
            }

            switch (gboxPacket.Type)
            {
                case BankActionType.Deposit:
                    if (gboxPacket.Option == 0)
                    {
                        Session.SendPacket($"qna #gbox^1^{gboxPacket.Amount}^1 Want to deposit {gboxPacket.Amount}000 gold?");
                        return;
                    }

                    if (gboxPacket.Option == 1)
                    {
                        if (gboxPacket.Amount <= 0)
                        {
                            //Packet hacking duplication
                            return;
                        }

                        Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("BANK_DEPOSIT"), (byte)SmemoType.Information));
                        if (Session.Account.BankMoney + gboxPacket.Amount * 1000 > ServerManager.Instance.MaxBankGold)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("MAX_GOLD_BANK_REACHED")));
                            Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("MAX_GOLD_BANK_REACHED"), (byte)SmemoType.Error));
                            return;
                        }

                        if (Session.Character.Gold < gboxPacket.Amount * 1000)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD")));
                            Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD"), (byte)SmemoType.Error));
                            return;
                        }

                        Session.Account.BankMoney += gboxPacket.Amount * 1000;
                        Session.Character.Gold -= gboxPacket.Amount * 1000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateGb((byte)GoldBankPacketType.Deposit));
                        Session.SendPacket(Session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("BANK_BALANCE"), Session.Account.BankMoney, Session.Character.Gold), 1));
                        Session.SendPacket(Session.Character.GenerateSmemo(string.Format(Language.Instance.GetMessageFromKey("BANK_BALANCE"), Session.Account.BankMoney, Session.Character.Gold),
                            (byte)SmemoType.Balance));
                    }

                    break;
                case BankActionType.Withdraw:
                    if (gboxPacket.Option == 0)
                    {
                        Session.SendPacket($"qna #gbox^2^{gboxPacket.Amount}^1 Would you like to withdraw {gboxPacket.Amount}000 gold? (Fee: 0 gold)");
                        return;
                    }

                    if (gboxPacket.Option == 1)
                    {
                        if (gboxPacket.Amount <= 0)
                        {
                            //Packet hacking duplication
                            return;
                        }

                        Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("WITHDRAW_BANK"), (byte)SmemoType.Information));
                        if (Session.Character.Gold + gboxPacket.Amount * 1000 > ServerManager.Instance.MaxGold)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo(Language.Instance.GetMessageFromKey("TOO_MUCH_GOLD")));
                            Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("TOO_MUCH_GOLD"), (byte)SmemoType.Error));
                            return;
                        }

                        if (Session.Account.BankMoney < gboxPacket.Amount * 1000)
                        {
                            Session.SendPacket(UserInterfaceHelper.Instance.GenerateInfo("NOT_ENOUGH_FUNDS"));
                            Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("NOT_ENOUGH_FUNDS"), (byte)SmemoType.Error));
                            return;
                        }

                        Session.Account.BankMoney -= gboxPacket.Amount * 1000;
                        Session.Character.Gold += gboxPacket.Amount * 1000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        Session.SendPacket(Session.Character.GenerateGb((byte)GoldBankPacketType.Withdraw));
                        Session.SendPacket(Session.Character.GenerateSmemo(Language.Instance.GetMessageFromKey("BANK_BALANCE"), (byte)SmemoType.Balance));
                    }

                    break;
            }
        }

        /// <summary>
        ///     Char_NEW character creation character
        /// </summary>
        /// <param name="characterCreatePacket"></param>
        public void CreateCharacter(CharacterCreatePacket characterCreatePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            // TODO: Hold Account Information in Authorized object
            long accountId = Session.Account.AccountId;
            byte slot = characterCreatePacket.Slot;
            string characterName = characterCreatePacket.Name;
            if (slot > 3 || DaoFactory.Instance.CharacterDao.LoadBySlot(accountId, slot) != null)
            {
                return;
            }

            if (characterName.Length <= 3 || characterName.Length >= 15)
            {
                return;
            }

            var rg = new Regex(@"^[\u0021-\u007E\u00A1-\u00AC\u00AE-\u00FF\u4E00-\u9FA5\u0E01-\u0E3A\u0E3F-\u0E5B\u002E]*$");
            if (rg.Matches(characterName).Count == 1)
            {
                CharacterDTO character = DaoFactory.Instance.CharacterDao.LoadByName(characterName);
                if (character == null || character.State == CharacterState.Inactive)
                {
                    if (characterCreatePacket.Slot > 3)
                    {
                        return;
                    }

                    CharacterDTO newCharacter = DependencyContainer.Instance.Get<BaseCharacter>().Character;
                    newCharacter.AccountId = accountId;
                    newCharacter.Gender = characterCreatePacket.Gender;
                    newCharacter.HairColor = characterCreatePacket.HairColor;
                    newCharacter.HairStyle = characterCreatePacket.HairStyle;
                    newCharacter.Name = characterName;
                    newCharacter.Slot = slot;
                    newCharacter.State = CharacterState.Active;
                    DaoFactory.Instance.CharacterDao.InsertOrUpdate(ref newCharacter);

                    // init quest
                    var firstQuest = new CharacterQuestDTO { CharacterId = newCharacter.CharacterId, QuestId = 1997, IsMainQuest = true };
                    DaoFactory.Instance.CharacterQuestDao.InsertOrUpdate(firstQuest);

                    // init skills
                    var skills = DependencyContainer.Instance.Get<BaseSkill>();
                    if (skills != null)
                    {
                        foreach (CharacterSkillDTO skill in skills.Skills)
                        {
                            skill.CharacterId = newCharacter.CharacterId;
                            DaoFactory.Instance.CharacterSkillDao.InsertOrUpdate(skill);
                        }
                    }


                    // init quicklist
                    var quicklist = DependencyContainer.Instance.Get<BaseQuicklist>();

                    if (quicklist != null)
                    {
                        foreach (QuicklistEntryDTO quicklistEntry in quicklist.Quicklist)
                        {
                            quicklistEntry.CharacterId = newCharacter.CharacterId;
                            DaoFactory.Instance.QuicklistEntryDao.InsertOrUpdate(quicklistEntry);
                        }
                    }

                    // init inventory
                    var inventory = new Inventory((Character)newCharacter);
                    var startupInventory = DependencyContainer.Instance.Get<BaseInventory>();
                    if (startupInventory != null)
                    {
                        foreach (BaseInventory.StartupInventoryItem item in startupInventory.Items)
                        {
                            inventory.AddNewToInventory(item.Vnum, item.Quantity, item.InventoryType);
                        }
                    }

                    foreach (ItemInstance i in inventory.Select(s => s.Value))
                    {
                        DaoFactory.Instance.IteminstanceDao.InsertOrUpdate(i);
                    }

                    LoadCharacters(characterCreatePacket.OriginalContent);
                }
                else
                {
                    Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("ALREADY_TAKEN")}");
                }
            }
            else
            {
                Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("INVALID_CHARNAME")}");
            }
        }

        /// <summary>
        ///     Char_DEL packet
        /// </summary>
        /// <param name="characterDeletePacket"></param>
        public void DeleteCharacter(CharacterDeletePacket characterDeletePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            AccountDTO account = DaoFactory.Instance.AccountDao.LoadById(Session.Account.AccountId);
            if (account == null)
            {
                return;
            }

            if (account.Password.ToLower() == characterDeletePacket.Password.ToSha512())
            {
                CharacterDTO character = DaoFactory.Instance.CharacterDao.LoadBySlot(account.AccountId, characterDeletePacket.Slot);
                if (character == null)
                {
                    return;
                }

                DaoFactory.Instance.GeneralLogDao.SetCharIdNull(Convert.ToInt64(character.CharacterId));
                DaoFactory.Instance.CharacterDao.DeleteByPrimaryKey(account.AccountId, characterDeletePacket.Slot);

                FamilyCharacterDTO familyCharacter = DaoFactory.Instance.FamilyCharacterDao.LoadByCharacterId(character.CharacterId);
                if (familyCharacter == null)
                {
                    LoadCharacters(string.Empty);
                    return;
                }

                // REMOVE FROM FAMILY
                DaoFactory.Instance.FamilyCharacterDao.Delete(character.Name);
                ServerManager.Instance.FamilyRefresh(familyCharacter.FamilyId);
            }
            else
            {
                Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BAD_PASSWORD")}");
            }
        }

        /// <summary>
        ///     Load Characters, this is the Entrypoint for the Client, Wait for 3 Packets.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        [Packet("OpenNos.EntryPoint", 3)]
        public void LoadCharacters(string packet)
        {
            string[] loginPacketParts = packet.Split(' ');

            // Load account by given SessionId
            bool isCrossServerLogin = false;
            if (Session.Account == null)
            {
                bool hasRegisteredAccountLogin = true;
                AccountDTO account = null;
                if (loginPacketParts.Length > 4)
                {
                    if (loginPacketParts.Length > 7 && loginPacketParts[4] == "DAC" && loginPacketParts[8] == "CrossServerAuthenticate")
                    {
                        isCrossServerLogin = true;
                        account = DaoFactory.Instance.AccountDao.LoadByName(loginPacketParts[5]);
                    }
                    else
                    {
                        account = DaoFactory.Instance.AccountDao.LoadByName(loginPacketParts[4]);
                    }
                }

                try
                {
                    if (account != null)
                    {
                        hasRegisteredAccountLogin = isCrossServerLogin
                            ? CommunicationServiceClient.Instance.IsCrossServerLoginPermitted(account.AccountId, Session.SessionId)
                            : CommunicationServiceClient.Instance.IsLoginPermitted(account.AccountId, Session.SessionId);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("MS Communication Failed.", ex);
                    Session.Disconnect();
                    return;
                }

                if (loginPacketParts.Length <= 4 || !hasRegisteredAccountLogin)
                {
                    Logger.Log.WarnFormat($"Client {Session.SessionId} forced Disconnection, login has not been registered or Account is already logged in.");
                    Session.Disconnect();
                    return;
                }

                if (account == null)
                {
                    Logger.Log.WarnFormat($"Client {Session.SessionId} forced Disconnection, invalid AccountName.");
                    Session.Disconnect();
                    return;
                }

                if (!account.Password.Equals(loginPacketParts[6].ToSha512(), StringComparison.OrdinalIgnoreCase) && !isCrossServerLogin)
                {
                    Logger.Log.WarnFormat($"Client {Session.SessionId} forced Disconnection, invalid Password or SessionId.");
                    Session.Disconnect();
                    return;
                }

                PenaltyLogDTO penalty = DaoFactory.Instance.PenaltyLogDao.LoadByAccount(account.AccountId).FirstOrDefault(s => s.DateEnd > DateTime.Now && s.Penalty == PenaltyType.Banned);
                if (penalty != null)
                {
                    Session.SendPacket($"fail {string.Format(Language.Instance.GetMessageFromKey("BANNED"), penalty.Reason, penalty.DateEnd.ToString("yyyy-MM-dd-HH:mm"))}");
                    Logger.Log.Info($"{account.Name} connected from {Session.IpAddress} while being banned");
                    Session.Disconnect();
                    return;
                }

                // TODO MAINTENANCE MODE
                if (ServerManager.Instance.Sessions.Count() >= ServerManager.Instance.AccountLimit)
                {
                    if (account.Authority < AuthorityType.Moderator)
                    {
                        Session.Disconnect();
                        return;
                    }
                }

                var accountobject = new Account
                {
                    AccountId = account.AccountId,
                    Name = account.Name,
                    Password = account.Password.ToLower(),
                    Authority = account.Authority,
                    BankMoney = account.BankMoney,
                    Money = account.Money
                };

                Session.InitializeAccount(accountobject, isCrossServerLogin);
            }

            // TODO: Wrap Database access up to GO
            if (Session.Account == null)
            {
                return;
            }

            if (isCrossServerLogin)
            {
                if (!byte.TryParse(loginPacketParts[6], out byte slot))
                {
                    return;
                }

                SelectCharacter(new SelectPacket { Slot = slot });
                return;
            }

            IEnumerable<CharacterDTO> characters = DaoFactory.Instance.CharacterDao.LoadByAccount(Session.Account.AccountId);
            Logger.Log.InfoFormat(Language.Instance.GetMessageFromKey("ACCOUNT_ARRIVED"), Session.Account.Name);

            // load characterlist packet for each character in CharacterDTO
            Session.SendPacket("clist_start 0");
            foreach (CharacterDTO character in characters)
            {
                IEnumerable<ItemInstanceDTO> inventory = DaoFactory.Instance.IteminstanceDao.LoadByType(character.CharacterId, InventoryType.Wear);

                WearableInstance[] equipment = new WearableInstance[16];
                foreach (ItemInstanceDTO equipmentEntry in inventory)
                {
                    // explicit load of iteminstance
                    var currentInstance = equipmentEntry as WearableInstance;
                    equipment[(short)currentInstance.Item.EquipmentSlot] = currentInstance;
                }

                string petlist = string.Empty;
                List<MateDTO> mates = DaoFactory.Instance.MateDao.LoadByCharacterId(character.CharacterId).ToList();
                for (int i = 0; i < 26; i++)
                {
                    //0.2105.1102.319.0.632.0.333.0.318.0.317.0.9.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1
                    petlist += $"{(i != 0 ? "." : "")}{(mates.Count > i ? $"{mates.ElementAt(i).Skin}.{mates.ElementAt(i).NpcMonsterVNum}" : "-1")}";
                }

                // 1 1 before long string of -1.-1 = act completion
                Session.SendPacket(
                    $"clist {character.Slot} {character.Name} 0 {(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 {(byte)character.Class} {character.Level} {character.HeroLevel} {equipment[(byte)EquipmentType.Hat]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.Armor]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.WeaponSkin]?.ItemVNum ?? (equipment[(byte)EquipmentType.MainWeapon]?.ItemVNum ?? -1)}.{equipment[(byte)EquipmentType.SecondaryWeapon]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.Mask]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.Fairy]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.CostumeSuit]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.CostumeHat]?.ItemVNum ?? -1} {character.JobLevel}  1 1 {petlist} {(equipment[(byte)EquipmentType.Hat] != null && equipment[(byte)EquipmentType.Hat].Item.IsColored ? equipment[(byte)EquipmentType.Hat].Design : 0)} 0");
            }

            Session.SendPacket("clist_end");
        }

        /// <summary>
        ///     select packet
        /// </summary>
        /// <param name="selectPacket"></param>
        public void SelectCharacter(SelectPacket selectPacket)
        {
            try
            {
                if (Session?.Account == null || Session.HasSelectedCharacter)
                {
                    return;
                }

                if (!(DaoFactory.Instance.CharacterDao.LoadBySlot(Session.Account.AccountId, selectPacket.Slot) is Character character))
                {
                    return;
                }

                character.GeneralLogs = DaoFactory.Instance.GeneralLogDao.LoadByAccount(Session.Account.AccountId).Where(s => s.CharacterId == character.CharacterId).ToList();
                character.MapInstanceId = ServerManager.Instance.GetBaseMapInstanceIdByMapId(character.MapId);
                Map currentMap = ServerManager.Instance.GetMapInstance(character.MapInstanceId)?.Map;
                if (currentMap != null && currentMap.IsBlockedZone(character.MapX, character.MapY))
                {
                    MapCell pos = currentMap.GetRandomPosition();
                    character.PositionX = pos.X;
                    character.PositionY = pos.Y;
                }
                else
                {
                    character.PositionX = character.MapX;
                    character.PositionY = character.MapY;
                }

                character.Authority = Session.Account.Authority;
                Session.SetCharacter(character);
                if (!Session.Character.GeneralLogs.Any(s => s.Timestamp == DateTime.Now && s.LogData == "World" && s.LogType == "Connection"))
                {
                    Session.Character.SpAdditionPoint += Session.Character.SpPoint;
                    Session.Character.SpPoint = 10000;
                }

                if (Session.Character.Hp > Session.Character.HpLoad())
                {
                    Session.Character.Hp = (int)Session.Character.HpLoad();
                }

                if (Session.Character.Mp > Session.Character.MpLoad())
                {
                    Session.Character.Mp = (int)Session.Character.MpLoad();
                }

                Session.Character.Respawns = DaoFactory.Instance.RespawnDao.LoadByCharacter(Session.Character.CharacterId).ToList();
                Session.Character.StaticBonusList = DaoFactory.Instance.StaticBonusDao.LoadByCharacterId(Session.Character.CharacterId).ToList();
                Session.Character.LoadInventory();
                Session.Character.LoadQuicklists();
                Session.Character.GenerateMiniland();
                if (!DaoFactory.Instance.CharacterQuestDao.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.IsMainQuest))
                {
                    var firstQuest = new CharacterQuestDTO { CharacterId = Session.Character.CharacterId, QuestId = 1997, IsMainQuest = true };
                    DaoFactory.Instance.CharacterQuestDao.InsertOrUpdate(firstQuest);
                }

                DaoFactory.Instance.CharacterQuestDao.LoadByCharacterId(Session.Character.CharacterId).ToList().ForEach(q => Session.Character.Quests.Add(q as CharacterQuest));
                DaoFactory.Instance.MateDao.LoadByCharacterId(Session.Character.CharacterId).ToList().ForEach(s =>
                {
                    var mate = (Mate)s;
                    mate.Owner = Session.Character;
                    mate.GenerateMateTransportId();
                    mate.Monster = ServerManager.Instance.GetNpc(s.NpcMonsterVNum);
                    Session.Character.Mates.Add(mate);
                    if (!mate.IsTeamMember)
                    {
                        mate.MapX = ServerManager.Instance.MinilandRandomPos().X;
                        mate.MapY = ServerManager.Instance.MinilandRandomPos().Y;
                    }
                });
                Session.Character.Life = Observable.Interval(TimeSpan.FromMilliseconds(300)).Subscribe(x => { Session?.Character?.CharacterLife(); });
                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                {
                    AccountId = Session.Account.AccountId,
                    CharacterId = Session.Character.CharacterId,
                    IpAddress = Session.IpAddress,
                    LogData = "World",
                    LogType = "Connection",
                    Timestamp = DateTime.Now
                });

                Session.SendPacket("OK");

                // Inform everyone about connected character
                CommunicationServiceClient.Instance.ConnectCharacter(ServerManager.Instance.WorldId, character.CharacterId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Select character failed.", ex);
            }
        }

        #endregion
    }
}