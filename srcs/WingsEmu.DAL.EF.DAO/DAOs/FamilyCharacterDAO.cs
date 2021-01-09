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
using WingsEmu.DTOs.Enums;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class FamilyCharacterDAO : MappingBaseDao<FamilyCharacter, FamilyCharacterDTO>, IFamilyCharacterDAO
    {
        public FamilyCharacterDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(string characterName)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Character character = context.Character.FirstOrDefault(c => c.Name.Equals(characterName) && c.State == (byte)CharacterState.Active);
                    FamilyCharacter familyCharacter = context.FamilyCharacter.FirstOrDefault(c => c.CharacterId.Equals(character.CharacterId));
                    if (character != null && familyCharacter != null)
                    {
                        context.FamilyCharacter.Remove(familyCharacter);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_FAMILYCHARACTER_ERROR"), e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilyCharacterDTO character)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long familyCharacterId = character.FamilyCharacterId;
                    FamilyCharacter entity = context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId));

                    if (entity == null)
                    {
                        character = Insert(character, context);
                        return SaveResult.Inserted;
                    }

                    character = Update(entity, character, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("INSERT_ERROR"), character, e.Message), e);
                return SaveResult.Error;
            }
        }

        public FamilyCharacterDTO LoadByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<FamilyCharacterDTO>(context.FamilyCharacter.FirstOrDefault(c => c.CharacterId == characterId));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IList<FamilyCharacterDTO> LoadByFamilyId(long familyId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.FamilyCharacter.Where(fc => fc.FamilyId.Equals(familyId)).ToList().Select(c => _mapper.Map<FamilyCharacterDTO>(c)).ToList();
            }
        }

        public FamilyCharacterDTO LoadById(long familyCharacterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<FamilyCharacterDTO>(context.FamilyCharacter.FirstOrDefault(c => c.FamilyCharacterId.Equals(familyCharacterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private FamilyCharacterDTO Insert(FamilyCharacterDTO character, OpenNosContext context)
        {
            var entity = _mapper.Map<FamilyCharacter>(character);
            context.FamilyCharacter.Add(entity);
            context.SaveChanges();
            return _mapper.Map<FamilyCharacterDTO>(entity);
        }

        private FamilyCharacterDTO Update(FamilyCharacter entity, FamilyCharacterDTO character, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(character, entity);
                context.SaveChanges();
            }

            return _mapper.Map<FamilyCharacterDTO>(entity);
        }

        #endregion
    }
}