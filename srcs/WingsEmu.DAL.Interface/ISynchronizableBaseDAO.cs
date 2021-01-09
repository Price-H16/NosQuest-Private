// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.DTOs.Base;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ISynchronizableBaseDAO<TDTO> : IMappingBaseDAO
    where TDTO : SynchronizableBaseDTO
    {
        #region Methods

        DeleteResult Delete(Guid id);

        DeleteResult Delete(IEnumerable<Guid> ids);

        TDTO InsertOrUpdate(TDTO dto);

        IEnumerable<TDTO> InsertOrUpdate(IEnumerable<TDTO> dtos);

        TDTO LoadById(Guid id);

        #endregion
    }
}