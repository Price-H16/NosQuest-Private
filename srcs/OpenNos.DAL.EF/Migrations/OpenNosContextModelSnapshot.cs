﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenNos.DAL.EF.DB;

namespace OpenNos.DAL.EF.Migrations
{
    [DbContext(typeof(OpenNosContext))]
    partial class OpenNosContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Account", b =>
                {
                    b.Property<long>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Authority");

                    b.Property<long>("BankMoney");

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<long>("Money");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("Password")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("RegistrationIP")
                        .HasMaxLength(45);

                    b.Property<string>("VerificationToken")
                        .HasMaxLength(32);

                    b.HasKey("AccountId");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.AntiBotLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CharacterId");

                    b.Property<string>("CharacterName");

                    b.Property<DateTime>("DateTime");

                    b.Property<bool>("Timeout");

                    b.HasKey("Id");

                    b.ToTable("AntiBotLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.BCard", b =>
                {
                    b.Property<int>("BCardId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short?>("CardId");

                    b.Property<byte>("CastType");

                    b.Property<int>("FirstData");

                    b.Property<bool>("IsLevelDivided");

                    b.Property<bool>("IsLevelScaled");

                    b.Property<short?>("ItemVNum");

                    b.Property<short?>("NpcMonsterVNum");

                    b.Property<int>("SecondData");

                    b.Property<short?>("SkillVNum");

                    b.Property<byte>("SubType");

                    b.Property<int>("ThirdData");

                    b.Property<byte>("Type");

                    b.HasKey("BCardId");

                    b.HasIndex("CardId");

                    b.HasIndex("ItemVNum");

                    b.HasIndex("NpcMonsterVNum");

                    b.HasIndex("SkillVNum");

                    b.ToTable("BCard");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.BazaarItem", b =>
                {
                    b.Property<long>("BazaarItemId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Amount");

                    b.Property<DateTime>("DateStart");

                    b.Property<short>("Duration");

                    b.Property<bool>("IsPackage");

                    b.Property<Guid>("ItemInstanceId");

                    b.Property<bool>("MedalUsed");

                    b.Property<long>("Price");

                    b.Property<long>("SellerId");

                    b.HasKey("BazaarItemId");

                    b.HasIndex("ItemInstanceId");

                    b.HasIndex("SellerId");

                    b.ToTable("BazaarItem");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Card", b =>
                {
                    b.Property<short>("CardId");

                    b.Property<byte>("BuffType");

                    b.Property<int>("Delay");

                    b.Property<int>("Duration");

                    b.Property<int>("EffectId");

                    b.Property<byte>("Level");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<byte>("Propability");

                    b.Property<short>("TimeoutBuff");

                    b.Property<byte>("TimeoutBuffChance");

                    b.HasKey("CardId");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Character", b =>
                {
                    b.Property<long>("CharacterId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AccountId");

                    b.Property<int>("Act4Dead");

                    b.Property<int>("Act4Kill");

                    b.Property<int>("Act4Points");

                    b.Property<int>("ArenaWinner");

                    b.Property<string>("Biography")
                        .HasMaxLength(255);

                    b.Property<bool>("BuffBlocked");

                    b.Property<byte>("Class");

                    b.Property<short>("Compliment");

                    b.Property<float>("Dignity");

                    b.Property<int>("Elo");

                    b.Property<bool>("EmoticonsBlocked");

                    b.Property<bool>("ExchangeBlocked");

                    b.Property<byte>("Faction");

                    b.Property<bool>("FamilyRequestBlocked");

                    b.Property<bool>("FriendRequestBlocked");

                    b.Property<byte>("Gender");

                    b.Property<long>("Gold");

                    b.Property<bool>("GroupRequestBlocked");

                    b.Property<byte>("HairColor");

                    b.Property<byte>("HairStyle");

                    b.Property<bool>("HeroChatBlocked");

                    b.Property<byte>("HeroLevel");

                    b.Property<long>("HeroXp");

                    b.Property<int>("Hp");

                    b.Property<bool>("HpBlocked");

                    b.Property<byte>("JobLevel");

                    b.Property<long>("JobLevelXp");

                    b.Property<byte>("Level");

                    b.Property<long>("LevelXp");

                    b.Property<short>("MapId");

                    b.Property<short>("MapX");

                    b.Property<short>("MapY");

                    b.Property<int>("MasterPoints");

                    b.Property<int>("MasterTicket");

                    b.Property<byte>("MaxMateCount");

                    b.Property<bool>("MinilandInviteBlocked");

                    b.Property<string>("MinilandMessage")
                        .HasMaxLength(255);

                    b.Property<short>("MinilandPoint");

                    b.Property<byte>("MinilandState");

                    b.Property<bool>("MouseAimLock");

                    b.Property<int>("Mp");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("Prefix")
                        .HasMaxLength(25);

                    b.Property<bool>("QuickGetUp");

                    b.Property<long>("RagePoint");

                    b.Property<long>("Reput");

                    b.Property<byte>("Slot");

                    b.Property<int>("SpAdditionPoint");

                    b.Property<int>("SpPoint");

                    b.Property<byte>("State");

                    b.Property<int>("TalentLose");

                    b.Property<int>("TalentSurrender");

                    b.Property<int>("TalentWin");

                    b.Property<bool>("WhisperBlocked");

                    b.HasKey("CharacterId");

                    b.HasIndex("AccountId");

                    b.HasIndex("MapId");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterHome", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<long>("CharacterId");

                    b.Property<short>("MapId");

                    b.Property<short>("MapX");

                    b.Property<short>("MapY");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("CharacterHome");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterQuest", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<long>("CharacterId");

                    b.Property<int>("FifthObjective");

                    b.Property<int>("FirstObjective");

                    b.Property<int>("FourthObjective");

                    b.Property<bool>("IsMainQuest");

                    b.Property<long>("QuestId");

                    b.Property<int>("SecondObjective");

                    b.Property<int>("ThirdObjective");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("QuestId");

                    b.ToTable("CharacterQuest");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterRelation", b =>
                {
                    b.Property<long>("CharacterRelationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CharacterId");

                    b.Property<long>("RelatedCharacterId");

                    b.Property<short>("RelationType");

                    b.HasKey("CharacterRelationId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("RelatedCharacterId");

                    b.ToTable("CharacterRelation");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterSkill", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<long>("CharacterId");

                    b.Property<short>("SkillVNum");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("SkillVNum");

                    b.ToTable("CharacterSkill");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Combo", b =>
                {
                    b.Property<int>("ComboId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Animation");

                    b.Property<short>("Effect");

                    b.Property<short>("Hit");

                    b.Property<short>("SkillVNum");

                    b.HasKey("ComboId");

                    b.HasIndex("SkillVNum");

                    b.ToTable("Combo");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Drop", b =>
                {
                    b.Property<short>("DropId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<int>("DropChance");

                    b.Property<short>("ItemVNum");

                    b.Property<short?>("MapTypeId");

                    b.Property<short?>("MonsterVNum");

                    b.HasKey("DropId");

                    b.HasIndex("ItemVNum");

                    b.HasIndex("MapTypeId");

                    b.HasIndex("MonsterVNum");

                    b.ToTable("Drop");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.EquipmentOption", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<byte>("Level");

                    b.Property<byte>("Type");

                    b.Property<int>("Value");

                    b.Property<Guid>("WearableInstanceId");

                    b.HasKey("Id");

                    b.HasIndex("WearableInstanceId");

                    b.ToTable("EquipmentOption");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ExchangeLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AccountId");

                    b.Property<long>("CharacterId");

                    b.Property<string>("CharacterName");

                    b.Property<DateTime>("Date");

                    b.Property<long>("Gold");

                    b.Property<short>("ItemAmount");

                    b.Property<short>("ItemVnum");

                    b.Property<long>("TargetAccountId");

                    b.Property<long>("TargetCharacterId");

                    b.Property<string>("TargetCharacterName");

                    b.HasKey("Id");

                    b.ToTable("ExchangeLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Family", b =>
                {
                    b.Property<long>("FamilyId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FamilyExperience");

                    b.Property<byte>("FamilyFaction");

                    b.Property<byte>("FamilyHeadGender");

                    b.Property<byte>("FamilyLevel");

                    b.Property<string>("FamilyMessage")
                        .HasMaxLength(255);

                    b.Property<byte>("ManagerAuthorityType");

                    b.Property<bool>("ManagerCanGetHistory");

                    b.Property<bool>("ManagerCanInvite");

                    b.Property<bool>("ManagerCanNotice");

                    b.Property<bool>("ManagerCanShout");

                    b.Property<byte>("MaxSize");

                    b.Property<byte>("MemberAuthorityType");

                    b.Property<bool>("MemberCanGetHistory");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<byte>("WarehouseSize");

                    b.HasKey("FamilyId");

                    b.ToTable("Family");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.FamilyCharacter", b =>
                {
                    b.Property<long>("FamilyCharacterId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("Authority");

                    b.Property<long>("CharacterId");

                    b.Property<string>("DailyMessage")
                        .HasMaxLength(255);

                    b.Property<int>("Experience");

                    b.Property<long>("FamilyId");

                    b.Property<byte>("Rank");

                    b.HasKey("FamilyCharacterId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("FamilyId");

                    b.ToTable("FamilyCharacter");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.FamilyLog", b =>
                {
                    b.Property<long>("FamilyLogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("FamilyId");

                    b.Property<string>("FamilyLogData")
                        .HasMaxLength(255);

                    b.Property<byte>("FamilyLogType");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("FamilyLogId");

                    b.HasIndex("FamilyId");

                    b.ToTable("FamilyLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.GeneralLog", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("AccountId");

                    b.Property<long?>("CharacterId");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(255);

                    b.Property<string>("LogData")
                        .HasMaxLength(255);

                    b.Property<string>("LogType");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("LogId");

                    b.HasIndex("AccountId");

                    b.HasIndex("CharacterId");

                    b.ToTable("GeneralLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Item", b =>
                {
                    b.Property<short>("VNum");

                    b.Property<byte>("BasicUpgrade");

                    b.Property<byte>("CellonLvl");

                    b.Property<byte>("Class");

                    b.Property<short>("CloseDefence");

                    b.Property<byte>("Color");

                    b.Property<short>("Concentrate");

                    b.Property<byte>("CriticalLuckRate");

                    b.Property<short>("CriticalRate");

                    b.Property<short>("DamageMaximum");

                    b.Property<short>("DamageMinimum");

                    b.Property<byte>("DarkElement");

                    b.Property<short>("DarkResistance");

                    b.Property<short>("DefenceDodge");

                    b.Property<short>("DistanceDefence");

                    b.Property<short>("DistanceDefenceDodge");

                    b.Property<short>("Effect");

                    b.Property<int>("EffectValue");

                    b.Property<byte>("Element");

                    b.Property<short>("ElementRate");

                    b.Property<byte>("EquipmentSlot");

                    b.Property<byte>("FireElement");

                    b.Property<short>("FireResistance");

                    b.Property<bool>("Flag1");

                    b.Property<bool>("Flag2");

                    b.Property<bool>("Flag3");

                    b.Property<bool>("Flag4");

                    b.Property<bool>("Flag5");

                    b.Property<bool>("Flag6");

                    b.Property<bool>("Flag7");

                    b.Property<bool>("Flag8");

                    b.Property<bool>("Flag9");

                    b.Property<byte>("Height");

                    b.Property<short>("HitRate");

                    b.Property<short>("Hp");

                    b.Property<short>("HpRegeneration");

                    b.Property<bool>("IsColored");

                    b.Property<bool>("IsConsumable");

                    b.Property<bool>("IsDroppable");

                    b.Property<bool>("IsHeroic");

                    b.Property<bool>("IsMinilandActionable");

                    b.Property<bool>("IsSoldable");

                    b.Property<bool>("IsTradable");

                    b.Property<bool>("IsWarehouse");

                    b.Property<byte>("ItemSubType");

                    b.Property<byte>("ItemType");

                    b.Property<long>("ItemValidTime");

                    b.Property<byte>("LevelJobMinimum");

                    b.Property<byte>("LevelMinimum");

                    b.Property<byte>("LightElement");

                    b.Property<short>("LightResistance");

                    b.Property<short>("MagicDefence");

                    b.Property<byte>("MaxCellon");

                    b.Property<byte>("MaxCellonLvl");

                    b.Property<short>("MaxElementRate");

                    b.Property<byte>("MaximumAmmo");

                    b.Property<int>("MinilandObjectPoint");

                    b.Property<short>("MoreHp");

                    b.Property<short>("MoreMp");

                    b.Property<short>("Morph");

                    b.Property<short>("Mp");

                    b.Property<short>("MpRegeneration");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<long>("Price");

                    b.Property<short>("PvpDefence");

                    b.Property<byte>("PvpStrength");

                    b.Property<short>("ReduceOposantResistance");

                    b.Property<long>("ReputPrice");

                    b.Property<byte>("ReputationMinimum");

                    b.Property<byte>("SecondaryElement");

                    b.Property<byte>("Sex");

                    b.Property<byte>("SpType");

                    b.Property<byte>("Speed");

                    b.Property<byte>("Type");

                    b.Property<short>("WaitDelay");

                    b.Property<byte>("WaterElement");

                    b.Property<short>("WaterResistance");

                    b.Property<byte>("Width");

                    b.HasKey("VNum");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ItemInstance", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<byte>("Agility");

                    b.Property<int>("Amount");

                    b.Property<long?>("BazaarItemId");

                    b.Property<long?>("BoundCharacterId");

                    b.Property<long>("CharacterId");

                    b.Property<short>("Design");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("DurabilityPoint");

                    b.Property<DateTime?>("ItemDeleteTime");

                    b.Property<short>("ItemVNum");

                    b.Property<short>("PartnerSkill1");

                    b.Property<short>("PartnerSkill2");

                    b.Property<short>("PartnerSkill3");

                    b.Property<short>("Rare");

                    b.Property<byte>("SkillRank1");

                    b.Property<byte>("SkillRank2");

                    b.Property<byte>("SkillRank3");

                    b.Property<short>("Slot");

                    b.Property<byte>("Type");

                    b.Property<byte>("Upgrade");

                    b.HasKey("Id");

                    b.HasIndex("BoundCharacterId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("ItemVNum");

                    b.ToTable("ItemInstance");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ItemInstance");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.LevelUpRewards", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Amount");

                    b.Property<short>("HeroLvl");

                    b.Property<bool>("IsMate");

                    b.Property<short>("JobLevel");

                    b.Property<short>("Level");

                    b.Property<short>("MateLevel");

                    b.Property<byte>("MateType");

                    b.Property<short>("Vnum");

                    b.HasKey("Id");

                    b.ToTable("LevelUpRewards");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.LogChat", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CharacterId");

                    b.Property<string>("ChatMessage")
                        .HasMaxLength(255);

                    b.Property<byte>("ChatType");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(255);

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("LogId");

                    b.HasIndex("CharacterId");

                    b.ToTable("LogChat");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.LogCommands", b =>
                {
                    b.Property<long>("CommandId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CharacterId");

                    b.Property<string>("Command");

                    b.Property<string>("Data");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(255);

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("CommandId");

                    b.HasIndex("CharacterId");

                    b.ToTable("LogCommands");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.LogVip", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("AccountId");

                    b.Property<long?>("CharacterId");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("VipPack");

                    b.HasKey("LogId");

                    b.HasIndex("AccountId");

                    b.HasIndex("CharacterId");

                    b.ToTable("LogVip");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Mail", b =>
                {
                    b.Property<long>("MailId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("AttachmentAmount");

                    b.Property<byte>("AttachmentRarity");

                    b.Property<byte>("AttachmentUpgrade");

                    b.Property<short?>("AttachmentVNum");

                    b.Property<DateTime>("Date");

                    b.Property<byte>("Design");

                    b.Property<string>("EqPacket")
                        .HasMaxLength(255);

                    b.Property<bool>("IsOpened");

                    b.Property<bool>("IsSenderCopy");

                    b.Property<string>("Message")
                        .HasMaxLength(255);

                    b.Property<long>("ReceiverId");

                    b.Property<byte>("SenderClass");

                    b.Property<byte>("SenderGender");

                    b.Property<byte>("SenderHairColor");

                    b.Property<byte>("SenderHairStyle");

                    b.Property<long>("SenderId");

                    b.Property<short>("SenderMorphId");

                    b.Property<string>("Title")
                        .HasMaxLength(255);

                    b.HasKey("MailId");

                    b.HasIndex("AttachmentVNum");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Mail");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Map", b =>
                {
                    b.Property<short>("MapId");

                    b.Property<byte[]>("Data");

                    b.Property<int>("Music");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<bool>("ShopAllowed");

                    b.HasKey("MapId");

                    b.ToTable("Map");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapMonster", b =>
                {
                    b.Property<int>("MapMonsterId");

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsMoving");

                    b.Property<short>("MapId");

                    b.Property<short>("MapX");

                    b.Property<short>("MapY");

                    b.Property<short>("MonsterVNum");

                    b.Property<byte>("Position");

                    b.HasKey("MapMonsterId");

                    b.HasIndex("MapId");

                    b.HasIndex("MonsterVNum");

                    b.ToTable("MapMonster");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapNpc", b =>
                {
                    b.Property<int>("MapNpcId");

                    b.Property<short>("Dialog");

                    b.Property<short>("Effect");

                    b.Property<short>("EffectDelay");

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsMoving");

                    b.Property<bool>("IsSitting");

                    b.Property<short>("MapId");

                    b.Property<short>("MapX");

                    b.Property<short>("MapY");

                    b.Property<short>("NpcVNum");

                    b.Property<byte>("Position");

                    b.HasKey("MapNpcId");

                    b.HasIndex("MapId");

                    b.HasIndex("NpcVNum");

                    b.ToTable("MapNpc");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapType", b =>
                {
                    b.Property<short>("MapTypeId");

                    b.Property<string>("MapTypeName");

                    b.Property<short>("PotionDelay");

                    b.Property<long?>("RespawnMapTypeId");

                    b.Property<long?>("ReturnMapTypeId");

                    b.HasKey("MapTypeId");

                    b.HasIndex("RespawnMapTypeId");

                    b.HasIndex("ReturnMapTypeId");

                    b.ToTable("MapType");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapTypeMap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("MapId");

                    b.Property<short>("MapTypeId");

                    b.HasKey("Id");

                    b.HasIndex("MapId");

                    b.HasIndex("MapTypeId");

                    b.ToTable("MapTypeMap");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Mate", b =>
                {
                    b.Property<long>("MateId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("Attack");

                    b.Property<bool>("CanPickUp");

                    b.Property<long>("CharacterId");

                    b.Property<byte>("Defence");

                    b.Property<byte>("Direction");

                    b.Property<long>("Experience");

                    b.Property<int>("Hp");

                    b.Property<bool>("IsSummonable");

                    b.Property<bool>("IsTeamMember");

                    b.Property<byte>("Level");

                    b.Property<short>("Loyalty");

                    b.Property<short>("MapX");

                    b.Property<short>("MapY");

                    b.Property<byte>("MateType");

                    b.Property<int>("Mp");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<short>("NpcMonsterVNum");

                    b.Property<short>("Skin");

                    b.HasKey("MateId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("NpcMonsterVNum");

                    b.ToTable("Mate");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MinilandObject", b =>
                {
                    b.Property<long>("MinilandObjectId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CharacterId");

                    b.Property<Guid?>("ItemInstanceId");

                    b.Property<byte>("Level1BoxAmount");

                    b.Property<byte>("Level2BoxAmount");

                    b.Property<byte>("Level3BoxAmount");

                    b.Property<byte>("Level4BoxAmount");

                    b.Property<byte>("Level5BoxAmount");

                    b.Property<short>("MapX");

                    b.Property<short>("MapY");

                    b.HasKey("MinilandObjectId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("ItemInstanceId");

                    b.ToTable("MinilandObject");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.NpcMonster", b =>
                {
                    b.Property<short>("NpcMonsterVNum");

                    b.Property<byte>("AmountRequired");

                    b.Property<byte>("AttackClass");

                    b.Property<byte>("AttackUpgrade");

                    b.Property<byte>("BasicArea");

                    b.Property<short>("BasicCooldown");

                    b.Property<byte>("BasicRange");

                    b.Property<short>("BasicSkill");

                    b.Property<short>("CloseDefence");

                    b.Property<short>("Concentrate");

                    b.Property<byte>("CriticalChance");

                    b.Property<short>("CriticalRate");

                    b.Property<short>("DamageMaximum");

                    b.Property<short>("DamageMinimum");

                    b.Property<short>("DarkResistance");

                    b.Property<short>("DefenceDodge");

                    b.Property<byte>("DefenceUpgrade");

                    b.Property<short>("DistanceDefence");

                    b.Property<short>("DistanceDefenceDodge");

                    b.Property<byte>("Element");

                    b.Property<short>("ElementRate");

                    b.Property<short>("FireResistance");

                    b.Property<int>("GiveDamagePercentage");

                    b.Property<byte>("HeroLevel");

                    b.Property<int>("HeroXP");

                    b.Property<bool>("IsHostile");

                    b.Property<bool>("IsPercent");

                    b.Property<int>("JobXP");

                    b.Property<byte>("Level");

                    b.Property<short>("LightResistance");

                    b.Property<short>("MagicDefence");

                    b.Property<int>("MaxHP");

                    b.Property<int>("MaxMP");

                    b.Property<byte>("MonsterType");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<bool>("NoAggresiveIcon");

                    b.Property<byte>("NoticeRange");

                    b.Property<byte>("Race");

                    b.Property<byte>("RaceType");

                    b.Property<int>("RespawnTime");

                    b.Property<byte>("Speed");

                    b.Property<int>("TakeDamages");

                    b.Property<short>("VNumRequired");

                    b.Property<short>("WaterResistance");

                    b.Property<int>("XP");

                    b.HasKey("NpcMonsterVNum");

                    b.ToTable("NpcMonster");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.NpcMonsterSkill", b =>
                {
                    b.Property<long>("NpcMonsterSkillId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("NpcMonsterVNum");

                    b.Property<short>("Rate");

                    b.Property<short>("SkillVNum");

                    b.HasKey("NpcMonsterSkillId");

                    b.HasIndex("NpcMonsterVNum");

                    b.HasIndex("SkillVNum");

                    b.ToTable("NpcMonsterSkill");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.PenaltyLog", b =>
                {
                    b.Property<int>("PenaltyLogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AccountId");

                    b.Property<string>("AdminName");

                    b.Property<DateTime>("DateEnd");

                    b.Property<DateTime>("DateStart");

                    b.Property<byte>("Penalty");

                    b.Property<string>("Reason")
                        .HasMaxLength(255);

                    b.HasKey("PenaltyLogId");

                    b.HasIndex("AccountId");

                    b.ToTable("PenaltyLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Portal", b =>
                {
                    b.Property<int>("PortalId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("DestinationMapId");

                    b.Property<short>("DestinationX");

                    b.Property<short>("DestinationY");

                    b.Property<bool>("IsDisabled");

                    b.Property<short>("SourceMapId");

                    b.Property<short>("SourceX");

                    b.Property<short>("SourceY");

                    b.Property<short>("Type");

                    b.HasKey("PortalId");

                    b.HasIndex("DestinationMapId");

                    b.HasIndex("SourceMapId");

                    b.ToTable("Portal");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Quest", b =>
                {
                    b.Property<long>("QuestId");

                    b.Property<int?>("EndDialogId");

                    b.Property<int>("InfoId");

                    b.Property<bool>("IsDaily");

                    b.Property<byte>("LevelMax");

                    b.Property<byte>("LevelMin");

                    b.Property<long?>("NextQuestId");

                    b.Property<int>("QuestType");

                    b.Property<int?>("StartDialogId");

                    b.Property<short?>("TargetMap");

                    b.Property<short?>("TargetX");

                    b.Property<short?>("TargetY");

                    b.HasKey("QuestId");

                    b.ToTable("Quest");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.QuestLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CharacterId");

                    b.Property<string>("IpAddress");

                    b.Property<DateTime?>("LastDaily");

                    b.Property<long>("QuestId");

                    b.HasKey("Id");

                    b.ToTable("QuestLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.QuestObjective", b =>
                {
                    b.Property<int>("QuestObjectiveId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Data");

                    b.Property<int?>("DropRate");

                    b.Property<int?>("Objective");

                    b.Property<byte>("ObjectiveIndex");

                    b.Property<int>("QuestId");

                    b.Property<int?>("SpecialData");

                    b.HasKey("QuestObjectiveId");

                    b.ToTable("QuestObjective");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.QuestReward", b =>
                {
                    b.Property<long>("QuestRewardId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount");

                    b.Property<int>("Data");

                    b.Property<byte>("Design");

                    b.Property<long>("QuestId");

                    b.Property<byte>("Rarity");

                    b.Property<byte>("RewardType");

                    b.Property<byte>("Upgrade");

                    b.HasKey("QuestRewardId");

                    b.ToTable("QuestReward");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.QuicklistEntry", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<long>("CharacterId");

                    b.Property<short>("Morph");

                    b.Property<short>("Pos");

                    b.Property<short>("Q1");

                    b.Property<short>("Q2");

                    b.Property<short>("Slot");

                    b.Property<short>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.ToTable("QuicklistEntry");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.RaidLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CharacterId");

                    b.Property<long?>("FamilyId");

                    b.Property<long>("RaidId");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("RaidLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Recipe", b =>
                {
                    b.Property<short>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("Amount");

                    b.Property<short>("ItemVNum");

                    b.Property<int>("MapNpcId");

                    b.Property<short>("ProduceItemVNum");

                    b.HasKey("RecipeId");

                    b.HasIndex("ItemVNum");

                    b.HasIndex("MapNpcId");

                    b.ToTable("Recipe");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.RecipeItem", b =>
                {
                    b.Property<short>("RecipeItemId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Amount");

                    b.Property<short>("ItemVNum");

                    b.Property<short>("RecipeId");

                    b.HasKey("RecipeItemId");

                    b.HasIndex("ItemVNum");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeItem");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Respawn", b =>
                {
                    b.Property<long>("RespawnId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CharacterId");

                    b.Property<short>("MapId");

                    b.Property<long>("RespawnMapTypeId");

                    b.Property<short>("X");

                    b.Property<short>("Y");

                    b.HasKey("RespawnId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("MapId");

                    b.HasIndex("RespawnMapTypeId");

                    b.ToTable("Respawn");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.RespawnMapType", b =>
                {
                    b.Property<long>("RespawnMapTypeId");

                    b.Property<short>("DefaultMapId");

                    b.Property<short>("DefaultX");

                    b.Property<short>("DefaultY");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.HasKey("RespawnMapTypeId");

                    b.HasIndex("DefaultMapId");

                    b.ToTable("RespawnMapType");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.RollGeneratedItem", b =>
                {
                    b.Property<short>("RollGeneratedItemId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsRareRandom");

                    b.Property<bool>("IsSuperReward");

                    b.Property<short>("ItemGeneratedAmount");

                    b.Property<byte>("ItemGeneratedUpgrade");

                    b.Property<short>("ItemGeneratedVNum");

                    b.Property<short>("MaximumOriginalItemRare");

                    b.Property<short>("MinimumOriginalItemRare");

                    b.Property<short>("OriginalItemDesign");

                    b.Property<short>("OriginalItemVNum");

                    b.Property<short>("Probability");

                    b.HasKey("RollGeneratedItemId");

                    b.HasIndex("ItemGeneratedVNum");

                    b.HasIndex("OriginalItemVNum");

                    b.ToTable("RollGeneratedItem");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ScriptedInstance", b =>
                {
                    b.Property<short>("ScriptedInstanceId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Label");

                    b.Property<short>("MapId");

                    b.Property<short>("PositionX");

                    b.Property<short>("PositionY");

                    b.Property<string>("Script")
                        .HasMaxLength(2147483647);

                    b.Property<byte>("Type");

                    b.HasKey("ScriptedInstanceId");

                    b.HasIndex("MapId");

                    b.ToTable("ScriptedInstance");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Shop", b =>
                {
                    b.Property<int>("ShopId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MapNpcId");

                    b.Property<byte>("MenuType");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<byte>("ShopType");

                    b.HasKey("ShopId");

                    b.HasIndex("MapNpcId");

                    b.ToTable("Shop");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ShopItem", b =>
                {
                    b.Property<int>("ShopItemId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("Color");

                    b.Property<short>("ItemVNum");

                    b.Property<short>("Rare");

                    b.Property<int>("ShopId");

                    b.Property<byte>("Slot");

                    b.Property<byte>("Type");

                    b.Property<byte>("Upgrade");

                    b.HasKey("ShopItemId");

                    b.HasIndex("ItemVNum");

                    b.HasIndex("ShopId");

                    b.ToTable("ShopItem");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ShopSkill", b =>
                {
                    b.Property<int>("ShopSkillId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ShopId");

                    b.Property<short>("SkillVNum");

                    b.Property<byte>("Slot");

                    b.Property<byte>("Type");

                    b.HasKey("ShopSkillId");

                    b.HasIndex("ShopId");

                    b.HasIndex("SkillVNum");

                    b.ToTable("ShopSkill");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Skill", b =>
                {
                    b.Property<short>("SkillVNum");

                    b.Property<short>("AttackAnimation");

                    b.Property<byte>("CPCost");

                    b.Property<short>("CastAnimation");

                    b.Property<short>("CastEffect");

                    b.Property<short>("CastId");

                    b.Property<short>("CastTime");

                    b.Property<byte>("Class");

                    b.Property<short>("Cooldown");

                    b.Property<short>("Duration");

                    b.Property<short>("Effect");

                    b.Property<byte>("Element");

                    b.Property<byte>("HitType");

                    b.Property<short>("ItemVNum");

                    b.Property<byte>("Level");

                    b.Property<byte>("LevelMinimum");

                    b.Property<byte>("MinimumAdventurerLevel");

                    b.Property<byte>("MinimumArcherLevel");

                    b.Property<byte>("MinimumMagicianLevel");

                    b.Property<byte>("MinimumSwordmanLevel");

                    b.Property<short>("MpCost");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<int>("Price");

                    b.Property<byte>("Range");

                    b.Property<byte>("SkillType");

                    b.Property<byte>("TargetRange");

                    b.Property<byte>("TargetType");

                    b.Property<byte>("Type");

                    b.Property<short>("UpgradeSkill");

                    b.Property<short>("UpgradeType");

                    b.HasKey("SkillVNum");

                    b.ToTable("Skill");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.StaticBonus", b =>
                {
                    b.Property<long>("StaticBonusId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CharacterId");

                    b.Property<DateTime>("DateEnd");

                    b.Property<byte>("StaticBonusType");

                    b.HasKey("StaticBonusId");

                    b.HasIndex("CharacterId");

                    b.ToTable("StaticBonus");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.StaticBuff", b =>
                {
                    b.Property<long>("StaticBuffId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("CardId");

                    b.Property<long>("CharacterId");

                    b.Property<int>("RemainingTime");

                    b.HasKey("StaticBuffId");

                    b.HasIndex("CardId");

                    b.HasIndex("CharacterId");

                    b.ToTable("StaticBuff");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Teleporter", b =>
                {
                    b.Property<short>("TeleporterId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Index");

                    b.Property<short>("MapId");

                    b.Property<int>("MapNpcId");

                    b.Property<short>("MapX");

                    b.Property<short>("MapY");

                    b.Property<byte>("Type");

                    b.HasKey("TeleporterId");

                    b.HasIndex("MapId");

                    b.HasIndex("MapNpcId");

                    b.ToTable("Teleporter");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.UpgradeLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("AccountId");

                    b.Property<long>("CharacterId");

                    b.Property<string>("CharacterName");

                    b.Property<DateTime>("Date");

                    b.Property<bool?>("HasAmulet");

                    b.Property<string>("ItemName");

                    b.Property<short>("ItemVnum");

                    b.Property<bool>("Success");

                    b.Property<string>("UpgradeType");

                    b.HasKey("Id");

                    b.ToTable("UpgradeLog");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.UsableInstance", b =>
                {
                    b.HasBaseType("OpenNos.DAL.EF.Entities.ItemInstance");

                    b.Property<short?>("HP");

                    b.Property<short?>("MP");

                    b.HasDiscriminator().HasValue("UsableInstance");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.WearableInstance", b =>
                {
                    b.HasBaseType("OpenNos.DAL.EF.Entities.ItemInstance");

                    b.Property<byte?>("Ammo");

                    b.Property<byte?>("Cellon");

                    b.Property<short?>("CloseDefence");

                    b.Property<short?>("Concentrate");

                    b.Property<short?>("CriticalDodge");

                    b.Property<byte?>("CriticalLuckRate");

                    b.Property<short?>("CriticalRate");

                    b.Property<short?>("DamageMaximum");

                    b.Property<short?>("DamageMinimum");

                    b.Property<byte?>("DarkElement");

                    b.Property<short?>("DarkResistance");

                    b.Property<short?>("DefenceDodge");

                    b.Property<short?>("DistanceDefence");

                    b.Property<short?>("DistanceDefenceDodge");

                    b.Property<short?>("ElementRate");

                    b.Property<byte?>("FireElement");

                    b.Property<short?>("FireResistance");

                    b.Property<short?>("HP")
                        .HasColumnName("WearableInstance_HP");

                    b.Property<short?>("HitRate");

                    b.Property<bool?>("IsEmpty");

                    b.Property<bool?>("IsFixed");

                    b.Property<byte?>("LightElement");

                    b.Property<short?>("LightResistance");

                    b.Property<short?>("MP")
                        .HasColumnName("WearableInstance_MP");

                    b.Property<short?>("MagicDefence");

                    b.Property<short?>("MaxElementRate");

                    b.Property<byte?>("ShellRarity");

                    b.Property<byte?>("WaterElement");

                    b.Property<short?>("WaterResistance");

                    b.Property<long?>("XP");

                    b.HasDiscriminator().HasValue("WearableInstance");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.SpecialistInstance", b =>
                {
                    b.HasBaseType("OpenNos.DAL.EF.Entities.WearableInstance");

                    b.Property<short?>("SlDamage");

                    b.Property<short?>("SlDefence");

                    b.Property<short?>("SlElement");

                    b.Property<short?>("SlHP");

                    b.Property<byte?>("SpDamage");

                    b.Property<byte?>("SpDark");

                    b.Property<byte?>("SpDefence");

                    b.Property<byte?>("SpElement");

                    b.Property<byte?>("SpFire");

                    b.Property<byte?>("SpHP");

                    b.Property<byte?>("SpLevel");

                    b.Property<byte?>("SpLight");

                    b.Property<byte?>("SpStoneUpgrade");

                    b.Property<byte?>("SpWater");

                    b.HasDiscriminator().HasValue("SpecialistInstance");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.BCard", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Card", "Card")
                        .WithMany("BCards")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "Item")
                        .WithMany("BCards")
                        .HasForeignKey("ItemVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.NpcMonster", "NpcMonster")
                        .WithMany("BCards")
                        .HasForeignKey("NpcMonsterVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Skill", "Skill")
                        .WithMany("BCards")
                        .HasForeignKey("SkillVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.BazaarItem", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.ItemInstance", "ItemInstance")
                        .WithMany("BazaarItem")
                        .HasForeignKey("ItemInstanceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("BazaarItem")
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Character", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Account", "Account")
                        .WithMany("Character")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("Character")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterHome", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("CharacterHome")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterQuest", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OpenNos.DAL.EF.Entities.Quest", "Quest")
                        .WithMany()
                        .HasForeignKey("QuestId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterRelation", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character1")
                        .WithMany("CharacterRelation1")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character2")
                        .WithMany("CharacterRelation2")
                        .HasForeignKey("RelatedCharacterId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.CharacterSkill", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("CharacterSkill")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Skill", "Skill")
                        .WithMany("CharacterSkill")
                        .HasForeignKey("SkillVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Combo", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Skill", "Skill")
                        .WithMany("Combo")
                        .HasForeignKey("SkillVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Drop", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "Item")
                        .WithMany("Drop")
                        .HasForeignKey("ItemVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.MapType", "MapType")
                        .WithMany("Drops")
                        .HasForeignKey("MapTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.NpcMonster", "NpcMonster")
                        .WithMany("Drop")
                        .HasForeignKey("MonsterVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.EquipmentOption", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.WearableInstance", "WearableInstance")
                        .WithMany()
                        .HasForeignKey("WearableInstanceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.FamilyCharacter", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("FamilyCharacter")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Family", "Family")
                        .WithMany("FamilyCharacters")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.FamilyLog", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Family", "Family")
                        .WithMany("FamilyLogs")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.GeneralLog", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Account", "Account")
                        .WithMany("GeneralLog")
                        .HasForeignKey("AccountId");

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("GeneralLog")
                        .HasForeignKey("CharacterId");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ItemInstance", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "BoundCharacter")
                        .WithMany()
                        .HasForeignKey("BoundCharacterId");

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("Inventory")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "Item")
                        .WithMany("ItemInstances")
                        .HasForeignKey("ItemVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.LogChat", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.LogCommands", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.LogVip", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId");
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Mail", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "Item")
                        .WithMany("Mail")
                        .HasForeignKey("AttachmentVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Receiver")
                        .WithMany("Mail1")
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Sender")
                        .WithMany("Mail")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapMonster", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("MapMonster")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.NpcMonster", "NpcMonster")
                        .WithMany("MapMonster")
                        .HasForeignKey("MonsterVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapNpc", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("MapNpc")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.NpcMonster", "NpcMonster")
                        .WithMany("MapNpc")
                        .HasForeignKey("NpcVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapType", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.RespawnMapType", "RespawnMapType")
                        .WithMany("MapTypes")
                        .HasForeignKey("RespawnMapTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.RespawnMapType", "ReturnMapType")
                        .WithMany("MapTypes1")
                        .HasForeignKey("ReturnMapTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MapTypeMap", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("MapTypeMap")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.MapType", "MapType")
                        .WithMany("MapTypeMap")
                        .HasForeignKey("MapTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Mate", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("Mate")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.NpcMonster", "NpcMonster")
                        .WithMany("Mate")
                        .HasForeignKey("NpcMonsterVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.MinilandObject", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("MinilandObject")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.ItemInstance", "ItemInstance")
                        .WithMany("MinilandObject")
                        .HasForeignKey("ItemInstanceId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.NpcMonsterSkill", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.NpcMonster", "NpcMonster")
                        .WithMany("NpcMonsterSkill")
                        .HasForeignKey("NpcMonsterVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Skill", "Skill")
                        .WithMany("NpcMonsterSkill")
                        .HasForeignKey("SkillVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.PenaltyLog", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Account", "Account")
                        .WithMany("PenaltyLog")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Portal", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("Portal")
                        .HasForeignKey("DestinationMapId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map1")
                        .WithMany("Portal1")
                        .HasForeignKey("SourceMapId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.QuicklistEntry", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("QuicklistEntry")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Recipe", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "Item")
                        .WithMany("Recipe")
                        .HasForeignKey("ItemVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.MapNpc", "MapNpc")
                        .WithMany("Recipe")
                        .HasForeignKey("MapNpcId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.RecipeItem", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "Item")
                        .WithMany("RecipeItem")
                        .HasForeignKey("ItemVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Recipe", "Recipe")
                        .WithMany("RecipeItem")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Respawn", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("Respawn")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("Respawn")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.RespawnMapType", "RespawnMapType")
                        .WithMany("Respawn")
                        .HasForeignKey("RespawnMapTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.RespawnMapType", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("RespawnMapType")
                        .HasForeignKey("DefaultMapId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.RollGeneratedItem", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "ItemGenerated")
                        .WithMany("RollGeneratedItem2")
                        .HasForeignKey("ItemGeneratedVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "OriginalItem")
                        .WithMany("RollGeneratedItem")
                        .HasForeignKey("OriginalItemVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ScriptedInstance", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("ScriptedInstance")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Shop", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.MapNpc", "MapNpc")
                        .WithMany("Shop")
                        .HasForeignKey("MapNpcId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ShopItem", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Item", "Item")
                        .WithMany("ShopItem")
                        .HasForeignKey("ItemVNum")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Shop", "Shop")
                        .WithMany("ShopItem")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.ShopSkill", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Shop", "Shop")
                        .WithMany("ShopSkill")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Skill", "Skill")
                        .WithMany("ShopSkill")
                        .HasForeignKey("SkillVNum")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.StaticBonus", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("StaticBonus")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.StaticBuff", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Card", "Card")
                        .WithMany("StaticBuff")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.Character", "Character")
                        .WithMany("StaticBuff")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("OpenNos.DAL.EF.Entities.Teleporter", b =>
                {
                    b.HasOne("OpenNos.DAL.EF.Entities.Map", "Map")
                        .WithMany("Teleporter")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OpenNos.DAL.EF.Entities.MapNpc", "MapNpc")
                        .WithMany("Teleporter")
                        .HasForeignKey("MapNpcId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
