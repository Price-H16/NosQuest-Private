// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IRaidLogDAO : IMappingBaseDAO
    {
        SaveResult InsertOrUpdate(ref RaidLogDTO raid);

        IEnumerable<RaidLogDTO> LoadByCharacterId(long characterId);

        IEnumerable<RaidLogDTO> LoadByFamilyId(long familyId);
    }
}