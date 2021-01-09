// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("mledit")]
    public class MlEditPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Type { get; set; }

        [PacketIndex(1, SerializeToEnd = true)]
        public string Parameters { get; set; }

        #endregion
    }
}