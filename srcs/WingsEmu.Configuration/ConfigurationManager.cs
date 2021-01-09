// WingsEmu
// 
// Developed by NosWings Team

using System.IO;

namespace WingsEmu.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfigurationSerializer _serializer;

        public ConfigurationManager(IConfigurationSerializer serializer) => _serializer = serializer;

        public T Load<T>(string path) where T : IConfiguration, new() => Load<T>(path, false);

        public T Load<T>(string path, bool createIfNotExists) where T : IConfiguration, new()
        {
            if (!File.Exists(path))
            {
                if (createIfNotExists)
                {
                    Save(path, new T());
                }
                else
                {
                    throw new IOException(path);
                }
            }

            string fileContent = File.ReadAllText(path);


            return _serializer.Deserialize<T>(fileContent);
        }

        public void Save<T>(string path, T value) where T : IConfiguration
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            string valueSerialized = _serializer.Serialize(value);

            File.WriteAllText(path, valueSerialized);
        }
    }
}