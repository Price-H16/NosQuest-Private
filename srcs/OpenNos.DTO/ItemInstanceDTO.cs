// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs.Base;
using WingsEmu.DTOs.Interfaces;
using WingsEmu.Packets.Enums;

namespace WingsEmu.DTOs
{
    public class ItemInstanceDTO : SynchronizableBaseDTO, IItemInstance
    {
        #region Properties

        public ushort Amount { get; set; }

        public long? BoundCharacterId { get; set; }

        public long CharacterId { get; set; }

        public short Design { get; set; }

        public int DurabilityPoint { get; set; }

        public DateTime? ItemDeleteTime { get; set; }

        public short ItemVNum { get; set; }

        public sbyte Rare { get; set; }

        public short Slot { get; set; }

        public InventoryType Type { get; set; }

        public byte Upgrade { get; set; }

        public byte Agility { get; set; }

        #endregion
    }
}