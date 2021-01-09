// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;

namespace OpenNos.GameObject.Shops
{
    public class Shop : ShopDTO
    {
        #region Properties

        public List<ShopItemDTO> ShopItems { get; set; }

        public List<ShopSkillDTO> ShopSkills { get; set; }

        #endregion

        #region Methods

        public override void Initialize()
        {
            ShopItems = ServerManager.Instance.GetShopItemsByShopId(ShopId);
            ShopSkills = ServerManager.Instance.GetShopSkillsByShopId(ShopId);
        }
        
        #endregion
    }
}