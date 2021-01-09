// WingsEmu
// 
// Developed by NosWings Team

using System.IO;
using System.Reflection;

namespace OpenNos.Core
{
    public class Application
    {
        #region Methods

        public static string AppPath(bool backSlash = true)
        {
            string text = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            if (backSlash)
            {
                text += @"\";
            }

            return text;
        }

        #endregion
    }
}