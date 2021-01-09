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
using WingsEmu.DTOs.Character;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class CharacterRelationDAO : MappingBaseDao<CharacterRelation, CharacterRelationDTO>, ICharacterRelationDAO
    {
        public CharacterRelationDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long id)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterRelation relation = context.CharacterRelation.SingleOrDefault(c => c.CharacterRelationId.Equals(id));

                    if (relation != null)
                    {
                        context.CharacterRelation.Remove(relation);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_CHARACTER_ERROR"), id, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterRelationDTO relation)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long characterId = relation.CharacterId;
                    long relatedCharacterId = relation.RelatedCharacterId;
                    CharacterRelation entity = context.CharacterRelation.FirstOrDefault(c => c.CharacterId.Equals(characterId) && c.RelatedCharacterId.Equals(relatedCharacterId));

                    if (entity == null)
                    {
                        relation = Insert(relation, context);
                        return SaveResult.Inserted;
                    }

                    relation = Update(entity, relation, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_CHARACTERRELATION_ERROR"), relation.CharacterRelationId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<CharacterRelationDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (CharacterRelation entity in context.CharacterRelation)
                {
                    yield return _mapper.Map<CharacterRelationDTO>(entity);
                }
            }
        }

        public CharacterRelationDTO LoadById(long relId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CharacterRelationDTO>(context.CharacterRelation.FirstOrDefault(s => s.CharacterRelationId.Equals(relId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private CharacterRelationDTO Insert(CharacterRelationDTO relation, OpenNosContext context)
        {
            var entity = _mapper.Map<CharacterRelation>(relation);
            context.CharacterRelation.Add(entity);
            context.SaveChanges();
            return _mapper.Map<CharacterRelationDTO>(entity);
        }

        private CharacterRelationDTO Update(CharacterRelation entity, CharacterRelationDTO relation, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(relation, entity);
                context.SaveChanges();
            }

            return _mapper.Map<CharacterRelationDTO>(entity);
        }

        #endregion
    }
}