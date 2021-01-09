// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.DTOs.Interfaces
{
    public interface IItemInstance
    {
        #region Properties

        ushort Amount { get; set; }

        long? BoundCharacterId { get; set; }

        short Design { get; set; }

        Guid Id { get; set; }

        DateTime? ItemDeleteTime { get; set; }

        short ItemVNum { get; set; }

        sbyte Rare { get; set; }

        byte Upgrade { get; set; }

        #endregion
    }
}