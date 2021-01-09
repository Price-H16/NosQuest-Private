// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("pjoin")]
    public class PJoinPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public GroupRequestType RequestType { get; set; }

        [PacketIndex(1)]
        public long CharacterId { get; set; }

        #endregion
    }
}