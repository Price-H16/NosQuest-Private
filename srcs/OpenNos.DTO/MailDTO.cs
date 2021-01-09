// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    [Serializable]
    public class MailDTO : MappingBaseDTO
    {
        #region Properties

        public ushort AttachmentAmount { get; set; }

        public byte AttachmentRarity { get; set; }

        public byte AttachmentUpgrade { get; set; }

        public short? AttachmentVNum { get; set; }

        public DateTime Date { get; set; }

        public string EqPacket { get; set; }

        public bool IsOpened { get; set; }

        public bool IsSenderCopy { get; set; }

        public long MailId { get; set; }

        public string Message { get; set; }

        public long ReceiverId { get; set; }

        public ClassType SenderClass { get; set; }

        public GenderType SenderGender { get; set; }

        public HairColorType SenderHairColor { get; set; }

        public HairStyleType SenderHairStyle { get; set; }

        public long SenderId { get; set; }

        public short SenderMorphId { get; set; }

        public string Title { get; set; }

        public byte Design { get; set; }

        #endregion
    }
}