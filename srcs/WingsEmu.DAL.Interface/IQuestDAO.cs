// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IQuestDAO : IMappingBaseDAO
    {
        #region Methods

        void InsertOrUpdate(List<QuestDTO> quests);
        QuestDTO Insert(QuestDTO quest);

        void Insert(List<QuestDTO> quests);

        List<QuestDTO> LoadAll();

        QuestDTO LoadById(long questId);

        #endregion
    }
}