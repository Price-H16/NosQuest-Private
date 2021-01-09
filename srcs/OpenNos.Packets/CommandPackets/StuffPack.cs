// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$StuffPack", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class StuffPackPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Type { get; set; }

        #endregion
    }
}