// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("u_as")]
    public class UseAoeSkillPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public int CastId { get; set; }

        [PacketIndex(1)]
        public short MapX { get; set; }

        [PacketIndex(2)]
        public short MapY { get; set; }

        #endregion
    }
}