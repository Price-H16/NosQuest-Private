// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ILogCommandsDAO : IMappingBaseDAO
    {
        SaveResult InsertOrUpdate(ref LogCommandsDTO logCommand);
    }
}