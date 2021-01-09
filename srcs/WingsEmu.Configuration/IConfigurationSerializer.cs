// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Configuration
{
    public interface IConfigurationSerializer
    {
        string Serialize<T>(T conf) where T : IConfiguration;

        T Deserialize<T>(string buffer) where T : IConfiguration;
    }
}