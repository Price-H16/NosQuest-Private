// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IBCardDAO : IMappingBaseDAO
    {
        #region Methods
        
        void Insert(List<BCardDTO> card);
        IEnumerable<BCardDTO> LoadAll();

        IEnumerable<BCardDTO> LoadByCardId(short cardId);

        IEnumerable<BCardDTO> LoadByItemVNum(short vNum);

        IEnumerable<BCardDTO> LoadByNpcMonsterVNum(short vNum);

        IEnumerable<BCardDTO> LoadBySkillVNum(short vNum);

        void Clean();

        #endregion
    }
}