// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ILogVIPDAO : IMappingBaseDAO
    {
        SaveResult InsertOrUpdate(ref LogVIPDTO log);

        LogVIPDTO GetLastByAccountId(long accountId);
    }
}