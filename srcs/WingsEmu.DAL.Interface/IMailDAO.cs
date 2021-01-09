// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IMailDAO : IMappingBaseDAO
    {
        #region Methods

        DeleteResult DeleteById(long mailId);

        SaveResult InsertOrUpdate(ref MailDTO mail);

        IEnumerable<MailDTO> LoadAll();

        IEnumerable<MailDTO> LoadByCharacterId(long characterId);

        MailDTO LoadById(long mailId);

        #endregion
    }
}