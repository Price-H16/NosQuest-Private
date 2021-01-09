﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.ComponentModel.DataAnnotations;
using WingsEmu.Packets.Enums;

namespace OpenNos.DAL.EF.Entities
{
    public class Mail
    {
        #region Properties

        public short AttachmentAmount { get; set; }

        public byte AttachmentRarity { get; set; }

        public byte AttachmentUpgrade { get; set; }

        public short? AttachmentVNum { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string EqPacket { get; set; }

        public bool IsOpened { get; set; }

        public bool IsSenderCopy { get; set; }

        public virtual Item Item { get; set; }

        [Key]
        public long MailId { get; set; }

        [MaxLength(255)]
        public string Message { get; set; }

        public virtual Character Receiver { get; set; }

        public long ReceiverId { get; set; }

        public virtual Character Sender { get; set; }

        public ClassType SenderClass { get; set; }

        public GenderType SenderGender { get; set; }

        public HairColorType SenderHairColor { get; set; }

        public HairStyleType SenderHairStyle { get; set; }

        public long SenderId { get; set; }

        public short SenderMorphId { get; set; }

        [MaxLength(255)]
        public string Title { get; set; }

        public byte Design { get; set; }

        #endregion
    }
}