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
    public class RecipeDAO : MappingBaseDao<Recipe, RecipeDTO>, IRecipeDAO
    {
        public RecipeDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public RecipeDTO LoadByItemVNum(short itemVNum)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var dto = new RecipeDTO();
                    Recipe info = context.Recipe.SingleOrDefault(s => s.ItemVNum.Equals(itemVNum));
                    if (info == null)
                    {
                        return null;
                    }

                    dto.Amount = info.Amount;
                    dto.ItemVNum = info.ItemVNum;
                    dto.RecipeId = info.RecipeId;

                    return dto;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public RecipeDTO Insert(RecipeDTO recipe)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    var entity = _mapper.Map<Recipe>(recipe);
                    context.Recipe.Add(entity);
                    context.SaveChanges();
                    return _mapper.Map<RecipeDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Recipe Recipe in context.Recipe)
                {
                    yield return _mapper.Map<RecipeDTO>(Recipe);
                }
            }
        }

        public RecipeDTO LoadById(short recipeId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<RecipeDTO>(context.Recipe.FirstOrDefault(s => s.RecipeId.Equals(recipeId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<RecipeDTO> LoadByNpc(int npcId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Recipe Recipe in context.Recipe.Where(s => s.MapNpcId.Equals(npcId)))
                {
                    yield return _mapper.Map<RecipeDTO>(Recipe);
                }
            }
        }

        public void Update(RecipeDTO recipe)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Recipe result = context.Recipe.FirstOrDefault(c => c.MapNpcId == recipe.MapNpcId && c.ItemVNum == recipe.ItemVNum);
                    if (result != null)
                    {
                        recipe.RecipeId = result.RecipeId;
                        _mapper.Map(recipe, result);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}