// WingsEmu
// 
// Developed by NosWings Team

using System.Threading.Tasks;
using Grpc.Core;
using OpenNos.Core;
using OpenNos.Core.Logging;

namespace WingsEmu.Communication.RPC
{
    public interface ICommunicationServiceFactory
    {
        Task<ICommunicationService> CreateService(string ipAddress, int port);
    }

    public class GRpcCommunicationServiceFactory : ICommunicationServiceFactory
    {
        public async Task<ICommunicationService> CreateService(string ipAddress, int port)
        {
            var channel = new Channel(ipAddress, port, ChannelCredentials.Insecure);
            Logger.Log.Info($"[MASTER_AUTH] Connecting to {ipAddress}:{port}");
            await channel.ConnectAsync();
            Logger.Log.Info("[MASTER_AUTH] connected !");
            return new MasterCommunicator(new Master.MasterClient(channel));
        }
    }
}