// WingsEmu
// 
// Developed by NosWings Team

using System.Collections.Generic;
using System.Linq;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;

namespace OpenNos.GameObject
{
    public class Account : AccountDTO
    {
        #region Properties

        public List<PenaltyLogDTO> PenaltyLogs
        {
            get
            {
                PenaltyLogDTO[] logs = new PenaltyLogDTO[ServerManager.Instance.PenaltyLogs.Count + 10];
                ServerManager.Instance.PenaltyLogs.CopyTo(logs);
                return logs.Where(s => s != null && s.AccountId == AccountId).ToList();
            }
        }

        #endregion

    }
}