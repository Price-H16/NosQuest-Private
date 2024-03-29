﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Act4Connect", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class Act4ConnectPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string Name { get; set; }

        public override string ToString() => "Act4Connect Name";

        #endregion
    }
}