// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using OpenNos.DAL.EF.DB;
using OpenNos.DAL.EF.Entities;
using WingsEmu.DAL.EF.DAO.DAOs.Base;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.EF.DAO.DAOs
{
    public class EquipmentOptionDAO : SynchronizableBaseDAO<EquipmentOption, EquipmentOptionDTO>, IEquipmentOptionDAO
    {
        public EquipmentOptionDAO(IMapper mapper) : base(mapper)
        {
        }

        #region Methods

        public SaveResult InsertOrUpdate(ref EquipmentOptionDTO equipmentOption)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    Guid id = equipmentOption.Id;
                    EquipmentOption entity = context.EquipmentOption.FirstOrDefault(c => c.Id.Equals(id));

                    if (entity == null)
                    {
                        equipmentOption = Insert(equipmentOption, context);
                        return SaveResult.Inserted;
                    }

                    equipmentOption = Update(entity, equipmentOption, context);
                    context.SaveChanges();
                    return SaveResult.Updated;
                }
            }
            catch (Exception)
            {
                return SaveResult.Error;
            }
        }

        public DeleteResult DeleteByWearableInstanceId(Guid wearableInstanceId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    foreach (EquipmentOption equipmentOption in context.EquipmentOption.Where(
                        i => i.WearableInstanceId.Equals(wearableInstanceId)))
                    {
                        if (equipmentOption != null)
                        {
                            context.EquipmentOption.Remove(equipmentOption);
                        }
                    }

                    context.SaveChanges();
                    return DeleteResult.Deleted;
                }
            }
            catch (Exception)
            {
                return DeleteResult.Error;
            }
        }

        public IEnumerable<EquipmentOptionDTO> GetOptionsByWearableInstanceId(Guid wearableInstanceId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (EquipmentOption cellonOptionobject in context.EquipmentOption.Where(i => i.WearableInstanceId.Equals(wearableInstanceId)))
                {
                    yield return _mapper.Map<EquipmentOptionDTO>(cellonOptionobject);
                }
            }
        }

        #endregion
    }
}