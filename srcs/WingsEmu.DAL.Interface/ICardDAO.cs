// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface ICardDAO : IMappingBaseDAO
    {
        #region Methods


        void Insert(List<CardDTO> card);

        IEnumerable<CardDTO> LoadAll();

        CardDTO LoadById(short cardId);

        #endregion
    }
}