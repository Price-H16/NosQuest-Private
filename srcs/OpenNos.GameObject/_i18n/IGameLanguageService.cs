// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using ChickenAPI.i18n;

namespace OpenNos.GameObject._i18n
{
    public interface IGameLanguageService
    {
        /// <summary>
        ///     Will return the string by its Key & <see cref="RegionLanguageType"/>
        ///     Used for plugins mainly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        string GetLanguage(string key, RegionLanguageType lang);

        /// <summary>
        ///     Will return the string by its key & <see cref="RegionLanguageType"/>
        ///     Used for ChickenAPI mainly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        string GetLanguage(GameDialogKey key, RegionLanguageType lang);

        /// <summary>
        ///     Will register the key and value by its <see cref="RegionLanguageType"/>
        ///     Used for plugins mainly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lang"></param>
        void SetLanguage(string key, string value, RegionLanguageType lang);

        /// <summary>
        ///     Will register the key and value by its <see cref="RegionLanguageType"/>
        ///     Used for GameDialogs mainly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lang"></param>
        void SetLanguage(GameDialogKey key, string value, RegionLanguageType lang);
    }
}