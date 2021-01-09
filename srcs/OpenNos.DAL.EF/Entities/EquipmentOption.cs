// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.ComponentModel.DataAnnotations.Schema;
using OpenNos.DAL.EF.Entities.Base;

namespace OpenNos.DAL.EF.Entities
{
    public class EquipmentOption : SynchronizableBaseEntity
    {
        #region Properties

        public byte Level { get; set; }

        public byte Type { get; set; }

        public int Value { get; set; }

        [ForeignKey(nameof(WearableInstanceId))]
        public virtual WearableInstance WearableInstance { get; set; }

        public Guid WearableInstanceId { get; set; }

        #endregion
    }
}