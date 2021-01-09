// WingsEmu
// 
// Developed by NosWings Team

using System;
using OpenNos.GameObject.Networking;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Event.TalentArena
{
    public class ArenaTeamMember
    {
        public ArenaTeamMember(ClientSession session, ArenaTeamType arenaTeamType, byte? order)
        {
            Session = session;
            ArenaTeamType = arenaTeamType;
            Order = order;
        }

        public ClientSession Session { get; set; }
        public ArenaTeamType ArenaTeamType { get; set; }
        public byte? Order { get; set; }
        public bool Dead { get; set; }
        public DateTime? LastSummoned { get; set; }
        public byte SummonCount { get; set; }
    }
}