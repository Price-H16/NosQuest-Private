// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$CharEdit", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class CharacterEditPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Property { get; set; }

        [PacketIndex(1, serializeToEnd: true)]
        public string Data { get; set; }

        public static string ReturnHelp() => "$CharEdit PROPERTYNAME DATA";

        #endregion
    }
}