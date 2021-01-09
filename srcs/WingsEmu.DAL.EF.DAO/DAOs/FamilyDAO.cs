﻿// WingsEmu
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

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class FamilyDAO : MappingBaseDao<Family, FamilyDTO>, IFamilyDAO
    {
        public FamilyDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public DeleteResult Delete(long familyId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Family Fam = context.Family.FirstOrDefault(c => c.FamilyId == familyId);

                    if (Fam != null)
                    {
                        context.Family.Remove(Fam);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("DELETE_ERROR"), familyId, e.Message), e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref FamilyDTO family)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    long AccountId = family.FamilyId;
                    Family entity = context.Family.FirstOrDefault(c => c.FamilyId.Equals(AccountId));

                    if (entity == null)
                    {
                        family = Insert(family, context);
                        return SaveResult.Inserted;
                    }

                    family = Update(entity, family, context);
                    context.SaveChanges();
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(string.Format(Language.Instance.GetMessageFromKey("UPDATE_FAMILY_ERROR"), family.FamilyId, e.Message), e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<FamilyDTO> LoadAll()
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (Family entity in context.Family)
                {
                    yield return _mapper.Map<FamilyDTO>(entity);
                }
            }
        }

        public FamilyDTO LoadByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    FamilyCharacter familyCharacter = context.FamilyCharacter.FirstOrDefault(fc => fc.Character.CharacterId.Equals(characterId));
                    if (familyCharacter != null)
                    {
                        Family family = context.Family.FirstOrDefault(a => a.FamilyId.Equals(familyCharacter.FamilyId));
                        if (family != null)
                        {
                            return _mapper.Map<FamilyDTO>(family);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        public FamilyDTO LoadById(long familyId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Family family = context.Family.FirstOrDefault(a => a.FamilyId.Equals(familyId));
                    if (family != null)
                    {
                        return _mapper.Map<FamilyDTO>(family);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        public FamilyDTO LoadByName(string name)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Family family = context.Family.FirstOrDefault(a => a.Name.Equals(name));
                    if (family != null)
                    {
                        return _mapper.Map<FamilyDTO>(family);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return null;
        }

        private FamilyDTO Insert(FamilyDTO family, OpenNosContext context)
        {
            var entity = _mapper.Map<Family>(family);
            context.Family.Add(entity);
            context.SaveChanges();
            return _mapper.Map<FamilyDTO>(entity);
        }

        private FamilyDTO Update(Family entity, FamilyDTO family, OpenNosContext context)
        {
            if (entity != null)
            {
                _mapper.Map(family, entity);
                context.SaveChanges();
            }

            return _mapper.Map<FamilyDTO>(entity);
        }

        #endregion
    }
}