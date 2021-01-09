// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class ItemInstanceDAO : SynchronizableBaseDAO<ItemInstance, ItemInstanceDTO>, IItemInstanceDAO
    {
        private readonly IItemInstanceMappingTypes _mappingTypes;
        public ItemInstanceDAO(IMapper mapper,IItemInstanceMappingTypes mappingTypes) : base(mapper)
        {
            _mappingTypes = mappingTypes;
        }

        public DeleteResult DeleteFromSlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                ItemInstanceDTO dto = LoadBySlotAndType(characterId, slot, type);
                return Delete(dto.Id);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public interface IItemInstanceMappingTypes
        {
            List<(Type, Type)> Types { get; }
        }

        public IEnumerable<ItemInstanceDTO> LoadByCharacterId(long characterId)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                foreach (ItemInstance itemInstance in context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)))
                {
                    yield return _mapper.Map<ItemInstanceDTO>(itemInstance);
                }
            }
        }

        public ItemInstanceDTO LoadBySlotAndType(long characterId, short slot, InventoryType type)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    byte inventoryType = (byte)type;
                    byte equipmentType = (byte)slot;
                    ItemInstance entity = context.ItemInstance.FirstOrDefault(i => i.CharacterId == characterId && i.Slot == equipmentType && i.Type == inventoryType);
                    return _mapper.Map<ItemInstanceDTO>(entity);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<ItemInstanceDTO> LoadByType(long characterId, InventoryType type)
        {
            using (OpenNosContext context = DataAccessHelper.CreateContext())
            {
                byte inventoryType = (byte)type;
                foreach (ItemInstance itemInstance in context.ItemInstance.Where(i => i.CharacterId == characterId && i.Type == inventoryType))
                {
                    yield return _mapper.Map<ItemInstanceDTO>(itemInstance);
                }
            }
        }

        public IList<Guid> LoadSlotAndTypeByCharacterId(long characterId)
        {
            try
            {
                using (OpenNosContext context = DataAccessHelper.CreateContext())
                {
                    return context.ItemInstance.Where(i => i.CharacterId.Equals(characterId)).Select(i => i.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
        
        protected override ItemInstanceDTO InsertOrUpdate(OpenNosContext context, ItemInstanceDTO itemInstance)
        {
            try
            {
                ItemInstance entity = context.ItemInstance.FirstOrDefault(c => c.Id == itemInstance.Id);

                itemInstance = entity == null ? Insert(itemInstance, context) : Update(entity, itemInstance, context);
                context.SaveChanges();
                return itemInstance;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        protected override ItemInstance MapEntity(ItemInstanceDTO dto)
        {
            try
            {
                var entity = _mapper.Map<ItemInstance>(dto);
                (Type key, Type value) = _mappingTypes.Types.FirstOrDefault(k => k.Item1 == dto.GetType());
                if (key != null)
                {
                    entity = _mapper.Map(dto, key, value) as ItemInstance;
                }

                return entity;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
    }
}