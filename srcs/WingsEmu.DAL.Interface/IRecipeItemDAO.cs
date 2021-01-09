// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IRecipeItemDAO : IMappingBaseDAO
    {
        #region Methods

        RecipeItemDTO Insert(RecipeItemDTO recipeitem);

        IEnumerable<RecipeItemDTO> LoadAll();

        RecipeItemDTO LoadById(short RecipeItemId);

        IEnumerable<RecipeItemDTO> LoadByRecipe(short recipeId);

        IEnumerable<RecipeItemDTO> LoadByRecipeAndItem(short recipeId, short itemVNum);

        #endregion
    }
}