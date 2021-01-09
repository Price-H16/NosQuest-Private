// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.Network.Cryptography
{
    public interface IDecrypter
    {
        string Decode(ReadOnlySpan<byte> bytesBuffer);
    }
}