// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("npc_req")]
    public class RequestNpcPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        [PacketIndex(1)]
        public long Owner { get; set; }

        #endregion
    }
}