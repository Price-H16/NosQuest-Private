// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IExchangeLogDao : IMappingBaseDAO
    {
        #region Methods

        SaveResult InsertOrUpdate(ref ExchangeLogDTO exchange);

        #endregion
    }
}