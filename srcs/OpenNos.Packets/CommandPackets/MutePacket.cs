// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Mute", PassNonParseablePacket = true, Authority = AuthorityType.Moderator)]
    public class MutePacket : PacketDefinition
    {
        #region Methods

        public override string ToString() => $"Mute Command CharacterName: {CharacterName} Duration: {Duration} Reason: {Reason}";

        #endregion

        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public int? Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$Mute CHARACTERNAME DURATION(MINUTES) REASON";

        #endregion
    }
}