// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace WingsEmu.World.Configuration
{
    public class ConfigurationHelper
    {
        public static T Load<T>(string path) where T : class, new() => Load<T>(path, false);

        public static T Load<T>(string path, bool createIfNotExists) where T : class, new()
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

            return JsonConvert.DeserializeObject<T>(fileContent, _settings);
        }

        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(new SnakeCaseNamingStrategy())
            },
            Formatting = Formatting.Indented
        };

        public static void Save<T>(string path, T value)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            string valueSerialized = JsonConvert.SerializeObject(value, _settings);

            File.WriteAllText(path, valueSerialized);
        }
    }
}