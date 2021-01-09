// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Linq;
using OpenNos.GameObject.Battle;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject.Buff
{
    public class Buff
    {
        #region Instantiation

        public Buff(int id, byte level = 0, bool isPermaBuff = false, IBattleEntity entity = null)
        {
            Card = ServerManager.Instance.Cards.FirstOrDefault(s => s.CardId == id);
            Level = level;
            IsPermaBuff = isPermaBuff;
            Entity = entity;
        }

        #endregion

        #region Properties

        public int Level;

        public Card Card { get; set; }

        public bool StaticBuff { get; set; }

        public int RemainingTime { get; set; }

        public DateTime Start { get; set; }

        public bool IsPermaBuff { get; set; }

        public IBattleEntity Entity { get; set; }

        #endregion
    }
}