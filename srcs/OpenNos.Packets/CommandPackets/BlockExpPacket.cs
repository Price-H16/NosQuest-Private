﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$BlockExp", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class BlockExpPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string CharacterName { get; set; }

        [PacketIndex(1)]
        public int Duration { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string Reason { get; set; }

        public static string ReturnHelp() => "$BlockExp CHARACTERNAME DURATION REASON";

        public override string ToString() => $"BlockExp Command CharacterName: {CharacterName} Duration: {Duration} Reason: {Reason}";

        #endregion
    }
}