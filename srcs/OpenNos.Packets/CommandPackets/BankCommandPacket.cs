// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Bank", PassNonParseablePacket = true, Authority = AuthorityType.User)]
    public class BankCommandPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public string Subcommand { get; set; }

        [PacketIndex(1)]
        public long? Amount { get; set; }

        [PacketIndex(2)]
        public string Target { get; set; }

        public static string ReturnHelp() => "$Bank [Balance/Deposit/Top/Transfer/Withdraw] <Amount> <Target>";
    }
}