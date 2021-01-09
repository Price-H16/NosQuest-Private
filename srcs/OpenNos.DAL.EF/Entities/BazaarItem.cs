﻿// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace OpenNos.DAL.EF.Entities
{
    public class BazaarItem
    {
        #region Properties

        public short Amount { get; set; }

        public long BazaarItemId { get; set; }

        public virtual Character Character { get; set; }

        public DateTime DateStart { get; set; }

        public short Duration { get; set; }

        public bool IsPackage { get; set; }

        public virtual ItemInstance ItemInstance { get; set; }

        public Guid ItemInstanceId { get; set; }

        public bool MedalUsed { get; set; }

        public long Price { get; set; }

        public long SellerId { get; set; }

        #endregion
    }
}