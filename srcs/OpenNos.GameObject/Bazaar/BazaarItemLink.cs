// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Items.Instance;
using WingsEmu.DTOs;

namespace OpenNos.GameObject.Bazaar
{
    public class BazaarItemLink
    {
        #region Properties

        public BazaarItemDTO BazaarItem { get; set; }

        public ItemInstance Item { get; set; }

        public string Owner { get; set; }

        #endregion
    }
}