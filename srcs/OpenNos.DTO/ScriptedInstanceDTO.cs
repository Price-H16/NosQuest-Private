// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class ScriptedInstanceDTO : MappingBaseDTO
    {
        #region Properties

        public short MapId { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public string Script { get; set; }

        public short ScriptedInstanceId { get; set; }

        public ScriptedInstanceType Type { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        #endregion
    }
}