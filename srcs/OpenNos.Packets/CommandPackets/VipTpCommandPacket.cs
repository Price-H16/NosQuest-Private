// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$VipTP", Authority = AuthorityType.Donator)]
    public class VipTpCommandPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0, SerializeToEnd = true)]
        public string Arg { get; set; }

        public override string ToString() => "$VipTp -help";

        #endregion
    }
}