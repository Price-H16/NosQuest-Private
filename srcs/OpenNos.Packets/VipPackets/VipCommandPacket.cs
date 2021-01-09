// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.VipPackets
{
    [PacketHeader("$VipTP", Authority = AuthorityType.Donator)]
    public class VipCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Arg { get; set; }

        public IEnumerable<string> Help() => new List<string>();

        public override string ToString() => "$VipTp -help";

        #endregion
    }
}