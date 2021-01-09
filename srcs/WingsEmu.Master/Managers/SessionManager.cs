// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.Master.Datas;

namespace WingsEmu.Master.Managers
{
    public class SessionManager
    {
        private readonly Dictionary<long, PlayerSession> _sessionByAccountId;
        private readonly Dictionary<long, PlayerSession> _sessionBySessionId;
        private readonly Dictionary<string, Dictionary<long, PlayerSession>> _sessionsByWorldGroupAndCharacterId;

        private readonly WorldServerManager _worldManager;

        public SessionManager(WorldServerManager worldManager)
        {
            _worldManager = worldManager;
            _sessionByAccountId = new Dictionary<long, PlayerSession>();
            _sessionBySessionId = new Dictionary<long, PlayerSession>();
            _sessionsByWorldGroupAndCharacterId = new Dictionary<string, Dictionary<long, PlayerSession>>();
        }

        private PlayerSession GetSessionByAccountId(long accountId) => !_sessionByAccountId.TryGetValue(accountId, out PlayerSession session) ? null : session;

        public bool IsConnectedByAccountId(long accountId) => _sessionByAccountId.ContainsKey(accountId);

        public bool ConnectAccountOnWorldId(Guid worldId, long accountId)
        {
            PlayerSession session = GetSessionByAccountId(accountId);
            if (session.ConnectedWorld != null)
            {
                // session already authed on another world
                return false;
            }

            WorldServer world = _worldManager.GetWorldById(worldId);
            if (world == null)
            {
                return false;
            }

            session.ConnectedWorld = world;
            return true;
        }

        public void DisconnectByAccountId(long accountId)
        {
            if (!_sessionByAccountId.Remove(accountId, out PlayerSession session))
            {
                // log the error
                return;
            }

            _sessionBySessionId.Remove(session.SessionId);
        }

        public PlayerSession GetByAccountId(long accountId)
        {
            return _sessionByAccountId.TryGetValue(accountId, out PlayerSession session) ? session : null;
        }

        public PlayerSession GetByCharacterId(string worldGroup, long characterId)
        {
            if (!_sessionsByWorldGroupAndCharacterId.TryGetValue(worldGroup, out Dictionary<long, PlayerSession> playerSessions))
            {
                playerSessions = new Dictionary<long, PlayerSession>();
                _sessionsByWorldGroupAndCharacterId[worldGroup] = playerSessions;
            }

            return playerSessions.TryGetValue(characterId, out PlayerSession session) ? session : null;
        }

        public bool ConnectCharacter(Guid worldId, long characterId, long accountId, CharacterSession charSession)
        {
            var world = _worldManager.GetWorldById(worldId);
            if (!_sessionsByWorldGroupAndCharacterId.TryGetValue(world.WorldGroup, out Dictionary<long, PlayerSession> sessions))
            {
                sessions = new Dictionary<long, PlayerSession>();
                _sessionsByWorldGroupAndCharacterId[world.WorldGroup] = sessions;
            }

            PlayerSession session = GetByAccountId(accountId);
            if (session == null)
            {
                return false;
            }

            session.CharacterId = characterId;
            session.Character = charSession;
            return sessions.TryAdd(characterId, session);
        }

        public void ConnectAccount(long accountId, long sessionId, string accountName)
        {
            if (!_sessionByAccountId.TryGetValue(accountId, out PlayerSession session))
            {
                session = new PlayerSession(accountId, sessionId, accountName);
                _sessionByAccountId[accountId] = session;
            }

            _sessionBySessionId[sessionId] = session;
        }
    }
}