// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Warn", PassNonParseablePacket = true, Authority = AuthorityType.Moderator)]
    public class WarningPacket : PacketDefinition
    {
        #region Methods

        public override string ToString() => $"Warning Command CharacterName: {CharacterName} Message: {Reason}";

        #endregion

        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1, serializeToEnd: true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$Warn CHARACTERNAME REASON";

        #endregion
    }
}