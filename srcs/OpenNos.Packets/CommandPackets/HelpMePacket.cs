// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$HelpMe", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class HelpMePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Message { get; set; }

        public override string ToString() => "$HelpMe [Message]";

        #endregion
    }
}