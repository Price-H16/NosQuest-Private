// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using OpenNos.GameObject.Shops;

namespace OpenNos.GameObject.Maps
{
    public class MapShop
    {
        #region Instantiation

        public MapShop()
        {
            Items = new List<PersonalShopItem>();
            Sell = 0;
        }

        #endregion

        #region Properties

        public List<PersonalShopItem> Items { get; set; }

        public string Name { get; set; }

        public long OwnerId { get; set; }

        public long Sell { get; set; }

        #endregion
    }
}