// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Autofac;
using AutoMapper;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Exceptions;
using ChickenAPI.Plugins.Modules;
using dotenv.net;
using Grpc.Core;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.Core.Utilities;
using OpenNos.DAL;
using OpenNos.GameObject.Configuration;
using OpenNos.GameObject.Networking;
using WingsEmu.Communication;
using WingsEmu.Communication.RPC;
using WingsEmu.Configuration;
using WingsEmu.Customization.NewCharCustomisation;
using WingsEmu.DAL.EF.DAO;
using WingsEmu.Packets;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Plugins.DB.MSSQL;
using WingsEmu.Plugins.DB.MSSQL.Mapping;
using WingsEmu.Plugins.Logging;
using ConfigurationHelper = WingsEmu.World.Configuration.ConfigurationHelper;

namespace WingsEmu.World
{
    public static class Program
    {
        #region Methods

        private static int _port;

        private static void PrintHeader()
        {
            Console.Title = "WingsEmu - World";
            const string text = @"
 __      __.__                     ___________              
/  \    /  \__| ____    ____  _____\_   _____/ _____  __ __ 
\   \/\/   /  |/    \  / ___\/  ___/|    __)_ /     \|  |  \
 \        /|  |   |  \/ /_/  >___ \ |        \  Y Y  \  |  /
  \__/\  / |__|___|  /\___  /____  >_______  /__|_|  /____/ 
       \/          \//_____/     \/        \/      \/       
            __      __            .__       .___            
           /  \    /  \___________|  |    __| _/            
           \   \/\/   /  _ \_  __ \  |   / __ |             
            \        (  <_> )  | \/  |__/ /_/ |             
             \__/\  / \____/|__|  |____/\____ |             
                  \/                         \/             
";
            string separator = new string('=', Console.WindowWidth);
            string logo = text.Split('\n').Select(s => string.Format("{0," + (Console.WindowWidth / 2 + s.Length / 2) + "}\n", s))
                .Aggregate("", (current, i) => current + i);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(separator + logo + separator);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void InitializeMasterCommunication()
        {
            int masterPort = Convert.ToInt32(Environment.GetEnvironmentVariable("MASTER_PORT") ?? "20500");
            string masterIp = Environment.GetEnvironmentVariable("MASTER_IP") ?? "localhost";
            var serviceFactory = new GRpcCommunicationServiceFactory();
            ICommunicationService service = serviceFactory.CreateService(masterIp, masterPort).ConfigureAwait(false).GetAwaiter().GetResult();
            CommunicationServiceClient.Initialize(service);
        }

