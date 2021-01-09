// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.Network.Cryptography
{
    public interface IEncrypter
    {
        ReadOnlyMemory<byte> Encode(string packet);
    }
}