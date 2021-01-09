// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("useobj")]
    public class UseobjPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public short Slot { get; set; }

        #endregion
    }
}