// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("hero")]
    public class HeroPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int Type { get; set; }

        [PacketIndex(1)]
        public string CharacterName { get; set; }

        [PacketIndex(2)]
        public string Message { get; set; }

        #endregion
    }
}