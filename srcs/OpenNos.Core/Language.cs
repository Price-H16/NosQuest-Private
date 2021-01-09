// WingsEmu
// 
// Developed by NosWings Team

namespace OpenNos.Core
{
    public class Language
    {
        private static Language _instance;
        public static Language Instance => _instance ?? (_instance = new Language());

        public string GetMessageFromKey(string message) => $"#<{message}>";
    }
}