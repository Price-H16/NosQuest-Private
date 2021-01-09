// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ILevelUpRewardsDAO : IMappingBaseDAO
    {
        IEnumerable<LevelUpRewardsDTO> LoadByLevelAndLevelType(byte level, LevelType type);
    }
}