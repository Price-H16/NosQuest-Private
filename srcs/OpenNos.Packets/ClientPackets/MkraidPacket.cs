// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("mkraid")]
    public class MkraidPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public byte RaidId { get; set; }

        [PacketIndex(1)]
        public short MapInstanceId { get; set; }

        #endregion
    }
}