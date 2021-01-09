// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("fmg")]
    public class FamilyManagementPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public FamilyAuthority FamilyAuthorityType { get; set; }

        [PacketIndex(1)]
        public long TargetId { get; set; }

        #endregion
    }
}