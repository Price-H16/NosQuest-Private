﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.Enums
{
    public enum TargetHitType : byte
    {
        SingleTargetHit = 0,
        SingleTargetHitCombo = 1,
        SingleAOETargetHit = 2,
        AOETargetHit = 3,
        ZoneHit = 4,
        SpecialZoneHit = 5
    }
}