// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Lobby", Authority = AuthorityType.GameMaster)]
    public class LobbyPacket : PacketDefinition
    {
        #region Methods

        public static string ReturnHelp() => "$Lobby";

        #endregion
    }
}