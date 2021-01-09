// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Effect", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class EffectCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int EffectId { get; set; }

        public static string ReturnHelp() => "$Effect EFFECTID";

        #endregion
    }
}