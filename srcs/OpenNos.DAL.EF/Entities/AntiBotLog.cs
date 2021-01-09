// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace OpenNos.DAL.EF.Entities
{
    public class AntiBotLog
    {
        public long Id { get; set; }

        public long CharacterId { get; set; }

        public string CharacterName { get; set; }

        public bool Timeout { get; set; }

        public DateTime DateTime { get; set; }
    }
}