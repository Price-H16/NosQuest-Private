// WingsEmu
// 
// Developed by NosWings Team

using System;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL.EF.DB;

namespace WingsEmu.DAL.EF.DAO
{
    public interface IOpenNosContextFactory
    {
        OpenNosContext CreateContext();
    }

    public static class DataAccessHelper
    {
        private static IOpenNosContextFactory _contextFactory;


        /// <summary>
        ///     Creates new instance of database context.
        /// </summary>
        public static OpenNosContext CreateContext() => _contextFactory.CreateContext();


        public static bool Initialize(IOpenNosContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            using (OpenNosContext context = CreateContext())
            {
                try
                {
                    context.Database.EnsureCreated();
                    Logger.Log.Info(Language.Instance.GetMessageFromKey("DATABASE_INITIALIZED"));
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(Language.Instance.GetMessageFromKey("DATABASE_NOT_UPTODATE"), ex);
                    return false;
                }

                return true;
            }
        }
    }
}