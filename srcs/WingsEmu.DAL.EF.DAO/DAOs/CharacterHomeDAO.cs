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
    public class CharacterHomeDAO : SynchronizableBaseDAO<CharacterHome, CharacterHomeDTO>, ICharacterHomeDAO
    {
        public CharacterHomeDAO(IMapper mapper) : base(mapper)
        {
        }

        public IEnumerable<CharacterHomeDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (CharacterHome home in context.CharacterHome)
                {
                    yield return _mapper.Map<CharacterHomeDTO>(home);
                }
            }
        }

        public DeleteResult DeleteByName(long characterId, string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    CharacterHome entity = context.CharacterHome.FirstOrDefault(s => s.CharacterId == characterId && s.Name == name);
                    if (entity != null)
                    {
                        context.CharacterHome.Remove(entity);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error("CharacterHome DeleteByName", e);
                return DeleteResult.Error;
            }
        }

        public IEnumerable<CharacterHomeDTO> LoadByCharacterId(long id)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (CharacterHome home in context.CharacterHome.Where(s => s.CharacterId == id))
                {
                    yield return _mapper.Map<CharacterHomeDTO>(home);
                }
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterHomeDTO dto)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                SaveResult tmp = InsertOrUpdate(ref dto, context);
                context.SaveChanges();
                return tmp;
            }
        }

        public SaveResult InsertOrUpdate(ref CharacterHomeDTO dto, OpenNosContext context)
        {
            try
            {
                Guid homeDtoId = dto.Id;
                CharacterHome entity = context.CharacterHome.FirstOrDefault(c => c.Id.Equals(homeDtoId));

                if (entity == null)
                {
                    dto = Insert(dto, context);
                    return SaveResult.Inserted;
                }

                dto = Update(entity, dto, context);
                return SaveResult.Updated;
            }
            catch (Exception e)
            {
                return SaveResult.Error;
            }
        }
    }
}