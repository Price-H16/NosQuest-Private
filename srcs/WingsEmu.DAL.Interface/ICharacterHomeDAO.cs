// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs.Character;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ICharacterHomeDAO : IMappingBaseDAO
    {
        SaveResult InsertOrUpdate(ref CharacterHomeDTO dto);

        IEnumerable<CharacterHomeDTO> LoadByCharacterId(long id);

        IEnumerable<CharacterHomeDTO> LoadAll();

        DeleteResult DeleteByName(long characterId, string name);
    }
}