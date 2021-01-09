// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IShopItemDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult DeleteById(int ItemId);

        ShopItemDTO Insert(ShopItemDTO item);

        void Insert(List<ShopItemDTO> items);

        IEnumerable<ShopItemDTO> LoadAll();

        ShopItemDTO LoadById(int ItemId);

        IEnumerable<ShopItemDTO> LoadByShopId(int ShopId);

        #endregion
    }
}