// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("fauth")]
    public class FAuthPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public FamilyAuthority MemberType { get; set; }

        [PacketIndex(1)]
        public byte AuthorityId { get; set; }

        [PacketIndex(2)]
        public byte Value { get; set; }

        #endregion
    }
}