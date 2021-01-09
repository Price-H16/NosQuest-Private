// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.HomePackets
{
    [PacketHeader("$SetHome", Authority = AuthorityType.User, PassNonParseablePacket = true)]
    public class SetHomePacket : PacketDefinition
    {
        [PacketIndex(0)]
        public string Name { get; set; }

        public override string ToString() => "$SetHome HOMDEID";
    }
}