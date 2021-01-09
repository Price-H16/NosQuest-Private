// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IBazaarItemDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(long bazaarItemId);

        SaveResult InsertOrUpdate(ref BazaarItemDTO bazaarItem);

        IEnumerable<BazaarItemDTO> LoadAll();

        BazaarItemDTO LoadById(long bazaarItemId);

        void RemoveOutDated();

        #endregion
    }
}