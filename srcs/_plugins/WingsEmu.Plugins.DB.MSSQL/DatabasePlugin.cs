// WingsEmu
// 
// Developed by NosWings Team

using Autofac;
using AutoMapper;
using ChickenAPI.Core.Logging;
using ChickenAPI.Core.Utils;
using ChickenAPI.Plugins;
using OpenNos.DAL;
using OpenNos.DAL.EF.DB;
using WingsEmu.DAL.EF.DAO;
using WingsEmu.DAL.EF.DAO.DAOs;
using WingsEmu.DAL.Interface;
using WingsEmu.Plugins.DB.MSSQL.Mapping;

namespace WingsEmu.Plugins.DB.MSSQL
{
    public class DatabasePlugin : ICorePlugin
    {
        private readonly ILogger _log;

        public DatabasePlugin(ILogger log)
        {
            _log = log;
        }

        public string Name => nameof(DatabasePlugin);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            _log.Info("Registering DAL.EF objects");
            builder.RegisterTypes(typeof(AccountDAO).Assembly.GetTypesImplementingInterface<IMappingBaseDAO>()).AsImplementedInterfaces().AsSelf();
            builder.RegisterType<DbContextFactory>().AsImplementedInterfaces().AsSelf();
            builder.RegisterType<DatabaseConfiguration>().AsImplementedInterfaces().AsSelf();
            _log.Info("Registering DAL objects");
            builder.RegisterType(typeof(DaoFactory)).AsSelf();
            _log.Info("Registering Mapping objects");
            builder.Register(_ => new WingsEmuItemInstanceMappingType()).As<ItemInstanceDAO.IItemInstanceMappingTypes>();
            _log.Info("Registering DAL.EF.DAO objects");
            builder.RegisterTypes(typeof(OpenNosContext).Assembly.GetTypes()).AsSelf().AsImplementedInterfaces().SingleInstance();
        }
    }
}