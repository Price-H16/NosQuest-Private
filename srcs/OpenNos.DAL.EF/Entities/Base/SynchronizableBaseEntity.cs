﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenNos.DAL.EF.Entities.Base
{
    public abstract class SynchronizableBaseEntity
    {
        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        #endregion
    }
}