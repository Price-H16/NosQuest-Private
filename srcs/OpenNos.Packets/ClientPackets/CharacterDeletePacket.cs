// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Packets.ClientPackets
{
    [PacketHeader("Char_DEL")]
    public class CharacterDeletePacket : PacketDefinition
    {
        #region Properties

        [PacketIndex(0)]
        public byte Slot { get; set; }

        [PacketIndex(1)]
        public string Password { get; set; }

        public override string ToString() => $"Delete Character Slot {Slot}";

        #endregion
    }
}