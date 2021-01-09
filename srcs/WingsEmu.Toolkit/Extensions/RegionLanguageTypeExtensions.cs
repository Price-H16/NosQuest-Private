// WingsEmu
// 
// Developed by NosWings Team

using System;
using ChickenAPI.i18n;

namespace WingsEmu.Toolkit.Extensions
{
    public static class RegionLanguageTypeExtensions
    {
        public static string ToNostaleRegionKey(this RegionLanguageType type)
        {
            switch (type)
            {
                case RegionLanguageType.FR:
                    return "fr";
                case RegionLanguageType.EN:
                    return "uk";
                case RegionLanguageType.DE:
                    return "de";
                case RegionLanguageType.PL:
                    return "pl";
                case RegionLanguageType.IT:
                    return "it";
                case RegionLanguageType.ES:
                    return "es";
                case RegionLanguageType.CZ:
                    return "cz";
                case RegionLanguageType.TR:
                    return "tr";
                default:
                    return "uk";
            }
        }
    }
}