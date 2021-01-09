// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Networking;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Event.TalentArena
{
    public class ArenaMember
    {
        public ClientSession Session { get; set; }
        public long? GroupId { get; set; }
        public EventType ArenaType { get; set; }
        public int Time { get; set; }
    }
}