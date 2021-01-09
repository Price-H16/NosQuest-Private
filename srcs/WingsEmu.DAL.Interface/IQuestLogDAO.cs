// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Enums;

namespace WingsEmu.DAL.Interface
{
    public interface IQuestLogDAO : IMappingBaseDAO
    {
        SaveResult InsertOrUpdate(ref QuestLogDTO bcard);

        QuestLogDTO LoadById(long id);

        IEnumerable<QuestLogDTO> LoadByCharacterId(long id);
    }
}