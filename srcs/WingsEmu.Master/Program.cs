// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutoMapper;
using ChickenAPI.Plugins;
using ChickenAPI.Plugins.Exceptions;
using ChickenAPI.Plugins.Modules;
using dotenv.net;
using Grpc.Core;
using Grpc.Core.Logging;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using WingsEmu.Communication.RPC;
using WingsEmu.DAL.EF.DAO;
using WingsEmu.Plugins.DB.MSSQL;
using WingsEmu.Plugins.DB.MSSQL.Mapping;
using WingsEmu.Plugins.Logging;
using ILogger = ChickenAPI.Core.Logging.ILogger;

namespace WingsEmu.Master
{
    internal class Program
    {
        private static void PrintHeader()
        {
            Console.Title = "WingsEmu - Master";
            const string text = @"
 __      __.__                     ___________              
/  \    /  \__| ____    ____  _____\_   _____/ _____  __ __ 
\   \/\/   /  |/    \  / ___\/  ___/|    __)_ /     \|  |  \
 \        /|  |   |  \/ /_/  >___ \ |        \  Y Y  \  |  /
  \__/\  / |__|___|  /\___  /____  >_______  /__|_|  /____/ 
       \/          \//_____/     \/        \/      \/       
           _____                   __                       
          /     \ _____    _______/  |_  ___________        
         /  \ /  \\__  \  /  ___/\   __\/ __ \_  __ \       
        /    Y    \/ __ \_\___ \  |  | \  ___/|  | \/       
        \____|__  (____  /____  > |__|  \___  >__|          
                \/     \/     \/            \/              
";
            string separator = new string('=', Console.WindowWidth);
            string logo = text.Split('\n').Select(s => string.Format("{0," + (Console.WindowWidth / 2 + s.Length / 2) + "}\n", s))
                .Aggregate("", (current, i) => current + i);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(separator + logo + separator);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static IContainer InitializePlugins()
        {
            var pluginBuilder = new ContainerBuilder();
            pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf();
            IContainer container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            coreBuilder.RegisterAssemblyTypes(typeof(Program).Assembly).AsSelf().AsImplementedInterfaces().SingleInstance();
            coreBuilder.RegisterAssemblyTypes(typeof(MasterCommunicator).Assembly).AsSelf().AsImplementedInterfaces().SingleInstance();
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

        private static void Main()
        {
            PrintHeader();
            DotEnv.Config(false, "server.env");
            using (IContainer coreContainer = InitializePlugins())
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
                // initialize DB
                
                if (!DataAccessHelper.Initialize(coreContainer.Resolve<IOpenNosContextFactory>()))
                {
                    Console.ReadLine();
                    return;
                }
                DaoFactory.Initialize(coreContainer.Resolve<DaoFactory>());

                Logger.Log.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                int port = Convert.ToInt32(Environment.GetEnvironmentVariable("MASTER_PORT") ?? "20500");
                string ip = Environment.GetEnvironmentVariable("MASTER_IP") ?? "localhost";

                var server = new Server
                {
                    Services = { global::Master.BindService(coreContainer.Resolve<MasterImpl>()) },
                    Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
                };
                Logger.Log.Info($"[RPC-SERVER] Listening on {ip}:{port}");
                server.Start();

                Console.Title = $"WingsEmu - Master | {ip}:{port}";

                for (;;)
                {
                    string line = Console.ReadLine();
                    if (line == string.Empty || line == "quit")
                    {
                        break;
                    }
                }

                server.ShutdownAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}