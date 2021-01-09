// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Items.Instance;
using OpenNos.GameObject.Maps;

namespace OpenNos.GameObject
{
    public class CharacterMapItem : MapItem
    {
        #region Instantiation

        public CharacterMapItem(short x, short y, ItemInstance itemInstance) : base(x, y) => ItemInstance = itemInstance;

        #endregion

        #region Methods

        public override ItemInstance GetItemInstance() => ItemInstance;

        #endregion

        #region Properties

        public override ushort Amount
        {
            get => ItemInstance.Amount;

            set => ItemInstance.Amount = Amount;
        }

        public override short ItemVNum
        {
            get => ItemInstance.ItemVNum;

            set => ItemInstance.ItemVNum = value;
        }

        public override long TransportId
        {
            get => base.TransportId;

            set
            {
                // cannot set TransportId
            }
        }

        #endregion
    }
}