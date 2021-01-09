// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs.Character
{
    public class CharacterQuestDTO : SynchronizableBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public long QuestId { get; set; }

        public int FirstObjective { get; set; }

        public int SecondObjective { get; set; }

        public int ThirdObjective { get; set; }

        public int FourthObjective { get; set; }

        public int FifthObjective { get; set; }

        public bool IsMainQuest { get; set; }

        #endregion
    }
}