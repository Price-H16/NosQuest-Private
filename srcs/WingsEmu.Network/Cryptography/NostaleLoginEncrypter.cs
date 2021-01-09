// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Text;

namespace WingsEmu.Network.Cryptography
{
    public class NostaleLoginEncrypter : IEncrypter
    {
        private readonly Encoding _encoding;

        public NostaleLoginEncrypter(Encoding encoding) => _encoding = encoding;

        public ReadOnlyMemory<byte> Encode(string packet)
        {
            byte[] tmp = _encoding.GetBytes(packet);
            if (tmp.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < packet.Length; i++)
            {
                tmp[i] = Convert.ToByte(tmp[i] + 15);
            }

            tmp[tmp.Length - 1] = 25;
            return tmp;
        }
    }
}