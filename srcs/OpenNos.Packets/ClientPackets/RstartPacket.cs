// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("rstart")]
    public class RStartPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public byte? Type { get; set; }

        #endregion
    }
}