        private static void CustomisationRegistration()
        {
            const string configPath = "./config/";
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseCharacter>(configPath + nameof(BaseCharacter) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseCharacter Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseQuicklist>(configPath + nameof(BaseQuicklist) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseQuicklist Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseInventory>(configPath + nameof(BaseInventory) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseInventory Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<BaseSkill>(configPath + nameof(BaseSkill) + ".json", true));
            Logger.Log.Info("[CUSTOMIZER] BaseSkill Loaded !");

            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameRateConfiguration>(configPath + "game.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game Rate            Configuration Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameMinMaxConfiguration>(configPath + "min_max.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game MinMax          Configuration Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameTrueFalseConfiguration>(configPath + "events.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game TrueFalse       Configuration Loaded !");
            DependencyContainer.Instance.Register(ConfigurationHelper.Load<GameScheduledEventsConfiguration>(configPath + "scheduled_events.json", true));
            Logger.Log.Info("[CUSTOMIZER] Game ScheduledEvents Configuration Loaded !");
        }

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

            coreBuilder.RegisterType<WorldServiceImpl>();
            coreBuilder.Register(_ => new GameObjectMapper()).As<IMapper>().SingleInstance();
            return coreBuilder.Build();
        }

        public static void Main(string[] args)
        {
            PrintHeader();
            Logger.InitializeLogger(new SerilogLogger());
            DotEnv.Config(false, "server.env");
            using (IContainer coreContainer = BuildCoreContainer())
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

                // initialize Loggers
                CustomisationRegistration();

                int gRpcPort = Convert.ToInt32(Environment.GetEnvironmentVariable("GRPC_PORT") ?? "17500");
                string gRpcIp = Environment.GetEnvironmentVariable("GRPC_IP") ?? "localhost";
                var gRpcEndPoint = new GRpcEndPoint
                {
                    Ip = gRpcIp,
                    Port = gRpcPort
                };
                var gRpcServer = new Server
                {
                    Services = { global::World.BindService(coreContainer.Resolve<WorldServiceImpl>()) },
                    Ports = { new ServerPort(gRpcEndPoint.Ip, gRpcEndPoint.Port, ServerCredentials.Insecure) }
                };
                Logger.Log.Info($"[RPC-Server] Listening on {gRpcEndPoint.Ip}:{gRpcEndPoint.Port}");
                gRpcServer.Start();

                InitializeMasterCommunication();

                // initialize api
                if (CommunicationServiceClient.Instance.IsMasterOnline())
                {
                    Logger.Log.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
                }

                // initialize DB
                if (!DataAccessHelper.Initialize(coreContainer.Resolve<IOpenNosContextFactory>()))
                {
                    Console.ReadKey();
                    return;
                }

                DaoFactory.Initialize(coreContainer.Resolve<DaoFactory>());

                // initialilize maps
                ServerManager.Instance.Initialize(
                    DependencyContainer.Instance.Get<GameRateConfiguration>(),
                    DependencyContainer.Instance.Get<GameMinMaxConfiguration>(),
                    DependencyContainer.Instance.Get<GameTrueFalseConfiguration>(),
                    DependencyContainer.Instance.Get<GameScheduledEventsConfiguration>()
                );


                PacketFactory.Initialize<WalkPacket>();
                string ip = Environment.GetEnvironmentVariable("SERVER_IP") ?? "127.0.0.1";
                _port = Convert.ToInt32(Environment.GetEnvironmentVariable("SERVER_PORT") ?? "1337");


                WorldServer server;
                portloop:
                try
                {
                    server = new WorldServer(IPAddress.Any, _port);
                    server.Start();
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10048)
                    {
                        _port++;
                        Logger.Log.Info("Port already in use! Incrementing...");
                        goto portloop;
                    }

                    Logger.Log.Error("General Error", ex);
                    Environment.Exit(1);
                    return;
                }

                ServerManager.Instance.ServerGroup = Environment.GetEnvironmentVariable("SERVER_GROUP") ?? "WingsEmu";
                int sessionLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("SERVER_SESSION_LIMIT") ?? "500");
                int? newChannelId = CommunicationServiceClient.Instance.RegisterWorldServer(new SerializableWorldServer
                {
                    Id = ServerManager.Instance.WorldId,
                    EndPointIp = ip,
                    EndPointPort = _port,
                    AccountLimit = sessionLimit,
                    WorldGroup = ServerManager.Instance.ServerGroup
                }, gRpcEndPoint);

                if (newChannelId.HasValue)
                {
                    ServerManager.Instance.ChannelId = newChannelId.Value;
                    ServerManager.Instance.IpAddress = ip;
                    ServerManager.Instance.Port = _port;
                    ServerManager.Instance.AccountLimit = sessionLimit;
                    Console.Title = string.Format(Language.Instance.GetMessageFromKey("WORLD_SERVER_CONSOLE_TITLE"), ServerManager.Instance.ChannelId, ServerManager.Instance.Sessions.Count(),
                        ServerManager.Instance.IpAddress, ServerManager.Instance.Port);
                }
                else
                {
                    server.Stop();
                    Logger.Log.Error("Could not retrieve ChannelId from Web API.", null);
                    Console.ReadKey();
                }

                while (!ServerManager.Instance.InShutdown)
                {
                    string tmp = Console.ReadLine();
                    if (tmp == "quit")
                    {
                        break;
                    }
                }
                server.Stop();
                gRpcServer.ShutdownAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        #endregion
    }
}