﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs.Character
{
    public class CharacterDTO : MappingBaseDTO
    {
        #region Properties

        public long AccountId { get; set; }

        public int Act4Dead { get; set; }

        public int Act4Kill { get; set; }

        public int Act4Points { get; set; }

        public int ArenaWinner { get; set; }

        public string Biography { get; set; }

        public bool BuffBlocked { get; set; }

        public long CharacterId { get; set; }

        public ClassType Class { get; set; }

        public short Compliment { get; set; }

        public float Dignity { get; set; }

        public int Elo { get; set; }

        public bool EmoticonsBlocked { get; set; }

        public bool ExchangeBlocked { get; set; }

        public FactionType Faction { get; set; }

        public bool FamilyRequestBlocked { get; set; }

        public bool FriendRequestBlocked { get; set; }

        public GenderType Gender { get; set; }

        public long Gold { get; set; }

        public bool GroupRequestBlocked { get; set; }

        public HairColorType HairColor { get; set; }

        public HairStyleType HairStyle { get; set; }

        public bool HeroChatBlocked { get; set; }

        public byte HeroLevel { get; set; }

        public long HeroXp { get; set; }

        public int Hp { get; set; }

        public bool HpBlocked { get; set; }

        public bool IsPetAutoRelive { get; set; }

        public bool IsPartnerAutoRelive { get; set; }

        public byte JobLevel { get; set; }

        public long JobLevelXp { get; set; }

        public byte Level { get; set; }

        public long LevelXp { get; set; }

        public short MapId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public int MasterPoints { get; set; }

        public int MasterTicket { get; set; }

        public byte MaxMateCount { get; set; }

        public bool MinilandInviteBlocked { get; set; }

        public string MinilandMessage { get; set; }

        public short MinilandPoint { get; set; }

        public MinilandState MinilandState { get; set; }

        public bool MouseAimLock { get; set; }

        public int Mp { get; set; }

        public string Prefix { get; set; }

        public string Name { get; set; }

        public bool QuickGetUp { get; set; }

        public long RagePoint { get; set; }

        public long Reput { get; set; }

        public byte Slot { get; set; }

        public int SpAdditionPoint { get; set; }

        public int SpPoint { get; set; }

        public CharacterState State { get; set; }

        public int TalentLose { get; set; }

        public int TalentSurrender { get; set; }

        public int TalentWin { get; set; }

        public bool WhisperBlocked { get; set; }

        #endregion
    }
}