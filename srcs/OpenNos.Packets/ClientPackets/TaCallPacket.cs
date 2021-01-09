// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("ta_call")]
    public class TaCallPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte CalledIndex { get; set; }

        #endregion
    }
}