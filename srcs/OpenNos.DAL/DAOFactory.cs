// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DAL.Interface;

namespace OpenNos.DAL
{
    public class DaoFactory
    {
        public static DaoFactory Instance { get; private set; }

        public static void Initialize(DaoFactory dao)
        {
            Instance = dao;
        }

        public DaoFactory(IAccountDAO accountDao, IBazaarItemDAO bazaarItemDao, ICardDAO cardDao, IBCardDAO bcardDao, IRollGeneratedItemDAO rollGeneratedItemDao,
            IEquipmentOptionDAO equipmentOptionDao, ICharacterDAO characterDao, ICharacterRelationDAO characterRelationDao, ICharacterHomeDAO characterHomeDao, ICharacterSkillDAO characterskillDao,
            ICharacterQuestDAO characterQuestDao, IComboDAO comboDao, IDropDAO dropDao, IExchangeLogDao exchangeLogDao, IFamilyCharacterDAO familycharacterDao, IFamilyDAO familyDao,
            IFamilyLogDAO familylogDao, IGeneralLogDAO generallogDao, IItemDAO itemDao, IItemInstanceDAO iteminstanceDao, ILevelUpRewardsDAO levelUpRewardsDao, ILogChatDAO logChatDao,
            ILogCommandsDAO logCommandsDao, ILogVIPDAO logVipDao, IMailDAO mailDao, IMapDAO mapDao, IMapMonsterDAO mapmonsterDao, IMapNpcDAO mapnpcDao, IMapTypeDAO maptypeDao,
            IMapTypeMapDAO maptypemapDao, IMateDAO mateDao, IMinilandObjectDAO minilandobjectDao, INpcMonsterDAO npcmonsterDao, INpcMonsterSkillDAO npcmonsterskillDao, IPenaltyLogDAO penaltylogDao,
            IPortalDAO portalDao, IQuestDAO questDao, IQuestLogDAO questLogDao, IQuestRewardDAO questRewardDao, IQuestObjectiveDAO questObjectiveDao, IQuicklistEntryDAO quicklistDao,
            IRaidLogDAO raidLogDao, IRecipeDAO recipeDao, IRecipeItemDAO recipeitemDao, IRespawnDAO respawnDao, IRespawnMapTypeDAO respawnMapTypeDao, IScriptedInstanceDAO scriptedinstanceDao,
            IShopDAO shopDao, IShopItemDAO shopitemDao, IShopSkillDAO shopskillDao, ISkillDAO skillDao, IStaticBonusDAO staticBonusDao, IStaticBuffDAO staticBuffDao, ITeleporterDAO teleporterDao,
            IUpgradeLogDao upgradeLogDao, IAntiBotLogDAO antiBotLogDao)
        {
            AccountDao = accountDao;
            BazaarItemDao = bazaarItemDao;
            CardDao = cardDao;
            BCardDao = bcardDao;
            RollGeneratedItemDao = rollGeneratedItemDao;
            EquipmentOptionDao = equipmentOptionDao;
            CharacterDao = characterDao;
            CharacterRelationDao = characterRelationDao;
            CharacterHomeDao = characterHomeDao;
            CharacterSkillDao = characterskillDao;
            CharacterQuestDao = characterQuestDao;
            ComboDao = comboDao;
            DropDao = dropDao;
            ExchangeLogDao = exchangeLogDao;
            FamilyCharacterDao = familycharacterDao;
            FamilyDao = familyDao;
            FamilyLogDao = familylogDao;
            GeneralLogDao = generallogDao;
            ItemDao = itemDao;
            IteminstanceDao = iteminstanceDao;
            LevelUpRewardsDao = levelUpRewardsDao;
            LogChatDao = logChatDao;
            LogCommandsDao = logCommandsDao;
            LogVipDao = logVipDao;
            MailDao = mailDao;
            MapDao = mapDao;
            MapMonsterDao = mapmonsterDao;
            MapNpcDao = mapnpcDao;
            MapTypeDao = maptypeDao;
            MapTypeMapDao = maptypemapDao;
            MateDao = mateDao;
            MinilandObjectDao = minilandobjectDao;
            NpcMonsterDao = npcmonsterDao;
            NpcMonsterSkillDao = npcmonsterskillDao;
            PenaltyLogDao = penaltylogDao;
            PortalDao = portalDao;
            QuestDao = questDao;
            QuestLogDao = questLogDao;
            QuestRewardDao = questRewardDao;
            QuestObjectiveDao = questObjectiveDao;
            QuicklistEntryDao = quicklistDao;
            RaidLogDao = raidLogDao;
            RecipeDao = recipeDao;
            RecipeItemDao = recipeitemDao;
            RespawnDao = respawnDao;
            RespawnMapTypeDao = respawnMapTypeDao;
            ScriptedInstanceDao = scriptedinstanceDao;
            ShopDao = shopDao;
            ShopItemDao = shopitemDao;
            ShopSkillDao = shopskillDao;
            SkillDao = skillDao;
            StaticBonusDao = staticBonusDao;
            StaticBuffDao = staticBuffDao;
            TeleporterDao = teleporterDao;
            UpgradeLogDao = upgradeLogDao;
            AntiBotLogDao = antiBotLogDao;
        }


