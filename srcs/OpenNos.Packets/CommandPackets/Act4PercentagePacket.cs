﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Act4Percent", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class Act4PercentagePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public int? Faction { get; set; }

        [PacketIndex(1)]
        public int? Percent { get; set; }

        public override string ToString() => "$Act4Percent Faction Percent";

        #endregion
    }
}