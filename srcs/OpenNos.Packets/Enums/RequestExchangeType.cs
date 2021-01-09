﻿// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.Enums
{
    public enum RequestExchangeType : byte
    {
        Unknown = 0,
        Requested = 1,
        List = 2,
        Confirmed = 3,
        Cancelled = 4,
        Declined = 5
    }
}