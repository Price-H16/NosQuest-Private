// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.DAL.EF.DB;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs.Base;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.EF.DAO.DAOs.Data
{
    public class RecipeItemDAO : MappingBaseDao<RecipeItem, RecipeItemDTO>, IRecipeItemDAO
    {
        public RecipeItemDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public RecipeItemDTO Insert(RecipeItemDTO recipeItem)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<RecipeItem>(recipeItem);
                    context.RecipeItem.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<RecipeItemDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeItemDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RecipeItem recipeItem in context.RecipeItem)
                {
                    yield return _mapper.Map<RecipeItemDTO>(recipeItem);
                }
            }
        }

        public RecipeItemDTO LoadById(short recipeItemId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RecipeItemDTO>(context.RecipeItem.FirstOrDefault(s => s.RecipeItemId.Equals(recipeItemId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeItemDTO> LoadByRecipe(short recipeId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RecipeItem recipeItem in context.RecipeItem.Where(s => s.RecipeId.Equals(recipeId)))
                {
                    yield return _mapper.Map<RecipeItemDTO>(recipeItem);
                }
            }
        }

        public IEnumerable<RecipeItemDTO> LoadByRecipeAndItem(short recipeId, short itemVNum)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (RecipeItem recipeItem in context.RecipeItem.Where(s => s.ItemVNum.Equals(itemVNum) && s.RecipeId.Equals(recipeId)))
                {
                    yield return _mapper.Map<RecipeItemDTO>(recipeItem);
                }
            }
        }

        #endregion
    }
}