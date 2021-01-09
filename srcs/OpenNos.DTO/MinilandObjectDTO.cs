﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;

namespace WingsEmu.DTOs
{
    public class MinilandObjectDTO : MappingBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public Guid? ItemInstanceId { get; set; }

        public byte Level1BoxAmount { get; set; }

        public byte Level2BoxAmount { get; set; }

        public byte Level3BoxAmount { get; set; }

        public byte Level4BoxAmount { get; set; }

        public byte Level5BoxAmount { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public long MinilandObjectId { get; set; }

        #endregion
    }
}