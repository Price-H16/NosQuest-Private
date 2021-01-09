// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Communication.RPC
{
    public interface ICommunicationClientFactory
    {
        ICommunicationClient CreateClient(string ip, int port);
    }
}