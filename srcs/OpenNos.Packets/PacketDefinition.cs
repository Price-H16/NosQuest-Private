// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets
{
    public abstract class PacketDefinition
    {
        #region Properties

        public string OriginalContent { get; set; }

        public string OriginalHeader { get; set; }

        #endregion
    }
}