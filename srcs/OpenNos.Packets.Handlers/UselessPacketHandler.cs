// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.Core.Handling;
using OpenNos.GameObject.Networking;
using WingsEmu.Packets.ClientPackets;

namespace WingsEmu.PacketHandlers
{
    public class UselessPacketHandler : IPacketHandler
    {
        #region Instantiation

        public UselessPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Members

        #endregion

        #region Methods

        public void ScpCts(ScpCtsPacket packet)
        {
            // idk
        }

        public void CClose(CClosePacket packet)
        {
            // idk
        }

        public void FStashEnd(FStashEndPacket packet)
        {
            // idk
        }

        public void FStashEnd(StashEndPacket packet)
        {
            // idk
        }

        public void Lbs(LbsPacket packet)
        {
            // idk
        }

        public void ShopClose(ShopClosePacket packet)
        {
            // Not needed for now.
        }

        public void Snap(SnapPacket packet)
        {
            // Not needed for now. (pictures)
        }

        #endregion
    }
}