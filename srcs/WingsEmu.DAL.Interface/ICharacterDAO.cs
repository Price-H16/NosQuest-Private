// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs.Character;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface ICharacterDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult DeleteByPrimaryKey(long accountId, byte characterSlot);

        List<CharacterDTO> GetTopCompliment();

        List<CharacterDTO> GetTopPoints();

        List<CharacterDTO> GetTopReputation();

        SaveResult InsertOrUpdate(ref CharacterDTO character);

        IEnumerable<CharacterDTO> LoadAll();

        IEnumerable<CharacterDTO> LoadByAccount(long accountId);

        CharacterDTO LoadById(long characterId);

        CharacterDTO LoadByName(string name);

        CharacterDTO LoadBySlot(long accountId, byte slot);

        IEnumerable<CharacterDTO> LoadAllCharactersByAccount(long accountId);

        #endregion
    }
}