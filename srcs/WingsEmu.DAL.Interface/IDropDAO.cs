// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IDropDAO : IMappingBaseDAO
    {
        #region Methods

        DropDTO Insert(DropDTO drop);

        void Insert(List<DropDTO> drops);

        List<DropDTO> LoadAll();

        IEnumerable<DropDTO> LoadByMonster(short monsterVNum);

        #endregion
    }
}