// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IGeneralLogDAO : IMappingBaseDAO
    {
        #region Methods

        bool IdAlreadySet(long id);

        GeneralLogDTO Insert(GeneralLogDTO generallog);

        IEnumerable<GeneralLogDTO> LoadAll();

        IEnumerable<GeneralLogDTO> LoadByAccount(long? accountId);

        IEnumerable<GeneralLogDTO> LoadByLogType(string logType, long? characterId);

        void SetCharIdNull(long? characterId);

        void WriteGeneralLog(long accountId, string ipAddress, long? characterId, string logType, string logData);

        #endregion
    }
}