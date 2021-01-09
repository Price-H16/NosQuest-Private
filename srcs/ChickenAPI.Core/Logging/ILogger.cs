// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace ChickenAPI.Core.Logging
{
    public interface ILogger
    {
        void Debug(string msg);
        void DebugFormat(string msg, params object[] objs);

        void Info(string msg);
        void InfoFormat(string msg, params object[] objs);

        void Warn(string msg);
        void WarnFormat(string msg, params object[] objs);

        void Error(string msg, Exception ex);
        void ErrorFormat(string msg, Exception ex, params object[] objs);

        void Fatal(string msg, Exception ex);
    }
}