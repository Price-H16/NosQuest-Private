// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.DAL;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using WingsEmu.Communication;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Families
{
    public class Family : FamilyDTO
    {
        #region Instantiation

        public Family() => FamilyCharacters = new List<FamilyCharacter>();

        public Family(FamilyDTO input)
        {
            FamilyCharacters = new List<FamilyCharacter>();
            FamilyExperience = input.FamilyExperience;
            FamilyHeadGender = input.FamilyHeadGender;
            FamilyId = input.FamilyId;
            FamilyLevel = input.FamilyLevel;
            FamilyMessage = input.FamilyMessage;
            FamilyFaction = input.FamilyFaction;
            ManagerAuthorityType = input.ManagerAuthorityType;
            ManagerCanGetHistory = input.ManagerCanGetHistory;
            ManagerCanInvite = input.ManagerCanInvite;
            ManagerCanNotice = input.ManagerCanNotice;
            ManagerCanShout = input.ManagerCanShout;
            MaxSize = input.MaxSize;
            MemberAuthorityType = input.MemberAuthorityType;
            MemberCanGetHistory = input.MemberCanGetHistory;
            Name = input.Name;
            WarehouseSize = input.WarehouseSize;
        }

        #endregion

        #region Properties

        public List<FamilyCharacter> FamilyCharacters { get; set; }

        public List<FamilyLogDTO> FamilyLogs { get; set; }

        public MapInstance LandOfDeath { get; set; }

        public ScriptedInstance Act4Raid { get; set; }

        public Inventory Warehouse { get; set; }

        #endregion

        #region Methods
        
        public void InsertFamilyLog(FamilyLogType logtype, string characterName = "", string characterName2 = "",
            string rainBowFamily = "", string message = "", byte level = 0, int experience = 0,
            int itemVNum = 0, byte upgrade = 0, int raidType = 0, FamilyAuthority authority = FamilyAuthority.Head,
            int righttype = 0, int rightvalue = 0)
        {
            string value = string.Empty;
            switch (logtype)
            {
                case FamilyLogType.DailyMessage:
                    value = $"{characterName}|{message}";
                    break;

                case FamilyLogType.FamilyXP:
                    value = $"{characterName}|{experience}";
                    break;

                case FamilyLogType.LevelUp:
                    value = $"{characterName}|{level}";
                    break;

                case FamilyLogType.RaidWon:
                    value = raidType.ToString();
                    break;

                case FamilyLogType.ItemUpgraded:
                    value = $"{characterName}|{itemVNum}|{upgrade}";
                    break;

                case FamilyLogType.UserManaged:
                    value = $"{characterName}|{characterName2}";
                    break;

                case FamilyLogType.FamilyLevelUp:
                    value = level.ToString();
                    break;

                case FamilyLogType.AuthorityChanged:
                    value = $"{characterName}|{(byte)authority}|{characterName2}";
                    break;

                case FamilyLogType.FamilyManaged:
                    value = characterName;
                    break;

                case FamilyLogType.RainbowBattle:
                    value = rainBowFamily;
                    break;

                case FamilyLogType.RightChanged:
                    value = $"{characterName}|{(byte)authority}|{righttype}|{rightvalue}";
                    break;

                case FamilyLogType.WareHouseAdded:
                case FamilyLogType.WareHouseRemoved:
                    value = $"{characterName}|{message}";
                    break;
            }

            var log = new FamilyLogDTO
            {
                FamilyId = FamilyId,
                FamilyLogData = value,
                FamilyLogType = logtype,
                Timestamp = DateTime.Now
            };
            DaoFactory.Instance.FamilyLogDao.InsertOrUpdate(ref log);
            ServerManager.Instance.FamilyRefresh(FamilyId);
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = FamilyId,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "fhis_stc",
                Type = MessageType.Family
            });
        }

        internal Family DeepCopy()
        {
            var clonedCharacter = (Family)MemberwiseClone();
            return clonedCharacter;
        }

        #endregion
    }
}