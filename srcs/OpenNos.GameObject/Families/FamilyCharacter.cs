// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.DAL;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Character;

namespace OpenNos.GameObject.Families
{
    public class FamilyCharacter : FamilyCharacterDTO
    {
        #region Members

        private CharacterDTO _character;

        #endregion

        #region Properties

        public CharacterDTO Character
        {
            get
            {
                if (_character == null)
                {
                    _character = DaoFactory.Instance.CharacterDao.LoadById(CharacterId);
                }

                return _character;
            }
        }

        #endregion

    }
}