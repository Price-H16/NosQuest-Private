﻿// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace OpenNos.GameObject.Event
{
    public class PercentBar
    {
        private readonly DateTime _nextMonth;

        private DateTime _latestUpdate;

        private int _percentage;

        private short _totalTime;

        public PercentBar()
        {
            DateTime olddate = DateTime.Now.AddMonths(1);
            _nextMonth = new DateTime(olddate.Year, olddate.Month, 1, 0, 0, 0, olddate.Kind);
            _latestUpdate = DateTime.Now;
        }

        public int MinutesUntilReset => (int)(_nextMonth - DateTime.Now).TotalMinutes;

        public byte Mode { get; set; }

        public int Percentage
        {
            get => Mode == 0 ? _percentage : 0;
            set => _percentage = value;
        }

        public short CurrentTime => Mode == 0 ? (short)0 : (short)(_latestUpdate.AddSeconds(_totalTime) - DateTime.Now).TotalSeconds;

        public short TotalTime
        {
            get => Mode == 0 ? (short)0 : _totalTime;
            set
            {
                _latestUpdate = DateTime.Now;
                _totalTime = value;
            }
        }

        public int KilledMonsters { get; set; }

        public bool IsMorcos { get; set; }

        public bool IsHatus { get; set; }

        public bool IsCalvina { get; set; }

        public bool IsBerios { get; set; }
    }
}