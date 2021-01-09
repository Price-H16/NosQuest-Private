// WingsEmu
// 
// Developed by NosWings Team

using OpenNos.GameObject.Networking;
using WingsEmu.Packets.Enums;

namespace WingsEmu.Game.Extensions.PacketGeneration
{
    public static class LoginPacketsExtensions
    {
        public static string GenerateFailcPacket(this ClientSession session, LoginFailType failType)
        {
            return $"failc {(short)failType}";
        }
    }
}