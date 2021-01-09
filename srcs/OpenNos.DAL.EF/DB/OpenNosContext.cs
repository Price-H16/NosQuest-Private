// WingsEmu
// 
// Developed by NosWings Team

using Microsoft.EntityFrameworkCore;
using OpenNos.DAL.EF.Entities;

namespace OpenNos.DAL.EF.DB
{
    public class OpenNosContext : DbContext
    {
        public OpenNosContext(DbContextOptions<OpenNosContext> options) : base(options)
        {
        }


        public virtual DbSet<Account> Account { get; set; }

        public virtual DbSet<BazaarItem> BazaarItem { get; set; }

        public virtual DbSet<Card> Card { get; set; }

        public virtual DbSet<BCard> BCard { get; set; }

        public virtual DbSet<EquipmentOption> EquipmentOption { get; set; }

        public virtual DbSet<ExchangeLog> ExchangeLog { get; set; }

        public virtual DbSet<Character> Character { get; set; }

        public virtual DbSet<CharacterQuest> CharacterQuest { get; set; }

        public virtual DbSet<CharacterRelation> CharacterRelation { get; set; }

        public virtual DbSet<CharacterSkill> CharacterSkill { get; set; }

        public virtual DbSet<RollGeneratedItem> RollGeneratedItem { get; set; }

        public virtual DbSet<Combo> Combo { get; set; }

        public virtual DbSet<Drop> Drop { get; set; }

        public virtual DbSet<Family> Family { get; set; }

        public virtual DbSet<FamilyCharacter> FamilyCharacter { get; set; }

        public virtual DbSet<FamilyLog> FamilyLog { get; set; }

        public virtual DbSet<GeneralLog> GeneralLog { get; set; }

        public virtual DbSet<Item> Item { get; set; }

        public virtual DbSet<ItemInstance> ItemInstance { get; set; }

        public virtual DbSet<LevelUpRewards> LevelUpRewards { get; set; }

        public virtual DbSet<LogChat> LogChat { get; set; }

        public virtual DbSet<LogVip> LogVip { get; set; }

        public virtual DbSet<LogCommands> LogCommands { get; set; }

        public virtual DbSet<Mail> Mail { get; set; }

        public virtual DbSet<Map> Map { get; set; }

        public virtual DbSet<MapMonster> MapMonster { get; set; }

        public virtual DbSet<MapNpc> MapNpc { get; set; }

        public virtual DbSet<MapType> MapType { get; set; }

        public virtual DbSet<MapTypeMap> MapTypeMap { get; set; }

        public virtual DbSet<Mate> Mate { get; set; }

        public virtual DbSet<MinilandObject> MinilandObject { get; set; }

        public virtual DbSet<NpcMonster> NpcMonster { get; set; }

        public virtual DbSet<NpcMonsterSkill> NpcMonsterSkill { get; set; }

        public virtual DbSet<PenaltyLog> PenaltyLog { get; set; }

        public virtual DbSet<Portal> Portal { get; set; }

        public virtual DbSet<Quest> Quest { get; set; }

        public virtual DbSet<QuestLog> QuestLog { get; set; }

        public virtual DbSet<QuestReward> QuestReward { get; set; }

        public virtual DbSet<QuestObjective> QuestObjective { get; set; }

        public virtual DbSet<QuicklistEntry> QuicklistEntry { get; set; }

        public virtual DbSet<RaidLog> RaidLog { get; set; }

        public virtual DbSet<Recipe> Recipe { get; set; }

        public virtual DbSet<RecipeItem> RecipeItem { get; set; }

        public virtual DbSet<Respawn> Respawn { get; set; }

        public virtual DbSet<RespawnMapType> RespawnMapType { get; set; }

        public virtual DbSet<ScriptedInstance> ScriptedInstance { get; set; }

        public virtual DbSet<Shop> Shop { get; set; }

        public virtual DbSet<ShopItem> ShopItem { get; set; }

        public virtual DbSet<ShopSkill> ShopSkill { get; set; }

