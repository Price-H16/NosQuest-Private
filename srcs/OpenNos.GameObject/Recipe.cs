// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace OpenNos.GameObject
{
    public class Recipe : RecipeDTO
    {
        #region Properties

        public List<RecipeItemDTO> Items { get; set; }

        #endregion
    }
}