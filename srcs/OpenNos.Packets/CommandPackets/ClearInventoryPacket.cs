// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Clear", PassNonParseablePacket = true, Authority = AuthorityType.GameMaster)]
    public class ClearInventoryPacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public InventoryType InventoryType { get; set; }

        public static string ReturnHelp() => "$Clear INVENTORYTYPE";

        #endregion
    }
}