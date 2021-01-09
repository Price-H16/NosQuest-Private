// WingsEmu
// 
// Developed by NosWings Team

using WingsEmu.Packets.Enums;

namespace WingsEmu.Packets.CommandPackets
{
    [PacketHeader("$Maintenance", PassNonParseablePacket = true, Authority = AuthorityType.Administrator)]
    public class MaintenancePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public string TimeBeforeMaintenance { get; set; }

        [PacketIndex(1)]
        public ServerState State { get; set; }

        public override string ToString() => "Maintenance TimeBeforeMaintenance State";

        #endregion
    }
}