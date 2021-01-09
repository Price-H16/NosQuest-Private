// WingsEmu
// 
// Developed by NosWings Team

namespace ChickenAPI.i18n
{
    /// <summary>
    /// Permits multi language within your logs
    /// </summary>
    public interface ILogLanguageService
    {
        /// <summary>
        ///     Will return the string by its key & region
        ///     Used for ChickenAPI mainly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetLanguage(LogI18NKeys key, RegionLanguageType type);

        /// <summary>
        ///     Will register the key and value by its region type
        ///     Used for ChickenAPI mainly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        void SetLanguage(LogI18NKeys key, string value, RegionLanguageType type);
    }
}