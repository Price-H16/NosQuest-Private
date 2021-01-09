// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("eff")]
    public class EffectPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte EffectType { get; set; }

        [PacketIndex(1)]
        public long CharacterId { get; set; }

        [PacketIndex(2)]
        public int Id { get; set; }

        #endregion
    }
}