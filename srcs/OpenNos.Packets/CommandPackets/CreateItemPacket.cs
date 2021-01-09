﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$CreateItem", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class CreateItemPacket : PacketDefinition
    {
        #region Methods

        public override string ToString() => ($"CreateItem Command VNum: {VNum}" + Design) != null ? $" Design: {Design}" :
            ("" + Upgrade) != null ? $" Upgrade: {Upgrade}" : "";

        #endregion

        #region Properties

        [PacketIndex(0)]
        public short VNum { get; set; }

        [PacketIndex(1)]
        public short? Design { get; set; }

        [PacketIndex(2)]
        public short? Upgrade { get; set; }

        public static string ReturnHelp() => "$CreateItem ITEMVNUM DESIGN/RARE/AMOUNT/WINGS UPDATE";

        #endregion
    }
}