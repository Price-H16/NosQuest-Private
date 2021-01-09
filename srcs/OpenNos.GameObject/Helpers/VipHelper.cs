﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using OpenNos.GameObject.Networking;
using WingsEmu.Packets.VipPackets;

namespace OpenNos.GameObject.Helpers
{
    internal class VipHelper
    {
        public Dictionary<string, Action<ClientSession, VipCommandPacket>> TeleportDictionary =
            new Dictionary<string, Action<ClientSession, VipCommandPacket>>
            {
                { "Teleport", Teleport }
            };

        private static void Teleport(ClientSession session, VipCommandPacket packet)
        {
            if (session == null)
            {
            }
        }

        public class MapPos
        {
            public MapPos(long mapId, short x, short y)
            {
                MapId = mapId;
                X = x;
                Y = y;
            }

            public long MapId { get; }
            public short X { get; }
            public short Y { get; }
        }

        #region Singleton

        private static VipHelper _instance;

        public static VipHelper Instance => _instance ?? (_instance = new VipHelper());

        #endregion
    }
}