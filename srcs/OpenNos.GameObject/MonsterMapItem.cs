// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Maps;
using OpenNos.GameObject.Networking;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject
{
    public class MonsterMapItem : MapItem
    {
        #region Instantiation

        public MonsterMapItem(short x, short y, short itemVNum, int amount = 1, long ownerId = -1) : base(x, y)
        {
            ItemVNum = itemVNum;
            if (amount < 1000)
            {
                Amount = (byte)amount;
            }

            GoldAmount = amount;
            OwnerId = ownerId;
        }

        #endregion

        #region Properties

        public sealed override ushort Amount { get; set; }

        public int GoldAmount { get; }

        public sealed override short ItemVNum { get; set; }

        public long? OwnerId { get; }

        #endregion

        #region Methods

        public override ItemInstance GetItemInstance()
        {
            if (ItemInstance == null && OwnerId != null)
            {
                ItemInstance = Inventory.InstantiateItemInstance(ItemVNum, OwnerId.Value, Amount);
            }

            return ItemInstance;
        }

        public void Rarify(ClientSession session)
        {
            ItemInstance instance = GetItemInstance();
            if (instance.Item.Type != InventoryType.Equipment ||
                instance.Item.ItemType != ItemType.Weapon && instance.Item.ItemType != ItemType.Armor)
            {
                return;
            }

            if (instance is WearableInstance wearableInstance)
            {
                wearableInstance?.RarifyItem(session, RarifyMode.Drop, RarifyProtection.None);
                wearableInstance.Upgrade = instance.Item.BasicUpgrade;
            }
        }

        #endregion
    }
}