// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IAccountDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult Delete(long accountId);

        long GetBankRanking(long accountId);

        SaveResult InsertOrUpdate(ref AccountDTO account);

        AccountDTO LoadById(long accountId);

        AccountDTO LoadByName(string name);

        bool ContainsAccounts();

        void WriteGeneralLog(long accountId, string ipAddress, long? characterId, GeneralLogType logType, string logData);

        #endregion
    }
}