        public virtual DbSet<Skill> Skill { get; set; }

        public virtual DbSet<StaticBonus> StaticBonus { get; set; }

        public virtual DbSet<Teleporter> Teleporter { get; set; }

        public virtual DbSet<CharacterHome> CharacterHome { get; set; }

        public virtual DbSet<StaticBuff> StaticBuff { get; set; }

        public virtual DbSet<UpgradeLog> UpgradeLog { get; set; }

        public virtual DbSet<AntiBotLog> AntiBotLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // remove automatic pluralization

            modelBuilder.Entity<Character>()
                .HasMany(e => e.CharacterHome)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId);

            modelBuilder.Entity<CharacterHome>()
                .HasOne(e => e.Character)
                .WithMany(s => s.CharacterHome);

            // build TPH tables for inheritance

            modelBuilder.Entity<WearableInstance>()
                .HasBaseType<ItemInstance>();

            modelBuilder.Entity<SpecialistInstance>()
                .HasBaseType<WearableInstance>();

            modelBuilder.Entity<UsableInstance>()
                .HasBaseType<ItemInstance>();


            modelBuilder.Entity<Account>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.Character)
                .WithOne(e => e.Account)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.PenaltyLog)
                .WithOne(e => e.Account)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.Inventory)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.Mate)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.CharacterSkill)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.StaticBonus)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.CharacterRelation1)
                .WithOne(e => e.Character1)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.CharacterRelation2)
                .WithOne(e => e.Character2)
                .HasForeignKey(e => e.RelatedCharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.StaticBuff)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Card>()
                .HasMany(e => e.StaticBuff)
                .WithOne(e => e.Card)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.QuicklistEntry)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.Respawn)
                .WithOne(e => e.Character)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.Mail)
                .WithOne(e => e.Sender)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.MinilandObject)
                .WithOne(e => e.Character)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasMany(e => e.Mail1)
                .WithOne(e => e.Receiver)
                .HasForeignKey(e => e.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Family>()
                .HasMany(e => e.FamilyLogs)
                .WithOne(e => e.Family)
                .HasForeignKey(e => e.FamilyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FamilyCharacter>()
                .HasOne(e => e.Character)
                .WithMany(e => e.FamilyCharacter)
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BazaarItem>()
                .HasOne(e => e.Character)
                .WithMany(e => e.BazaarItem)
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BazaarItem>()
                .HasOne(e => e.ItemInstance)
                .WithMany(e => e.BazaarItem)
                .HasForeignKey(e => e.ItemInstanceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MinilandObject>()
                .HasOne(e => e.ItemInstance)
                .WithMany(e => e.MinilandObject)
                .HasForeignKey(e => e.ItemInstanceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FamilyCharacter>()
                .HasOne(e => e.Family)
                .WithMany(e => e.FamilyCharacters)
                .HasForeignKey(e => e.FamilyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.Drop)
                .WithOne(e => e.Item)
                .HasForeignKey(e => e.ItemVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.Recipe)
                .WithOne(e => e.Item)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.RecipeItem)
                .WithOne(e => e.Item)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.ItemInstances)
                .WithOne(e => e.Item)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasMany(e => e.ShopItem)
                .WithOne(e => e.Item)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mail>()
                .HasOne(e => e.Item)
                .WithMany(e => e.Mail)
                .HasForeignKey(e => e.AttachmentVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RollGeneratedItem>()
                .HasOne(e => e.OriginalItem)
                .WithMany(e => e.RollGeneratedItem)
                .HasForeignKey(e => e.OriginalItemVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RollGeneratedItem>()
                .HasOne(e => e.ItemGenerated)
                .WithMany(e => e.RollGeneratedItem2)
                .HasForeignKey(e => e.ItemGeneratedVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Map>()
                .HasMany(e => e.Character)
                .WithOne(e => e.Map)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Map>()
                .HasMany(e => e.MapMonster)
                .WithOne(e => e.Map)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Respawn>()
                .HasOne(e => e.Map)
                .WithMany(e => e.Respawn)
                .HasForeignKey(e => e.MapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Respawn>()
                .HasOne(e => e.RespawnMapType)
                .WithMany(e => e.Respawn)
                .HasForeignKey(e => e.RespawnMapTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RespawnMapType>()
                .HasOne(e => e.Map)
                .WithMany(e => e.RespawnMapType)
                .HasForeignKey(e => e.DefaultMapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapType>()
                .HasOne(e => e.RespawnMapType)
                .WithMany(e => e.MapTypes)
                .HasForeignKey(e => e.RespawnMapTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapType>()
                .HasOne(e => e.ReturnMapType)
                .WithMany(e => e.MapTypes1)
                .HasForeignKey(e => e.ReturnMapTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Map>()
                .HasMany(e => e.MapNpc)
                .WithOne(e => e.Map)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Map>()
                .HasMany(e => e.Portal)
                .WithOne(e => e.Map)
                .HasForeignKey(e => e.DestinationMapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Map>()
                .HasMany(e => e.Portal1)
                .WithOne(e => e.Map1)
                .HasForeignKey(e => e.SourceMapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Map>()
                .HasMany(e => e.ScriptedInstance)
                .WithOne(e => e.Map)
                .HasForeignKey(e => e.MapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Map>()
                .HasMany(e => e.Teleporter)
                .WithOne(e => e.Map)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BCard>()
                .HasOne(e => e.Skill)
                .WithMany(e => e.BCards)
                .HasForeignKey(e => e.SkillVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BCard>()
                .HasOne(e => e.NpcMonster)
                .WithMany(e => e.BCards)
                .HasForeignKey(e => e.NpcMonsterVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BCard>()
                .HasOne(s => s.Card)
                .WithMany(s => s.BCards)
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BCard>()
                .HasOne(e => e.Item)
                .WithMany(e => e.BCards)
                .HasForeignKey(e => e.ItemVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapTypeMap>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<MapTypeMap>()
                .HasOne(e => e.Map)
                .WithMany(e => e.MapTypeMap)
                .HasForeignKey(e => e.MapId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapTypeMap>()
                .HasOne(e => e.MapType)
                .WithMany(e => e.MapTypeMap)
                .HasForeignKey(e => e.MapTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapType>()
                .HasMany(e => e.Drops)
                .WithOne(e => e.MapType)
                .HasForeignKey(e => e.MapTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapNpc>()
                .HasMany(e => e.Recipe)
                .WithOne(e => e.MapNpc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapNpc>()
                .HasMany(e => e.Shop)
                .WithOne(e => e.MapNpc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MapNpc>()
                .HasMany(e => e.Teleporter)
                .WithOne(e => e.MapNpc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NpcMonster>()
                .HasMany(e => e.Drop)
                .WithOne(s => s.NpcMonster)
                .HasForeignKey(e => e.MonsterVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NpcMonster>()
                .HasMany(e => e.Mate)
                .WithOne(e => e.NpcMonster)
                .HasForeignKey(e => e.NpcMonsterVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NpcMonster>()
                .HasMany(e => e.MapMonster)
                .WithOne(e => e.NpcMonster)
                .HasForeignKey(e => e.MonsterVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NpcMonster>()
                .HasMany(e => e.MapNpc)
                .WithOne(e => e.NpcMonster)
                .HasForeignKey(e => e.NpcVNum)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NpcMonster>()
                .HasMany(e => e.NpcMonsterSkill)
                .WithOne(e => e.NpcMonster)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Recipe>()
                .HasMany(e => e.RecipeItem)
                .WithOne(e => e.Recipe)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shop>()
                .HasMany(e => e.ShopItem)
                .WithOne(e => e.Shop)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shop>()
                .HasMany(e => e.ShopSkill)
                .WithOne(e => e.Shop)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.CharacterSkill)
                .WithOne(e => e.Skill)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.Combo)
                .WithOne(e => e.Skill)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.NpcMonsterSkill)
                .WithOne(e => e.Skill)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.ShopSkill)
                .WithOne(e => e.Skill)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}