// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("ps_op")]
    public class PsopPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte PetId { get; set; }

        [PacketIndex(1)]
        public byte SkillSlot { get; set; }

        [PacketIndex(2)]
        public byte Option { get; set; }

        #endregion
    }
}