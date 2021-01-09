// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class QuestObjectiveDTO : MappingBaseDTO
    {
        public int QuestObjectiveId { get; set; }

        public int QuestId { get; set; }

        public int? Data { get; set; }

        public int? Objective { get; set; }

        public int? SpecialData { get; set; }

        public int? DropRate { get; set; }

        public byte ObjectiveIndex { get; set; }
    }
}