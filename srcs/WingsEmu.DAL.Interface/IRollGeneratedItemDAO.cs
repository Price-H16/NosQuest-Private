// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IRollGeneratedItemDAO : IMappingBaseDAO
    {
        #region Methods

        IEnumerable<RollGeneratedItemDTO> LoadAll();

        IEnumerable<RollGeneratedItemDTO> LoadByItemVNum(short vnum);

        #endregion
    }
}