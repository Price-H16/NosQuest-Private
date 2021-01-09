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
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class CharacterDAO : MappingBaseDao<Character, CharacterDTO>, ICharacterDAO
    {
        public CharacterDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult DeleteByPrimaryKey(long accountId, byte characterSlot)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    // actually a Character wont be deleted, it just will be disabled for future traces
                    Character character = context.Character.SingleOrDefault(c => c.AccountId.Equals(accountId) && c.Slot.Equals(characterSlot) && c.State.Equals((byte)CharacterState.Active));

                    if (character == null)
                    {
                        return DeleteResult.Deleted;
                    }

                    character.State = (byte)CharacterState.Inactive;
                    Update(character, _mapper.Map<CharacterDTO>(character), context);

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_CHARACTER_ERROR"), characterSlot, e.Message), e);
                return DeleteResult.Error;
            }
        }

        /// <summary>
        ///     Returns first 30 occurences of highest Compliment
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopCompliment()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.Account.Authority <= AuthorityType.Moderator).OrderByDescending(c => c.Compliment).Take(30).ToList().Select(c => _mapper.Map<CharacterDTO>(c))
                    .ToList();
            }
        }

        /// <summary>
        ///     Returns first 30 occurences of highest Act4Points
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopPoints()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.Account.Authority <= AuthorityType.Moderator).OrderByDescending(c => c.Act4Points).Take(30).ToList().Select(c => _mapper.Map<CharacterDTO>(c))
                    .ToList();
            }
        }

        /// <summary>
        ///     Returns first 30 occurences of highest Reputation
        /// </summary>
        /// <returns></returns>
        public List<CharacterDTO> GetTopReputation()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.Account.Authority <= AuthorityType.Moderator).OrderByDescending(c => c.Reput).Take(43).ToList().Select(c => _mapper.Map<CharacterDTO>(c))
                    .ToList();
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterDTO character)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long characterId = character.CharacterId;
                    Character entity = context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId));

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

        [Obsolete("LoadAll is obsolete, create a separate DAO statement for your function")]
        public IEnumerable<CharacterDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Character chara in context.Character)
                {
                    yield return _mapper.Map<CharacterDTO>(chara);
                }
            }
        }

        public IEnumerable<CharacterDTO> LoadByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.AccountId.Equals(accountId) && c.State.Equals((byte)CharacterState.Active)).OrderByDescending(c => c.Slot).ToList()
                    .Select(c => _mapper.Map<CharacterDTO>(c)).ToList();
            }
        }

        public IEnumerable<CharacterDTO> LoadAllCharactersByAccount(long accountId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                return context.Character.Where(c => c.AccountId.Equals(accountId)).OrderByDescending(c => c.Slot).ToList().Select(c => _mapper.Map<CharacterDTO>(c)).ToList();
            }
        }

        public CharacterDTO LoadById(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CharacterDTO>(context.Character.FirstOrDefault(c => c.CharacterId.Equals(characterId)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public CharacterDTO LoadByName(string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CharacterDTO>(context.Character.SingleOrDefault(c => c.Name.Equals(name) && c.State.Equals((byte)CharacterState.Active)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        public CharacterDTO LoadBySlot(long accountId, byte slot)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return _mapper.Map<CharacterDTO>(context.Character.SingleOrDefault(c => c.AccountId.Equals(accountId) && c.Slot.Equals(slot) && c.State.Equals((byte)CharacterState.Active)));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private CharacterDTO Insert(CharacterDTO character, OpenNosContext context)
        {
            var entity = _mapper.Map<Character>(character);
            context.Character.Add(entity);
            context.SaveChanges();
            return _mapper.Map<CharacterDTO>(entity);
        }

        private CharacterDTO Update(Character entity, CharacterDTO character, OpenNosContext context)
        {
            if (entity == null)
            {
                return null;
            }

            _mapper.Map(character, entity);
            context.SaveChanges();

            return _mapper.Map<CharacterDTO>(entity);
        }

        #endregion
    }
}