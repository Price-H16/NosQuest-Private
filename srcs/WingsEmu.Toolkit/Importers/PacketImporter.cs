// WingsEmu
// 
// Developed by NosWings Team

using System.IO;
using System.Text;

namespace WingsEmu.Toolkit.Importers
{
    public class PacketImporter
    {
        private readonly ImportConfiguration _configuration;
        private string FilePacket => $"{_configuration.Folder}/packet.txt";
        public PacketImporter(ImportConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Import()
        {
            using (var packetTxtStream = new StreamReader(FilePacket, Encoding.GetEncoding(1252)))
            {
                string line;
                while ((line = packetTxtStream.ReadLine()) != null)
                {
                    string[] linesave = line.Split(' ');
                    _configuration.Packets.Add(linesave);
                }
            }
        }
    }
}