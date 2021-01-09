// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$SearchItem", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class SearchItemPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Data { get; set; }

        public static string ReturnHelp() => "$SearchItem PAGE NAME(*)";

        #endregion
    }
}