// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IRecipeDAO : IMappingBaseDAO
    {
        #region Methods

        RecipeDTO Insert(RecipeDTO recipe);

        IEnumerable<RecipeDTO> LoadAll();

        RecipeDTO LoadById(short RecipeId);

        IEnumerable<RecipeDTO> LoadByNpc(int mapNpcId);

        void Update(RecipeDTO recipe);

        RecipeDTO LoadByItemVNum(short itemVNum);

        #endregion
    }
}