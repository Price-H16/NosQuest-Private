﻿// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Concurrent;
using OpenNos.GameObject.Event;

namespace OpenNos.GameObject.Maps
{
    public class ZoneEvent
    {
        #region Properties

        public short X { get; set; }

        public short Y { get; set; }

        public short Range { get; set; }

        public ConcurrentBag<EventContainer> Events { get; set; }

        public ZoneEvent()
        {
            Events = new ConcurrentBag<EventContainer>();
            Range = 1;
        }

        public bool InZone(short positionX, short positionY) => positionX <= X + Range && positionX >= X - Range && positionY <= Y + Range && positionY >= Y - Range;

        #endregion
    }
}