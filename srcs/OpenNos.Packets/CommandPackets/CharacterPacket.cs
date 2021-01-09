// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Character", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class CharacterPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Property { get; set; }

        [PacketIndex(1)]
        public string Value { get; set; }

        [PacketIndex(2)]
        public string Name { get; set; }

        #endregion
    }
}