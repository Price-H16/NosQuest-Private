// WingsEmu
// 
// Developed by NosWings Team

using System.Text;

namespace WingsEmu.Network.Cryptography
{
    public interface INetworkClientInformation
    {
        int SessionId { get; }
        Encoding Encoding { get; }
    }
}