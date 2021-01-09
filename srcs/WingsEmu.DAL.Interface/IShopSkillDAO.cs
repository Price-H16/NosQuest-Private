// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IShopSkillDAO : IMappingBaseDAO
    {
        #region Methods

        ShopSkillDTO Insert(ShopSkillDTO shopskill);

        void Insert(List<ShopSkillDTO> skills);

        IEnumerable<ShopSkillDTO> LoadAll();

        IEnumerable<ShopSkillDTO> LoadByShopId(int ShopId);

        #endregion
    }
}