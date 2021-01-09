// WingsEmu
// 
// Developed by NosWings Team

using Grpc.Core;

namespace WingsEmu.Communication.RPC
{
    public class WorldClientFactory : ICommunicationClientFactory
    {
        public ICommunicationClient CreateClient(string ip, int port)
        {
            var channel = new Channel(ip, port, ChannelCredentials.Insecure);
            channel.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            return new WorldCommunicator(new World.WorldClient(channel));
        }
    }
}