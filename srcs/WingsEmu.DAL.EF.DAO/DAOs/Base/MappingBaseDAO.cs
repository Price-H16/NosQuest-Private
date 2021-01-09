// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using AutoMapper;
using WingsEmu.DAL.Interface;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DAL.EF.DAO.DAOs.Base
{
    public abstract class MappingBaseDao<TEntity, TDto> : IMappingBaseDAO
    where TDto : MappingBaseDTO
    {
        #region Members

        protected readonly IDictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        protected readonly IMapper _mapper;

        protected MappingBaseDao(IMapper mapper) => _mapper = mapper;

        #endregion

    }
}