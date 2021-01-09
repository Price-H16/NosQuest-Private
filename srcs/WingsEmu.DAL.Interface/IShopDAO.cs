// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IShopDAO : IMappingBaseDAO
    {
        #region Methods

        ShopDTO Insert(ShopDTO shop);

        void Insert(List<ShopDTO> shops);

        IEnumerable<ShopDTO> LoadAll();

        ShopDTO LoadById(int shopId);

        ShopDTO LoadByNpc(int mapNpcId);

        #endregion
    }
}