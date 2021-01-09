// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ILogChatDAO : IMappingBaseDAO
    {
        SaveResult InsertOrUpdate(ref LogChatDTO logchat);
    }
}