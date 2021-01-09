// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs.Character
{
    public class CharacterHomeDTO : SynchronizableBaseDTO
    {
        public long CharacterId { get; set; }

        public string Name { get; set; }

        public short MapId { get; set; }
        public short MapX { get; set; }
        public short MapY { get; set; }
    }
}