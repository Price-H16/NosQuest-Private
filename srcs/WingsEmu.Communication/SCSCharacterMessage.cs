// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Communication
{
    public class SCSCharacterMessage
    {
        #region Instantiation

        #endregion

        #region Properties

        public long? DestinationCharacterId { get; set; }

        public string Message { get; set; }

        public long SourceCharacterId { get; set; }

        public Guid SourceWorldId { get; set; }
        public int SourceWorldChannelId { get; set; }

        public MessageType Type { get; set; }

        #endregion
    }
}