// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IItemDAO : IMappingBaseDAO
    {
        #region Methods

        IEnumerable<ItemDTO> FindByName(string name);

        ItemDTO Insert(ItemDTO item);

        void Insert(IEnumerable<ItemDTO> items);

        IEnumerable<ItemDTO> LoadAll();

        ItemDTO LoadById(short Vnum);

        #endregion
    }
}