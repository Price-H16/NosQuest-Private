// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;

namespace WingsEmu.Toolkit.Importers
{
    public class ImportConfiguration
    {
        public string Folder { get; set; }
        public string Lang { get; set; }
        public List<string[]> Packets { get; set; }
    }
}