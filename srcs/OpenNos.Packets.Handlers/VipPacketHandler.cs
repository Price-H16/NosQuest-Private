// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.Core.Handling;
using OpenNos.GameObject.Networking;

namespace WingsEmu.PacketHandlers
{
    public class VipPacketHandler : IPacketHandler
    {
        #region Instantiation

        public VipPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion
    }
}