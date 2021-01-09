// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$PetExp", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class PetExpPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public int Amount { get; set; }

        public static string ReturnHelp() => "$PetExp AMOUNT";
    }
}