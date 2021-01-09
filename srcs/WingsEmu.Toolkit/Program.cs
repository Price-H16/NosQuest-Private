// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Autofac;
using AutoMapper;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Exceptions;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using WingsEmu.DAL.EF.DAO;
using WingsEmu.Plugins.DB.MSSQL;
using WingsEmu.Plugins.DB.MSSQL.Mapping;
using WingsEmu.Plugins.Logging;
using WingsEmu.Toolkit.Importers;

namespace WingsEmu.Toolkit
{
    public class Program
    {
        #region Methods

        private static IContainer BuildCoreContainer()
        {
            var pluginBuilder = new ContainerBuilder();
            pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces();
            // pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf();
            IContainer container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            foreach (ICorePlugin plugin in container.Resolve<IEnumerable<ICorePlugin>>())
            {
                try
                {
                    plugin.OnLoad(coreBuilder);
                }
                catch (PluginException e)
                {
                }
            }

            coreBuilder.Register(_ => new ToolkitMapper()).As<IMapper>().SingleInstance();
            return coreBuilder.Build();
        }

        public static void Main(string[] args)
        {
            // initialize logger
            Logger.InitializeLogger(new SerilogLogger());
            using (IContainer coreContainer = BuildCoreContainer())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var config = new ImportConfiguration
                {
                    Folder = "",
                    Lang = "uk",
                    Packets = new List<string[]>()
                };
                Console.Title = "WingsEmu - Parser";
                if (!DataAccessHelper.Initialize(coreContainer.Resolve<IOpenNosContextFactory>()))
                {
                    return;
                }

                DaoFactory.Initialize(coreContainer.Resolve<DaoFactory>());

                var key = new ConsoleKeyInfo();
                Logger.Log.Warn(Language.Instance.GetMessageFromKey("NEED_TREE"));
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine("Root");
                Console.ResetColor();
                Console.WriteLine("-----_code_en_Card.txt");
                Console.WriteLine("-----_code_en_Item.txt");
                Console.WriteLine("-----_code_en_MapIDData.txt");
                Console.WriteLine("-----_code_en_monster.txt");
                Console.WriteLine("-----_code_en_Skill.txt");
                Console.WriteLine("-----packet.txt");
                Console.WriteLine("-----Card.dat");
                Console.WriteLine("-----Item.dat");
                Console.WriteLine("-----MapIDData.dat");
                Console.WriteLine("-----monster.dat");
                Console.WriteLine("-----Skill.dat");
                Console.WriteLine("-----quest.dat");
                Console.WriteLine("-----qstprize.dat");
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine("-----map");
                Console.ResetColor();
                Console.WriteLine("----------0");
                Console.WriteLine("----------1");
                Console.WriteLine("----------...");

                try
                {
                    Logger.Log.Warn(Language.Instance.GetMessageFromKey("ENTER_PATH"));
                    string folder = string.Empty;
                    if (args.Length == 0)
                    {
                        folder = Console.ReadLine();
                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ALL")} [Y/n]");
                        key = Console.ReadKey(true);
                    }
                    else
                    {
                        folder = args.Aggregate(folder, (current, str) => current + str + " ");
                    }

                    config.Folder = folder;
                    var factory = new ImportFactory(config);
                    var packetImporter = new PacketImporter(config);
                    packetImporter.Import();

                    var mapImporter = new MapImporter(config);

                    if (key.KeyChar != 'n')
                    {
                        mapImporter.Import();
                        factory.LoadMaps();
                        factory.ImportCards();
                        factory.ImportRespawnMapType();
                        factory.ImportMapType();
                        factory.ImportMapTypeMap();
                        ImportFactory.ImportAccounts();
                        factory.ImportPortals();
                        factory.ImportScriptedInstances();
                        factory.ImportItems();
                        factory.ImportSkills();
                        factory.ImportNpcMonsters();
                        factory.ImportNpcMonsterData();
                        factory.ImportMapNpcs();
                        factory.ImportMonsters();
                        factory.ImportShops();
                        factory.ImportTeleporters();
                        factory.ImportShopItems();
                        factory.ImportShopSkills();
                        factory.ImportRecipe();
                        factory.ImportHardcodedItemRecipes();
                        factory.ImportQuests();
                    }
                    else
                    {
                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            mapImporter.Import();
                            factory.LoadMaps();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPTYPES")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportMapType();
                            factory.ImportMapTypeMap();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ACCOUNTS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            ImportFactory.ImportAccounts();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_PORTALS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportPortals();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_TIMESPACES")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportScriptedInstances();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ITEMS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportItems();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_NPCMONSTERS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportNpcMonsters();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_NPCMONSTERDATA")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportNpcMonsterData();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_CARDS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportCards();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SKILLS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportSkills();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPNPCS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportMapNpcs();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MONSTERS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportMonsters();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportShops();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_TELEPORTERS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportTeleporters();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPITEMS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportShopItems();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPSKILLS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportShopSkills();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_RECIPES")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportRecipe();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_HARDCODED_RECIPES")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportHardcodedItemRecipes();
                        }

                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_QUESTS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportQuests();
                        }
                    }

                    Console.WriteLine($"{Language.Instance.GetMessageFromKey("DONE")}");
                    Thread.Sleep(5000);
                }
                catch (FileNotFoundException e)
                {
                    Logger.Log.Error(Language.Instance.GetMessageFromKey("AT_LEAST_ONE_FILE_MISSING"), e);
                    Thread.Sleep(5000);
                }
            }
        }

        #endregion
    }
}