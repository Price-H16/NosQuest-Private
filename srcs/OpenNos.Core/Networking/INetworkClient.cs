// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace OpenNos.Core.Networking
{
    public interface INetworkSession
    {
        /// <summary>
        /// </summary>
        long ClientId { get; }

        // todo urgent rework
        int SessionId { get; set; }

        /// <summary>
        /// </summary>
        IPAddress IpAddress { get; }

        bool IsConnected { get; }

        bool IsDisposing { get; set; }
        event EventHandler<string> PacketReceived;


        void SendPacket(string packet);
        Task SendPacketAsync(string packet);
        void SendPacketFormat(string packet, params object[] param);
        Task SendPacketAsyncFormat(string packet, params object[] param);
        void SendPackets(IEnumerable<string> packets);
        Task SendPacketsAsync(IEnumerable<string> packets);

        void DisconnectClient();
    }
}