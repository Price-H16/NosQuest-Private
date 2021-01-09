// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Event
{
    public class EventSchedule : IConfigurationSectionHandler
    {
        #region Methods

        public object Create(object parent, object configContext, XmlNode section)
        {
            List<Schedule> list = new List<Schedule>();
            foreach (XmlNode aSchedule in section.ChildNodes)
            {
                list.Add(GetSchedule(aSchedule));
            }

            return list;
        }

        private static Schedule GetSchedule(XmlNode str)
        {
            if (str.Attributes != null)
            {
                var result = new Schedule
                {
                    Event = (EventType)Enum.Parse(typeof(EventType), str.Attributes["event"].Value),
                    Time = TimeSpan.Parse(str.Attributes["time"].Value)
                };
                return result;
            }

            return null;
        }

        #endregion
    }
}