// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ClearMap", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ClearMapPacket : PacketDefinition
    {
        #region Properties

        public override string ToString() => "ClearMap Name";

        #endregion
    }
}