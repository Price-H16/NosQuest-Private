﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Rarify", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class RarifyPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(1)]
        public RarifyMode Mode { get; set; }

        [PacketIndex(2)]
        public RarifyProtection Protection { get; set; }

        [PacketIndex(0)]
        public short Slot { get; set; }

        public static string ReturnHelp() => "$Rarify SLOT MODE PROTECTION";

        #endregion
    }
}