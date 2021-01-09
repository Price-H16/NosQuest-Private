// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.Master.Datas
{
    public class PlayerSession
    {
        public PlayerSession(long accountId, long sessionId, string accountName)
        {
            AccountId = accountId;
            SessionId = sessionId;
            AccountName = accountName;
        }

        public string AccountName { get; }

        public long AccountId { get; }

        public long CharacterId { get; set; }

        public CharacterSession Character { get; set; }

        public bool CanSwitchChannel { get; set; }

        public DateTime LastPulse { get; set; }

        public WorldServer ConnectedWorld { get; set; }

        public WorldServer PreviousChannel { get; set; }

        public string IpAddress { get; set; }

        public long SessionId { get; }
    }
}