﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.DTOs.Interfaces
{
    public interface IWearableInstance : IItemInstance
    {
        #region Properties

        byte Ammo { get; set; }

        byte Cellon { get; set; }

        short CloseDefence { get; set; }

        short Concentrate { get; set; }

        short CriticalDodge { get; set; }

        byte CriticalLuckRate { get; set; }

        short CriticalRate { get; set; }

        short DamageMaximum { get; set; }

        short DamageMinimum { get; set; }

        byte DarkElement { get; set; }

        short DarkResistance { get; set; }

        short DefenceDodge { get; set; }

        short DistanceDefence { get; set; }

        short DistanceDefenceDodge { get; set; }

        short ElementRate { get; set; }

        byte FireElement { get; set; }

        short FireResistance { get; set; }

        short HitRate { get; set; }

        short HP { get; set; }

        bool IsEmpty { get; set; }

        bool IsFixed { get; set; }

        byte LightElement { get; set; }

        short LightResistance { get; set; }

        short MagicDefence { get; set; }

        short MP { get; set; }

        sbyte? ShellRarity { get; set; }

        byte WaterElement { get; set; }

        short WaterResistance { get; set; }

        long XP { get; set; }

        #endregion
    }
}