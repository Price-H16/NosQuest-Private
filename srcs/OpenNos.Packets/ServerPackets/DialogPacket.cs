﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ServerPackets
{
    [PacketHeader("dlg")]
    public class DialogPacket<TAnswerYesPacket, TAnswerNoPacket> : PacketDefinition
    where TAnswerYesPacket : PacketDefinition
    where TAnswerNoPacket : PacketDefinition
    {
        [PacketIndex(0, true)]
        public TAnswerYesPacket AnswerYesReturnPacket { get; set; }

        [PacketIndex(1, true)]
        public TAnswerNoPacket AnswerNoReturnPacket { get; set; }

        [PacketIndex(2, SerializeToEnd = true)]
        public string DialogText { get; set; }
    }
}