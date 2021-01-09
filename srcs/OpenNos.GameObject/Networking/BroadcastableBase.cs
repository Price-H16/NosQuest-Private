﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Core.Logging;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Helpers;
using WingsEmu.Packets;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Networking
{
    public abstract class BroadcastableBase : IDisposable
    {
        #region Instantiation

        protected BroadcastableBase()
        {
            LastUnregister = DateTime.Now.AddMinutes(-1);
            _sessions = new ConcurrentDictionary<long, ClientSession>();
            _battleEntities = new ConcurrentDictionary<Tuple<SessionType, long>, IBattleEntity>();
        }

        #endregion

        #region Members

        /// <summary>
        ///     List of all connected clients.
        /// </summary>
        private readonly ConcurrentDictionary<long, ClientSession> _sessions;

        internal readonly ConcurrentDictionary<Tuple<SessionType, long>, IBattleEntity> _battleEntities;

        private bool _disposed;

        #endregion

        #region Properties

        public IEnumerable<ClientSession> Sessions
        {
            get
            {
                return _sessions.Select(s => s.Value)
                    .Where(s => s.HasSelectedCharacter && !s.IsDisposing && s.IsConnected);
            }
        }

        protected DateTime LastUnregister { get; private set; }

        #endregion

        #region Methods

        public void Broadcast(string packet)
        {
            Broadcast(null, packet);
        }

        public void Broadcast(string packet, int xRangeCoordinate, int yRangeCoordinate)
        {
            Broadcast(new BroadcastPacket(null, packet, ReceiverType.AllInRange, xCoordinate: xRangeCoordinate,
                yCoordinate: yRangeCoordinate));
        }

        public void Broadcast(PacketDefinition packet)
        {
            Broadcast(null, packet);
        }

        public void Broadcast(PacketDefinition packet, int xRangeCoordinate, int yRangeCoordinate)
        {
            Broadcast(new BroadcastPacket(null, PacketFactory.Serialize(packet), ReceiverType.AllInRange,
                xCoordinate: xRangeCoordinate, yCoordinate: yRangeCoordinate));
        }

        public void Broadcast(ClientSession client, PacketDefinition packet, ReceiverType receiver = ReceiverType.All,
            string characterName = "", long characterId = -1)
        {
            Broadcast(client, PacketFactory.Serialize(packet), receiver, characterName, characterId);
        }

        public void Broadcast(BroadcastPacket packet)
        {
            try
            {
                SpreadBroadcastpacket(packet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Broadcast(ClientSession client, string content, ReceiverType receiver = ReceiverType.All,
            string characterName = "", long characterId = -1)
        {
            try
            {
                SpreadBroadcastpacket(new BroadcastPacket(client, content, receiver, characterName, characterId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        public ClientSession GetSessionByCharacterId(long characterId) => _sessions.ContainsKey(characterId) ? _sessions[characterId] : null;

        public Mate GetMateByMateTransportId(long mateTransportId)
        {
            return (Mate)_battleEntities.Values
                .FirstOrDefault(b => b is Mate m && m.MateTransportId == mateTransportId).GetSession();
        }

        public void RegisterSession(ClientSession session)
        {
            if (!session.HasSelectedCharacter)
            {
                return;
            }

            session.RegisterTime = DateTime.Now;

            // Create a ChatClient and store it in a collection
            _sessions[session.Character.CharacterId] = session;
            _battleEntities[new Tuple<SessionType, long>(SessionType.Character, session.Character.CharacterId)] =
                session.Character;
            session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                _battleEntities[new Tuple<SessionType, long>(m.SessionType(), m.GetId())] = m);

            if (session.HasCurrentMapInstance)
            {
                session.CurrentMapInstance.IsSleeping = false;
            }

            Console.Title = string.Format(Language.Instance.GetMessageFromKey("WORLD_SERVER_CONSOLE_TITLE"),
                ServerManager.Instance.ChannelId, ServerManager.Instance.Sessions.Count(),
                ServerManager.Instance.IpAddress, ServerManager.Instance.Port);
        }

        public void UnregisterSession(long characterId)
        {
            // Remove client from online clients list
            if (!_sessions.TryRemove(characterId, out ClientSession session))
            {
                return;
            }

            if (session.HasCurrentMapInstance && _sessions.Count == 0)
            {
                session.CurrentMapInstance.IsSleeping = true;
            }

            _battleEntities.TryRemove(
                new Tuple<SessionType, long>(SessionType.Character, session.Character.CharacterId),
                out IBattleEntity character);
            session.Character.Mates.Where(m => m.IsTeamMember).ToList().ForEach(m =>
                _battleEntities.TryRemove(new Tuple<SessionType, long>(m.SessionType(), m.GetId()),
                    out IBattleEntity mate));

            Console.Title = string.Format(Language.Instance.GetMessageFromKey("WORLD_SERVER_CONSOLE_TITLE"),
                ServerManager.Instance.ChannelId, ServerManager.Instance.Sessions.Count(),
                ServerManager.Instance.IpAddress, ServerManager.Instance.Port);
            LastUnregister = DateTime.Now;
        }

        private void SpreadBroadcastpacket(BroadcastPacket sentPacket)
        {
            if (Sessions == null || string.IsNullOrEmpty(sentPacket?.Packet))
            {
                return;
            }

            switch (sentPacket.Receiver)
            {
                case ReceiverType.All: // send packet to everyone
                    if (sentPacket.Packet.StartsWith("out"))
                    {
                        foreach (ClientSession session in Sessions)
                        {
                            if (!session.HasSelectedCharacter)
                            {
                                continue;
                            }

                            if (sentPacket.Sender != null)
                            {
                                if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId))
                                {
                                    session.SendPacket(sentPacket.Packet);
                                }
                            }
                            else
                            {
                                session.SendPacket(sentPacket.Packet);
                            }
                        }
                    }
                    else
                    {
                        Parallel.ForEach(Sessions, session =>
                        {
                            if (!session.HasSelectedCharacter)
                            {
                                return;
                            }

                            if (sentPacket.Sender != null)
                            {
                                if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId))
                                {
                                    session.SendPacket(sentPacket.Packet);
                                }
                            }
                            else
                            {
                                session.SendPacket(sentPacket.Packet);
                            }
                        });
                    }

                    break;
                case ReceiverType.AllExceptMeAct4:
                    if (sentPacket.Sender == null)
                    {
                        return;
                    }

                    foreach (ClientSession session in Sessions.Where(s =>
                        s.SessionId != sentPacket.Sender.SessionId &&
                        s.Character.Faction == sentPacket.Sender.Character.Faction && s.HasSelectedCharacter))
                    {
                        session.SendPacket(sentPacket.Packet);
                    }

                    break;
                case ReceiverType.AllExceptMe: // send to everyone except the sender
                    if (sentPacket.Packet.StartsWith("out"))
                    {
                        foreach (ClientSession session in Sessions.Where(
                            s => s.SessionId != sentPacket.Sender.SessionId))
                        {
                            if (!session.HasSelectedCharacter)
                            {
                                continue;
                            }

                            if (sentPacket.Sender != null)
                            {
                                if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId))
                                {
                                    session.SendPacket(sentPacket.Packet);
                                }
                            }
                            else
                            {
                                session.SendPacket(sentPacket.Packet);
                            }
                        }
                    }
                    else
                    {
                        Parallel.ForEach(Sessions.Where(s => s.SessionId != sentPacket.Sender.SessionId), session =>
                        {
                            if (!session.HasSelectedCharacter)
                            {
                                return;
                            }

                            if (sentPacket.Sender != null)
                            {
                                if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId))
                                {
                                    session.SendPacket(sentPacket.Packet);
                                }
                            }
                            else
                            {
                                session.SendPacket(sentPacket.Packet);
                            }
                        });
                    }

                    break;
                case ReceiverType.AllExceptGroup:
                    foreach (ClientSession session in Sessions.Where(s =>
                        s.SessionId != sentPacket.Sender.SessionId &&
                        (s.Character?.Group == null ||
                            s.Character?.Group?.GroupId != sentPacket.Sender?.Character?.Group?.GroupId)))
                    {
                        if (!session.HasSelectedCharacter)
                        {
                            continue;
                        }

                        if (sentPacket.Sender != null)
                        {
                            if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId))
                            {
                                session.SendPacket(sentPacket.Packet);
                            }
                        }
                        else
                        {
                            session.SendPacket(sentPacket.Packet);
                        }
                    }

                    break;
                case ReceiverType.AllInRange: // send to everyone which is in a range of 50x50
                    if (sentPacket.XCoordinate != 0 && sentPacket.YCoordinate != 0)
                    {
                        Parallel.ForEach(
                            Sessions.Where(s => s.Character.IsInRange(sentPacket.XCoordinate, sentPacket.YCoordinate)),
                            session =>
                            {
                                if (!session.HasSelectedCharacter)
                                {
                                    return;
                                }

                                if (sentPacket.Sender != null)
                                {
                                    if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId)
                                    )
                                    {
                                        session.SendPacket(sentPacket.Packet);
                                    }
                                }
                                else
                                {
                                    session.SendPacket(sentPacket.Packet);
                                }
                            });
                    }

                    break;

                case ReceiverType.OnlySomeone:
                    if (sentPacket.SomeonesCharacterId > 0 || !string.IsNullOrEmpty(sentPacket.SomeonesCharacterName))
                    {
                        ClientSession targetSession = Sessions.SingleOrDefault(s =>
                            s.Character.CharacterId == sentPacket.SomeonesCharacterId ||
                            s.Character.Name == sentPacket.SomeonesCharacterName);
                        if (targetSession != null && targetSession.HasSelectedCharacter)
                        {
                            if (sentPacket.Sender != null)
                            {
                                if (!sentPacket.Sender.Character.IsBlockedByCharacter(targetSession.Character
                                    .CharacterId))
                                {
                                    targetSession.SendPacket(sentPacket.Packet);
                                }
                                else
                                {
                                    sentPacket.Sender.SendPacket(
                                        UserInterfaceHelper.Instance.GenerateInfo(
                                            Language.Instance.GetMessageFromKey("BLACKLIST_BLOCKED")));
                                }
                            }
                            else
                            {
                                targetSession.SendPacket(sentPacket.Packet);
                            }
                        }
                    }

                    break;

                case ReceiverType.AllNoEmoBlocked:
                    Parallel.ForEach(Sessions.Where(s => !s.Character.EmoticonsBlocked), session =>
                    {
                        if (!session.HasSelectedCharacter)
                        {
                            return;
                        }

                        if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId))
                        {
                            session.SendPacket(sentPacket.Packet);
                        }
                    });
                    break;

                case ReceiverType.AllNoHeroBlocked:
                    Parallel.ForEach(Sessions.Where(s => !s.Character.HeroChatBlocked), session =>
                    {
                        if (!session.HasSelectedCharacter)
                        {
                            return;
                        }

                        if (!sentPacket.Sender.Character.IsBlockedByCharacter(session.Character.CharacterId))
                        {
                            session.SendPacket(sentPacket.Packet);
                        }
                    });
                    break;

                case ReceiverType.Group:
                    Parallel.ForEach(
                        Sessions.Where(s =>
                            s.Character?.Group != null && sentPacket.Sender?.Character?.Group != null &&
                            s.Character.Group.GroupId == sentPacket.Sender.Character.Group.GroupId),
                        session => { session.SendPacket(sentPacket.Packet); });
                    break;

                case ReceiverType.Unknown:
                    break;
            }
        }

        #endregion
    }
}