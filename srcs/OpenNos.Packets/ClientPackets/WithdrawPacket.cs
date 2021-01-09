// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("withdraw")]
    public class WithdrawPacket : PacketDefinition
    {
        #region Properties        

        [PacketIndex(0)]
        public short Slot { get; set; }

        [PacketIndex(1)]
        public byte Amount { get; set; }

        [PacketIndex(2)]
        public bool PetBackpack { get; set; }

        #endregion
    }
}