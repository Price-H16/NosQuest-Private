// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;

namespace OpenNos.GameObject.Event
{
    public class EventWave
    {
        public EventWave(byte delay, ConcurrentBag<EventContainer> events, byte offset = 0)
        {
            Delay = delay;
            Offset = offset;
            Events = events;
        }

        public byte Delay { get; }
        public byte Offset { get; set; }
        public DateTime LastStart { get; set; }
        public ConcurrentBag<EventContainer> Events { get; }

        #region Methods

        #endregion
    }
}