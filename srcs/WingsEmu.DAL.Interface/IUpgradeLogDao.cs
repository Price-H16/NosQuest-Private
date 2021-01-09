// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IUpgradeLogDao : IMappingBaseDAO
    {
        #region Methods

        SaveResult InsertOrUpdate(ref UpgradeLogDTO upgr);

        #endregion
    }
}