        public IAntiBotLogDAO AntiBotLogDao { get; }

        public IAccountDAO AccountDao { get; }

        public IBazaarItemDAO BazaarItemDao { get; }

        public ICardDAO CardDao { get; }

        public IEquipmentOptionDAO EquipmentOptionDao { get; }

        public ICharacterDAO CharacterDao { get; }

        public ICharacterHomeDAO CharacterHomeDao { get; }

        public ICharacterRelationDAO CharacterRelationDao { get; }

        public ICharacterSkillDAO CharacterSkillDao { get; }

        public ICharacterQuestDAO CharacterQuestDao { get; }

        public IComboDAO ComboDao { get; }

        public IDropDAO DropDao { get; }

        public IFamilyCharacterDAO FamilyCharacterDao { get; }

        public IFamilyDAO FamilyDao { get; }

        public IFamilyLogDAO FamilyLogDao { get; }

        public IGeneralLogDAO GeneralLogDao { get; }

        public IItemDAO ItemDao { get; }

        public IItemInstanceDAO IteminstanceDao { get; }

        public ILevelUpRewardsDAO LevelUpRewardsDao { get; }

        public ILogChatDAO LogChatDao { get; }

        public ILogCommandsDAO LogCommandsDao { get; }

        public ILogVIPDAO LogVipDao { get; }

        public IMailDAO MailDao { get; }

        public IMapDAO MapDao { get; }

        public IMapMonsterDAO MapMonsterDao { get; }

        public IMapNpcDAO MapNpcDao { get; }

        public IMapTypeDAO MapTypeDao { get; }

        public IMapTypeMapDAO MapTypeMapDao { get; }

        public IMateDAO MateDao { get; }

        public IMinilandObjectDAO MinilandObjectDao { get; }

        public INpcMonsterDAO NpcMonsterDao { get; }

        public INpcMonsterSkillDAO NpcMonsterSkillDao { get; }

        public IPenaltyLogDAO PenaltyLogDao { get; }

        public IPortalDAO PortalDao { get; }

        public IQuestDAO QuestDao { get; }

        public IQuestLogDAO QuestLogDao { get; }

        public IQuestObjectiveDAO QuestObjectiveDao { get; }

        public IQuestRewardDAO QuestRewardDao { get; }

        public IQuicklistEntryDAO QuicklistEntryDao { get; }

        public IRaidLogDAO RaidLogDao { get; }

        public IRecipeDAO RecipeDao { get; }

        public IRecipeItemDAO RecipeItemDao { get; }

        public IRespawnDAO RespawnDao { get; }

        public IRespawnMapTypeDAO RespawnMapTypeDao { get; }

        public IShopDAO ShopDao { get; }

        public IShopItemDAO ShopItemDao { get; }

        public IShopSkillDAO ShopSkillDao { get; }

        public ISkillDAO SkillDao { get; }

        public IStaticBonusDAO StaticBonusDao { get; }

        public IStaticBuffDAO StaticBuffDao { get; }

        public ITeleporterDAO TeleporterDao { get; }

        public IScriptedInstanceDAO ScriptedInstanceDao { get; }

        public IBCardDAO BCardDao { get; }

        public IRollGeneratedItemDAO RollGeneratedItemDao { get; }

        public IExchangeLogDao ExchangeLogDao { get; }

        public IUpgradeLogDao UpgradeLogDao { get; }
    }
}