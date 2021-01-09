// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Event
{
    public class Schedule
    {
        #region Properties

        public EventType Event { get; set; }

        public TimeSpan Time { get; set; }

        #endregion
    }
}