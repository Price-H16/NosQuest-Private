// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("frank_cts")]
    public class FrankCtsPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public byte Type { get; set; }

        #endregion
    }
}