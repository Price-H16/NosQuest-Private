// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IComboDAO : IMappingBaseDAO
    {
        #region Methods

        ComboDTO Insert(ComboDTO combo);

        void Insert(List<ComboDTO> combos);

        IEnumerable<ComboDTO> LoadAll();

        ComboDTO LoadById(short ComboId);

        IEnumerable<ComboDTO> LoadBySkillVnum(short skillVNum);

        IEnumerable<ComboDTO> LoadByVNumHitAndEffect(short skillVNum, short hit, short effect);

        #endregion
    }
}