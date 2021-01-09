// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Runtime.CompilerServices;
using ChickenAPI.Core.Logging;

namespace OpenNos.Core.Logging
{
    public class Logger
    {
        #region Properties

        public static ILogger Log { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Wraps up the error message with the CallerMemberName
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="innerException"></param>
        public static void Error(Exception innerException = null, [CallerMemberName] string memberName = "")
        {
            if (innerException != null)
            {
                Log?.Error($"{memberName}: {innerException.Message}", innerException);
            }
        }

        public static void InitializeLogger(ILogger log)
        {
            Log = log;
        }

        #endregion
    }
}