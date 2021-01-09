// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Upgrade", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class UpgradeCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short Slot { get; set; }

        [PacketIndex(1)]
        public UpgradeMode Mode { get; set; }

        [PacketIndex(2)]
        public UpgradeProtection Protection { get; set; }

        public static string ReturnHelp() => "$Upgrade SLOT MODE PROTECTION";

        #endregion
    }
}