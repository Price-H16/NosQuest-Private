// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("npinfo")]
    public class NpinfoPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Page { get; set; }

        #endregion
    }
}