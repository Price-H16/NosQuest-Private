// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace WingsEmu.DAL.EF.DAO
{
    public class DatabaseConfiguration
    {
        public DatabaseConfiguration()
        {
            Ip = Environment.GetEnvironmentVariable("DATABASE_IP") ?? "localhost";
            Username = Environment.GetEnvironmentVariable("DATABASE_USER") ?? "sa";
            Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "strong_pass2018";
            Database = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "noswings_game";
            if (!ushort.TryParse(Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "1433", out ushort port))
            {
                port = 1433;
            }

            Port = port;
        }

        public string Ip { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public ushort Port { get; set; }

        public override string ToString() => $"Server={Ip},{Port};User id={Username};Password={Password};Initial Catalog={Database};";
    }
}