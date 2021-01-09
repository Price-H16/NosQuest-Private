// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Configuration
{
    public interface IConfigurationManager
    {
        /// <summary>
        ///     Loads the configuration from the given paths
        ///     <exception cref="System.IO.IOException"></exception>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T Load<T>(string path) where T : IConfiguration, new();

        /// <summary>
        ///     Loads the configuration from the given path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T Load<T>(string path, bool createIfNotExists) where T : IConfiguration, new();

        /// <summary>
        ///     Saves the configuration to a defined path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="value"></param>
        void Save<T>(string path, T value) where T : IConfiguration;
    }
}