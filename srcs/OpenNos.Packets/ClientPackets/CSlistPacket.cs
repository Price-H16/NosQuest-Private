// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("c_slist")]
    public class CsListPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Index { get; set; }


        [PacketIndex(1)]
        public byte Filter { get; set; }

        #endregion
    }
}