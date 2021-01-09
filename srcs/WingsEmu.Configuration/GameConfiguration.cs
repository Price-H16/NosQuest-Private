// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WingsEmu.Configuration
{
    [DataContract]
    public class GameRateConfiguration : IConfiguration
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int XpRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int JobXpRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int HeroXpRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int FairyXpRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int MateXpRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int FamilyXpRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int ReputRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int DropRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int QuestDropRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int MaxGold { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long MaxBankGold { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int GoldDropRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int GoldRate { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int GlacernonPercentRatePvm { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int GlacernonPercentRatePvp { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int CylloanPercentRatePvm { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool ReputOnMonster { get; set; }
    }

    [DataContract]
    public class GameMinMaxConfiguration : IConfiguration
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public short MaxLevel { get; set; } = 99;


        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public short MaxMateLevel { get; set; } = 99;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public short MaxJobLevel { get; set; } = 80;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public short MaxSpLevel { get; set; } = 99;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public short MaxHeroLevel { get; set; } = 50;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public short HeroMinLevel { get; set; } = 88;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public short MinLodLevel { get; set; } = 50;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int MaximumHomes { get; set; } = 5;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public byte LobbySpeed { get; set; } = 30;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public byte MaxBotCodeAttempts { get; set; } = 3;
    }

    [DataContract]
    public class GameTrueFalseConfiguration : IConfiguration
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool IntroOnCharacterCreate { get; set; } = false;

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool RaidPortalFromAnywhere { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool Maintenance { get; set; }
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool AntiBot { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool LodTimes { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool AutoLoot { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool Easter { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool Winter { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool Estival { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool Halloween { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool Valentine { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public bool MessageOfTheDay { get; set; } = true;
    }
}