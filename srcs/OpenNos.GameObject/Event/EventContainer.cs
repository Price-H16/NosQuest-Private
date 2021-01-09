// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Maps;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Event
{
    public class EventContainer
    {
        #region Instantiation

        public EventContainer(MapInstance mapInstance, EventActionType eventActionType, object param)
        {
            MapInstance = mapInstance;
            EventActionType = eventActionType;
            Parameter = param;
        }

        #endregion

        #region Properties

        public EventActionType EventActionType { get; }

        public MapInstance MapInstance { get; set; }

        public object Parameter { get; set; }

        #endregion
    }
}