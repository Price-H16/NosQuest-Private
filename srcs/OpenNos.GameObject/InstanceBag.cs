// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using OpenNos.GameObject.Event;

namespace OpenNos.GameObject
{
    public class InstanceBag : IDisposable
    {
        #region Instantiation

        public InstanceBag()
        {
            Clock = new Clock(1);
            DeadList = new ConcurrentBag<long>();
            UnlockEvents = new ConcurrentBag<EventContainer>();
            ButtonLocker = new Locker();
            MonsterLocker = new Locker();
        }

        #endregion

        #region Methods

        public string GenerateScore() => $"rnsc {Point}";

        #endregion

        #region Properties

        public Clock Clock { get; set; }

        public int Combo { get; set; }

        public long Creator { get; set; }

        public ConcurrentBag<long> DeadList { get; set; }

        public byte EndState { get; set; }

        public short Lives { get; set; }

        public bool Lock { get; set; }

        public int MonstersKilled { get; set; }

        public int NpcsKilled { get; set; }

        public int Point { get; set; }

        public int RoomsVisited { get; set; }
        public Locker MonsterLocker { get; set; }
        public Locker ButtonLocker { get; set; }
        public ConcurrentBag<EventContainer> UnlockEvents { get; set; }

        public void Dispose()
        {
            Clock.Dispose();
        }

        #endregion
    }
}