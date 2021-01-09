// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;

namespace OpenNos.DAL.EF.Entities
{
    public class Recipe
    {
        #region Instantiation

        public Recipe() => RecipeItem = new HashSet<RecipeItem>();

        #endregion

        #region Properties

        public byte Amount { get; set; }

        public virtual Item Item { get; set; }

        public short ItemVNum { get; set; }

        public virtual MapNpc MapNpc { get; set; }

        public int MapNpcId { get; set; }

        public short RecipeId { get; set; }

        public short ProduceItemVNum { get; set; }

        public virtual ICollection<RecipeItem> RecipeItem { get; set; }

        #endregion
    }
}