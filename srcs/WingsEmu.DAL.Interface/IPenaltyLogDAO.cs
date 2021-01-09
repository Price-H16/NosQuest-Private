// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IPenaltyLogDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(int penaltylogId);

        SaveResult InsertOrUpdate(ref PenaltyLogDTO log);

        IEnumerable<PenaltyLogDTO> LoadAll();

        IEnumerable<PenaltyLogDTO> LoadByAccount(long accountId);

        PenaltyLogDTO LoadById(int relId);

        #endregion
    }
}