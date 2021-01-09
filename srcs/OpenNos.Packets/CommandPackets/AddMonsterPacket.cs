﻿// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$AddMonster", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class AddMonsterPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public short MonsterVNum { get; set; }

        [PacketIndex(1)]
        public bool IsMoving { get; set; }

        public static string ReturnHelp() => "$AddMonster VNUM MOVE";

        #endregion
    }
}