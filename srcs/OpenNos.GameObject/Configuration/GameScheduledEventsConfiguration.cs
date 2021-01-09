// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OpenNos.GameObject.Event;
using WingsEmu.Configuration;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Configuration
{
    [DataContract]
    public class GameScheduledEventsConfiguration : IConfiguration
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public List<Schedule> ScheduledEvents { get; set; } = new List<Schedule>
        {
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("00:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("01:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("02:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("03:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("04:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("05:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("06:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("07:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("08:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("09:00") },
            new Schedule { Event = EventType.INSTANTBATTLE, Time = TimeSpan.Parse("10:00") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("00:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("01:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("02:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("03:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("04:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("05:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("06:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("07:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("08:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("09:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("10:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("11:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("12:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("13:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("14:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("15:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("16:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("17:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("20:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("21:30") },
            new Schedule { Event = EventType.LOD, Time = TimeSpan.Parse("22:00") },
            new Schedule { Event = EventType.LODDH, Time = TimeSpan.Parse("23:30") },
        };

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBeforeAutoKick { get; set; } = TimeSpan.FromMinutes(2);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBetweenAct6Refresh { get; set; } = TimeSpan.FromSeconds(5);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBetweenAct4Refresh { get; set; } = TimeSpan.FromSeconds(5);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBetweenOutdatedBazaarItemRefresh { get; set; } = TimeSpan.FromMinutes(5);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBetweenGroupRefresh { get; set; } = TimeSpan.FromSeconds(2);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBetweenAct4FlowerRespawn { get; set; } = TimeSpan.FromMinutes(1);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBetweenAntiBotProcess { get; set; } = TimeSpan.FromHours(3);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeBetweenItemValidityRefresh { get; set; } = TimeSpan.FromSeconds(1);

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public TimeSpan TimeAutoKickInterval { get; set; } = TimeSpan.FromMinutes(2);
    }
}