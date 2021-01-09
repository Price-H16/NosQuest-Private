// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Autofac;
using AutoMapper;
using ChickenAPI.Core.Logging;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Modules;
using dotenv.net;
using Microsoft.EntityFrameworkCore.Design;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using WingsEmu.Communication;
using WingsEmu.Communication.RPC;
using WingsEmu.DAL.EF.DAO;
using WingsEmu.Network;
using WingsEmu.Packets;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Plugins.DB.MSSQL;
using WingsEmu.Plugins.DB.MSSQL.Mapping;
using WingsEmu.Plugins.Logging;

namespace WingsEmu.Login
{
    public class Program
    {
        #region Methods

        private static void InitializeMasterCommunication()
        {
            int masterPort = Convert.ToInt32(Environment.GetEnvironmentVariable("MASTER_PORT") ?? "20500");
            string masterIp = Environment.GetEnvironmentVariable("MASTER_IP") ?? "localhost";
            var serviceFactory = new GRpcCommunicationServiceFactory();
            ICommunicationService service = serviceFactory.CreateService(masterIp, masterPort).ConfigureAwait(false).GetAwaiter().GetResult();
            CommunicationServiceClient.Initialize(service);
        }

        private static void PrintHeader()
        {
            Console.Title = "WingsEmu - Login";
            const string text = @"
 __      __.__                     ___________              
/  \    /  \__| ____    ____  _____\_   _____/ _____  __ __ 
\   \/\/   /  |/    \  / ___\/  ___/|    __)_ /     \|  |  \
 \        /|  |   |  \/ /_/  >___ \ |        \  Y Y  \  |  /
  \__/\  / |__|___|  /\___  /____  >_______  /__|_|  /____/ 
       \/          \//_____/     \/        \/      \/       
            .____                 .__                       
            |    |    ____   ____ |__| ____                 
            |    |   /  _ \ / ___\|  |/    \                
            |    |__(  <_> ) /_/  >  |   |  \               
            |_______ \____/\___  /|__|___|  /               
                    \/    /_____/         \/                
";
            string separator = new string('=', Console.WindowWidth);
            string logo = text.Split('\n').Select(s => string.Format("{0," + (Console.WindowWidth / 2 + s.Length / 2) + "}\n", s))
                .Aggregate("", (current, i) => current + i);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(separator + logo + separator);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Main()
        {
            try
            {
                PrintHeader();
                Logger.InitializeLogger(new SerilogLogger());
                DotEnv.Config(false, "server.env");

                int port = Convert.ToInt32(Environment.GetEnvironmentVariable("SERVER_PORT") ?? "4000");
                Console.Title = $"WingsEmu - Login Server - {port}";


                var pluginBuilder = new ContainerBuilder();
                pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces().AsSelf();
                pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
                pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf();
                IContainer container = pluginBuilder.Build();

                var coreBuilder = new ContainerBuilder();
                foreach (ICorePlugin plugin in container.Resolve<IEnumerable<ICorePlugin>>())
                {
                    plugin.OnLoad(coreBuilder);
                }

                coreBuilder.Register(_ => new ToolkitMapper()).As<IMapper>().SingleInstance();

                using (IContainer coreContainer = coreBuilder.Build())
                {
                    var gameBuilder = new ContainerBuilder();
                    gameBuilder.RegisterInstance(coreContainer).As<IContainer>();
                    gameBuilder.RegisterModule(new CoreContainerModule(coreContainer));
                    IContainer gameContainer = gameBuilder.Build();
                    IEnumerable<IGamePlugin> plugins = gameContainer.Resolve<IEnumerable<IGamePlugin>>();
                    if (plugins != null)
                    {
                        foreach (IGamePlugin gamePlugin in plugins)
                        {
                            gamePlugin.OnEnable();
                            gamePlugin.OnDisable();
                        }
                    }

                    Logger.InitializeLogger(coreContainer.Resolve<ILogger>());
                    // initialize Logger
                    InitializeMasterCommunication();

                    // initialize api
                    if (CommunicationServiceClient.Instance.IsMasterOnline())
                    {
                        Logger.Log.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
                    }

                    // initialize DB
                    if (!DataAccessHelper.Initialize(coreContainer.Resolve<IOpenNosContextFactory>()))
                    {
                        Console.ReadLine();
                        return;
                    }
                    DaoFactory.Initialize(coreContainer.Resolve<DaoFactory>());

                    Logger.Log.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                    try
                    {

                        // initialize PacketSerialization
                        PacketFactory.Initialize<WalkPacket>();

                        var server = new LoginServer(IPAddress.Any, port, new BasicSpamProtector());
                        server.Start();

                        for (;;)
                        {
                            string tmp = Console.ReadLine();
                            if (tmp == "quit")
                            {
                                break;
                            }
                        }

                        server.Stop();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("General Error Server", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("General Error", ex);
                Console.ReadKey();
            }
        }

        #endregion
    }
}