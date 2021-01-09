// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Linq;
using OpenNos.DAL;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Character;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Event.MINILANDREFRESH
{
    public class MinilandRefresh
    {
        #region Methods

        public static void GenerateMinilandEvent()
        {
            ServerManager.Instance.SaveAll();
            foreach (CharacterDTO chara in DaoFactory.Instance.CharacterDao.LoadAll())
            {
                GeneralLogDTO gen = DaoFactory.Instance.GeneralLogDao.LoadByAccount(null).LastOrDefault(s =>
                    s.LogData == "MinilandRefresh" && s.LogType == "World" && s.Timestamp.Day == DateTime.Now.Day);
                int count = DaoFactory.Instance.GeneralLogDao.LoadByAccount(chara.AccountId).Count(s =>
                    s.LogData == "MINILAND" && s.Timestamp > DateTime.Now.AddDays(-1) &&
                    s.CharacterId == chara.CharacterId);

                ClientSession session = ServerManager.Instance.GetSessionByCharacterId(chara.CharacterId);
                if (session != null)
                {
                    session.Character.GetReput(2 * count, true);
                    session.Character.MinilandPoint = 2000;
                }
                else if (CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup,
                    chara.CharacterId))
                {
                    if (gen == null)
                    {
                        chara.Reput += 2 * count;
                    }

                    chara.MinilandPoint = 2000;
                    CharacterDTO chara2 = chara;
                    DaoFactory.Instance.CharacterDao.InsertOrUpdate(ref chara2);
                }
            }

            DaoFactory.Instance.GeneralLogDao.Insert(new GeneralLogDTO
            {
                LogData = "MinilandRefresh",
                LogType = "World",
                Timestamp = DateTime.Now
            });
            ServerManager.Instance.StartedEvents.Remove(EventType.MINILANDREFRESHEVENT);
        }

        #endregion
    }
}