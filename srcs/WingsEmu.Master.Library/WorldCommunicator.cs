// WingsEmu
// 
// Developed by NosWings Team

using System;
using WingsEmu.DTOs;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Communication.RPC
{
    public class WorldCommunicator : ICommunicationClient
    {
        private readonly World.WorldClient _proxy;

        public WorldCommunicator(World.WorldClient proxy) => _proxy = proxy;

        public void UpdateBazaar(long bazaarItemId)
        {
            _proxy.UpdateBazaar(bazaarItemId.ToLong());
        }

        public void CharacterConnected(long characterId)
        {
            _proxy.CharacterConnected(characterId.ToLong());
        }

        public void CharacterDisconnected(long characterId)
        {
            _proxy.CharacterDisconnected(characterId.ToLong());
        }

        public void UpdateFamily(long familyId, bool changeFaction)
        {
            _proxy.UpdateFamily(new WorldUpdateFamilyRequest
            {
                FamilyId = familyId,
                ChangeFaction = changeFaction
            });
        }

        public void SendMessageToCharacter(SCSCharacterMessage message)
        {
            // 
        }

        public void Shutdown()
        {
            _proxy.Shutdown(new Void());
        }

        public void UpdatePenaltyLog(int penaltyLogId)
        {
            _proxy.UpdatePenaltyLog(penaltyLogId.ToInt());
        }

        public void UpdateRelation(long relationId)
        {
            _proxy.UpdateRelation(relationId.ToLong());
        }

        public void KickSession(long? accountId, long? sessionId)
        {
            _proxy.KickSession(new AccountIdAndSessionIdRequest
            {
                AccountId = accountId ?? 0,
                SessionId = sessionId ?? 0
            });
        }

        public void SendMail(MailDTO mail)
        {
            throw new NotImplementedException();
        }

        public void ChangeAuthority(long accountId, AuthorityType authority)
        {
            _proxy.ChangeAuthority(new ChangeAuthorityWorldRequest
            {
                AccountId = accountId,
                AuthorityType = (int)authority
            });
        }
    }
}