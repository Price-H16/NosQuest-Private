// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.Core.Networking;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Networking
{
    public class SessionManager
    {
        protected ConcurrentDictionary<long, ClientSession> Sessions = new ConcurrentDictionary<long, ClientSession>();

        public SessionManager(Type packetHandler, bool isWorldServer)
        {
            PacketHandler = packetHandler;
            IsWorldServer = isWorldServer;
        }


        public bool IsWorldServer { get; set; }


        protected Type PacketHandler { get; }


        public void AddSession(INetworkSession customClient)
        {
            Logger.Log.Info(Language.Instance.GetMessageFromKey("NEW_CONNECT") + customClient.ClientId);

            var session = new ClientSession(customClient);
            session.Initialize(PacketHandler, IsWorldServer);
            if (!IsWorldServer)
            {
                return;
            }

            if (Sessions.TryAdd(customClient.ClientId, session))
            {
                return;
            }

            Logger.Log.WarnFormat(Language.Instance.GetMessageFromKey("FORCED_DISCONNECT"), customClient.ClientId);
            customClient.DisconnectClient();
            Sessions.TryRemove(customClient.ClientId, out session);
        }

        public virtual void StopServer()
        {
            Sessions.Clear();
            ServerManager.Instance.StopServer();
        }


        protected void RemoveSession(INetworkSession client)
        {
            Sessions.TryRemove(client.ClientId, out ClientSession session);

            // check if session hasnt been already removed
            if (session == null)
            {
                return;
            }

            session.IsDisposing = true;

            if (IsWorldServer && session.HasSelectedCharacter)
            {
                session.Character.Mates.Where(s => s.IsTeamMember).ToList().ForEach(s =>
                    session.CurrentMapInstance?.Broadcast(session, s.GenerateOut(), ReceiverType.AllExceptMe));
                session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateOut(),
                    ReceiverType.AllExceptMe);
            }

            session.Destroy();

            if (IsWorldServer)
            {
                if (session.HasSelectedCharacter)
                {
                    if (session.Character.Hp < 1)
                    {
                        session.Character.Hp = 1;
                    }

                    if (ServerManager.Instance.Groups.Any(s => s.IsMemberOfGroup(session.Character.CharacterId)))
                    {
                        ServerManager.Instance.GroupLeave(session);
                    }

                    session.Character.LeaveTalentArena(true);
                    session.Character.Save();
                }
            }

            client.DisconnectClient();
            Logger.Log.Info(Language.Instance.GetMessageFromKey("DISCONNECT") + client.ClientId);
            // session = null;
        }
    }
}