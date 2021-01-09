// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.DTOs;

namespace WingsEmu.DAL.Interface
{
    public interface IQuestObjectiveDAO : IMappingBaseDAO
    {
        #region Methods

        QuestObjectiveDTO Insert(QuestObjectiveDTO questObjective);

        void Insert(List<QuestObjectiveDTO> questObjectives);

        List<QuestObjectiveDTO> LoadAll();

        IEnumerable<QuestObjectiveDTO> LoadByQuestId(long questId);

        #endregion
    }
}