// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IAntiBotLogDAO : IMappingBaseDAO
    {
        SaveResult InsertOrUpdate(ref AntiBotLogDTO botlog);
    }
}