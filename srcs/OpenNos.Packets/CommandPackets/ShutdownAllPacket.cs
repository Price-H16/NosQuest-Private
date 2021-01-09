// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$ShutdownAll", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ShutdownAllPacket : PacketDefinition
    {
        [PacketIndex(0)]
        public string WorldGroup { get; set; }

        public static string ReturnHelp() => "$ShutdownAll WORLDGROUP(*)";

        public override string ToString() => $"ShutdownAll Command WorldGroup: {WorldGroup}";
    }
}