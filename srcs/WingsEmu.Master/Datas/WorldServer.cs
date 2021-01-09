// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.Master.Datas
{
    public class WorldServer
    {
        public int AccountLimit { get; set; }

        public int ChannelId { get; set; }

        public string Endpoint { get; set; }
        public int Port { get; set; }

        public Guid Id { get; set; }

        public string WorldGroup { get; set; }

        public bool IsAct4 { get; set; }

        public bool IsInvisible { get; set; }
    }
}