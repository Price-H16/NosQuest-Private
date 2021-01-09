// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Communication
{
    public interface ICommunicationClientEndPoint
    {
        string Ip { get; set; }
        int Port { get; set; }
    }
    public interface ICommunicationService
    {
        /*
         * Account
         */
        bool IsAccountConnected(long accountId);
        void DisconnectAccount(long accountId);
        bool ConnectAccount(Guid worldId, long accountId, long sessionId);
        void PulseAccount(long accountId);

        /*
         * Character
         */
        bool IsCharacterConnected(string worldGroup, long characterId);
        bool ConnectCharacter(Guid worldId, long characterId);
        void DisconnectCharacter(Guid worldId, long characterId);
        bool IsLoginPermitted(long accountId, long sessionId);
        bool ChangeAuthority(string worldGroup, string characterName, AuthorityType authority);

        /*
         * Session
         */
        void KickSession(long? accountId, long? sessionId);
        void RegisterAccountLogin(long accountId, long sessionId, string accountName, string ipAddress);
        string RetrieveServerStatistics(bool online = false);


        /*
         * Character Interaction
         */
        int? SendMessageToCharacter(SCSCharacterMessage message);
        void SendMail(string worldGroup, MailDTO mail);

        /*
         * World
         */
        int? RegisterWorldServer(SerializableWorldServer worldServer, ICommunicationClientEndPoint endPoint);
        int? GetChannelIdByWorldId(Guid worldId);
        void Shutdown(string worldGroup);
        bool GetMaintenanceState();
        void SetMaintenanceState(bool state);
        void UnregisterWorldServer(Guid worldId);
        void SetWorldServerAsInvisible(Guid worldId);


        /*
         * Session
         */
        bool ConnectCrossServerAccount(Guid worldId, long accountId, int sessionId);
        bool IsCrossServerLoginPermitted(long accountId, int sessionId);
        void RegisterCrossServerLogin(long accountId, int sessionId);

        /*
         * Act4
         */
        bool IsMasterOnline();
        SerializableWorldServer GetPreviousChannelByAccountId(long accountId);
        SerializableWorldServer GetAct4ChannelInfo(string worldGroup);

        /*
         * Update
         */
        IEnumerable<SerializableWorldServer> RetrieveRegisteredWorldServers();
        void UpdateRelation(string worldGroup, long relationId);
        void UpdateFamily(string worldGroup, long familyId, bool changeFaction);
        void UpdateBazaar(string worldGroup, long bazaarItemId);
        void Cleanup();
    }
}