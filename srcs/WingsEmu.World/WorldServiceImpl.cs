// WingsEmu
// 
// Developed by NosWings Team

using System.Threading.Tasks;
using Grpc.Core;
using OpenNos.GameObject.Networking;
using WingsEmu.Communication.RPC;

namespace WingsEmu.World
{
    public class WorldServiceImpl : global::World.WorldBase
    {
        public override Task<Void> CharacterConnected(Long request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnCharacterConnected(request.Id);
            return Task.FromResult(new Void());
        }

        public override Task<Void> CharacterDisconnected(Long request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnCharacterDisconnected(request.Id);
            return Task.FromResult(new Void());
        }

        public override Task<Void> SendMessageToCharacter(SendMessageToCharacterRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Void());
        }

        public override Task<Void> ChangeAuthority(ChangeAuthorityWorldRequest request, ServerCallContext context)
        {
            return  Task.FromResult(new Void());
        }

        public override Task<Void> KickSession(AccountIdAndSessionIdRequest request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnKickSession(request.AccountId, request.SessionId);
            return Task.FromResult(new Void());
        }

        public override Task<Void> SendMail(SendMailRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Void());
        }

        public override Task<Bool> UpdateBazaar(Long request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnUpdateBazaar(request.Id);
            return Task.FromResult(true.ToBool());
        }

        public override Task<Void> UpdateFamily(WorldUpdateFamilyRequest request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnUpdateFamily(request.FamilyId, request.ChangeFaction);
            return Task.FromResult(new Void());
        }

        public override Task<Void> UpdatePenaltyLog(Int request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnUpdatePenaltyLog(request.Id);
            return Task.FromResult(new Void());
        }

        public override Task<Void> UpdateRelation(Long request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnUpdateRelation(request.Id);
            return Task.FromResult(new Void());
        }

        public override Task<Void> Shutdown(Void request, ServerCallContext context)
        {
            CommunicationServiceEvents.Instance.OnShutdown();
            return Task.FromResult(new Void());
        }
    }
}