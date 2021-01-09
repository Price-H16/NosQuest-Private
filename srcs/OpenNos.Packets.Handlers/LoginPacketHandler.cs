// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using OpenNos.Core;
using OpenNos.Core.Handling;
using OpenNos.Core.Logging;
using OpenNos.DAL;
using OpenNos.GameObject.Networking;
using WingsEmu.Communication;
using WingsEmu.DTOs;
using WingsEmu.Game.Extensions.PacketGeneration;
using WingsEmu.Packets.ClientPackets;
using WingsEmu.Packets.Enums;

namespace WingsEmu.PacketHandlers
{
    public class LoginPacketHandler : IPacketHandler
    {
        #region Members

        private readonly ClientSession _session;

        #endregion

        #region Instantiation

        public LoginPacketHandler(ClientSession session) => _session = session;

        #endregion

        #region Methods

        public void SendServerListPacket(int sessionId, string accountName)
        {
            IEnumerable<SerializableWorldServer> worlds = CommunicationServiceClient.Instance.RetrieveRegisteredWorldServers();

            if (worlds?.Any() == true)
            {
                string lastGroup = string.Empty;
                int worldGroupCount = 0;
                var packetBuilder = new StringBuilder();
                packetBuilder.AppendFormat("NsTeST {0} {1} ", accountName, sessionId);
                foreach (SerializableWorldServer world in worlds)
                {
                    Logger.Log.Info($"Worlds : {JsonConvert.SerializeObject(world)}");
                    if (lastGroup != world.WorldGroup)
                    {
                        worldGroupCount++;
                    }

                    lastGroup = world.WorldGroup;
                    int color = 0;
                    packetBuilder.AppendFormat("{0}:{1}:{2}:", world.EndPointIp, world.EndPointPort, color);
                    packetBuilder.AppendFormat("{0}.{1}.{2} ", worldGroupCount, world.ChannelId, world.WorldGroup);
                }

                packetBuilder.Append("-1:-1:-1:10000.10000.1");
                Logger.Log.Info("Packet : " + packetBuilder);
                _session.SendPacket(packetBuilder.ToString());
                return;
            }

            Logger.Log.Warn("Could not retrieve Worldserver groups. Please make sure they've already been registered.");
            _session.SendPacket(_session.GenerateFailcPacket(LoginFailType.Maintenance));
        }

        /// <summary>
        ///     login packet
        /// </summary>
        /// <param name="loginPacket"></param>
        public void VerifyLogin(LoginPacket loginPacket)
        {
            if (loginPacket == null)
            {
                return;
            }

            var user = new UserDTO
            {
                Name = loginPacket.Name,
                Password = loginPacket.Password
            };
            AccountDTO loadedAccount = DaoFactory.Instance.AccountDao.LoadByName(user.Name);
            if (loadedAccount == null || !loadedAccount.Password.ToUpper().Equals(user.Password))
            {
                _session.SendPacket(_session.GenerateFailcPacket(LoginFailType.AccountOrPasswordWrong));
                return;
            }

            DaoFactory.Instance.AccountDao.WriteGeneralLog(loadedAccount.AccountId, _session.IpAddress, null, GeneralLogType.Connection, "LoginServer");

            //check if the account is connected
            if (CommunicationServiceClient.Instance.IsAccountConnected(loadedAccount.AccountId))
            {
                _session.SendPacket(_session.GenerateFailcPacket(LoginFailType.AlreadyConnected));
                return;
            }

            AuthorityType type = loadedAccount.Authority;
            PenaltyLogDTO penalty = DaoFactory.Instance.PenaltyLogDao.LoadByAccount(loadedAccount.AccountId).FirstOrDefault(s => s.DateEnd > DateTime.Now && s.Penalty == PenaltyType.Banned);
            if (penalty != null)
            {
                _session.SendPacket(_session.GenerateFailcPacket(LoginFailType.Banned));
                return;
            }

            switch (type)
            {
                case AuthorityType.Banned:
                    _session.SendPacket($"failc {(byte)LoginFailType.Banned}");
                    break;

                case AuthorityType.Unconfirmed:
                case AuthorityType.Closed:
                    _session.SendPacket(_session.GenerateFailcPacket(LoginFailType.CantConnect));
                    break;

                default:
                {
                    int newSessionId = SessionFactory.Instance.GenerateSessionId();
                    Logger.Log.Debug(string.Format(Language.Instance.GetMessageFromKey("CONNECTION"), user.Name, newSessionId));

                    if (CommunicationServiceClient.Instance.GetMaintenanceState() && loadedAccount.Authority < AuthorityType.GameMaster)
                    {
                        _session.SendPacket(_session.GenerateFailcPacket(LoginFailType.Maintenance));
                        return;
                    }

                    try
                    {
                        CommunicationServiceClient.Instance.RegisterAccountLogin(loadedAccount.AccountId, newSessionId, loadedAccount.Name, _session.IpAddress);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("General Error SessionId: " + newSessionId, ex);
                        _session.SendPacket(_session.GenerateFailcPacket(LoginFailType.CantConnect));
                        return;
                    }

                    SendServerListPacket(newSessionId, loadedAccount.Name);
                }
                    break;
            }
        }

        #endregion
    }
}