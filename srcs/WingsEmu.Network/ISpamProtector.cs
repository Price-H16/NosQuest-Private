// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Network
{
    public interface ISpamProtector
    {
        bool CanConnect(string ipAddress);
    }
}