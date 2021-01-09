﻿// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Linq;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Character;

namespace OpenNos.GameObject.Quests
{
    public class CharacterQuest : CharacterQuestDTO
    {
        #region Members

        private Quest _quest;

        #endregion

        #region Instantiation

        public CharacterQuest()
        {
        }

        public CharacterQuest(long questId, long characterId)
        {
            QuestId = questId;
            CharacterId = characterId;
        }

        #endregion

        #region Properties

        public Quest Quest => _quest ?? (_quest = ServerManager.Instance.GetQuest(QuestId));

        public bool RewardInWaiting { get; set; }

        public List<QuestRewardDTO> QuestRewards { get; set; }

        public short QuestNumber { get; set; }

        #endregion

        #region Methods

        public string GetInfoPacket(bool sendMsg) =>
            $"{QuestNumber}.{Quest.InfoId}.{(IsMainQuest || Quest.IsDaily ? Quest.InfoId : 0)}.{Quest.QuestType}.{FirstObjective}.{GetObjectiveByIndex(1)?.Objective ?? 0}.{(RewardInWaiting ? 1 : 0)}.{SecondObjective}.{GetObjectiveByIndex(2)?.Objective ?? 0}.{ThirdObjective}.{GetObjectiveByIndex(3)?.Objective ?? 0}.{FourthObjective}.{GetObjectiveByIndex(4)?.Objective ?? 0}.{FifthObjective}.{GetObjectiveByIndex(5)?.Objective ?? 0}.{(sendMsg ? 1 : 0)}";

        public QuestObjectiveDTO GetObjectiveByIndex(byte index)
        {
            return Quest.QuestObjectives.FirstOrDefault(q => q.ObjectiveIndex.Equals(index));
        }

        public int[] GetObjectives() => new[] { FirstObjective, SecondObjective, ThirdObjective, FourthObjective, FifthObjective };

        public void Incerment(byte index, int amount)
        {
            switch (index)
            {
                case 1:
                    FirstObjective += FirstObjective >= GetObjectiveByIndex(index)?.Objective ? 0 : amount;
                    break;

                case 2:
                    SecondObjective += SecondObjective >= GetObjectiveByIndex(index)?.Objective ? 0 : amount;
                    break;

                case 3:
                    ThirdObjective += ThirdObjective >= GetObjectiveByIndex(index)?.Objective ? 0 : amount;
                    break;

                case 4:
                    FourthObjective += FourthObjective >= GetObjectiveByIndex(index)?.Objective ? 0 : amount;
                    break;

                case 5:
                    FifthObjective += FifthObjective >= GetObjectiveByIndex(index)?.Objective ? 0 : amount;
                    break;
            }
        }
        
        #endregion
    }
}