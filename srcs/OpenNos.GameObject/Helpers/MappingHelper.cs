// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;

namespace OpenNos.GameObject.Helpers
{
    public class MappingHelper
    {
        #region Properties

        public Dictionary<int, int> GuriItemEffects = new Dictionary<int, int>
        {
            { 859, 1343 },
            { 860, 1344 },
            { 861, 1344 },
            { 875, 1558 },
            { 876, 1559 },
            { 877, 1560 },
            { 878, 1560 },
            { 879, 1561 },
            { 880, 1561 }
        };

        #endregion

        #region Singleton

        private static MappingHelper _instance;

        public static MappingHelper Instance => _instance ?? (_instance = new MappingHelper());

        #endregion
    }
}