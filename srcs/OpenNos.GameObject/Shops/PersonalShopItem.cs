// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Items.Instance;

namespace OpenNos.GameObject.Shops
{
    public class PersonalShopItem
    {
        #region Properties

        public ItemInstance ItemInstance { get; set; }

        public long Price { get; set; }

        public short SellAmount { get; set; }

        public short ShopSlot { get; set; }

        #endregion
    }
}