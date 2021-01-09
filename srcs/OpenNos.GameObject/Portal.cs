// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;

namespace OpenNos.GameObject
{
    public class Portal : PortalDTO
    {
        #region Instantiation

        public Portal() => OnTraversalEvents = new List<EventContainer>();

        #endregion

        #region Methods

        public string GenerateGp() => $"gp {SourceX} {SourceY} {ServerManager.Instance.GetMapInstance(DestinationMapInstanceId)?.Map.MapId ?? 0} {Type} {PortalId} {(IsDisabled ? 1 : 0)}";

        #endregion

        #region Members

        private Guid _destinationMapInstanceId;
        private Guid _sourceMapInstanceId;

        #endregion

        #region Properties

        public Guid DestinationMapInstanceId
        {
            get
            {
                if (_destinationMapInstanceId == default && DestinationMapId != -1)
                {
                    _destinationMapInstanceId = ServerManager.Instance.GetBaseMapInstanceIdByMapId(DestinationMapId);
                }

                return _destinationMapInstanceId;
            }
            set => _destinationMapInstanceId = value;
        }

        public List<EventContainer> OnTraversalEvents { get; set; }

        public Guid SourceMapInstanceId
        {
            get
            {
                if (_sourceMapInstanceId == default)
                {
                    _sourceMapInstanceId = ServerManager.Instance.GetBaseMapInstanceIdByMapId(SourceMapId);
                }

                return _sourceMapInstanceId;
            }
            set => _sourceMapInstanceId = value;
        }

        #endregion
    }
}