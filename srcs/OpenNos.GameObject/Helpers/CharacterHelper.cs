﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Diagnostics.CodeAnalysis;
using OpenNos.GameObject.Networking;
using WingsEmu.Packets.Enums;

namespace OpenNos.GameObject.Helpers
{
    public class CharacterHelper
    {
        #region Instantiation

        public CharacterHelper()
        {
            LoadSpeedData();
            LoadJobXpData();
            LoadSpxpData();
            LoadHeroXpData();
            LoadXpData();
            LoadHpData();
            LoadMpData();
            LoadStats();
            LoadHpHealth();
            LoadMpHealth();
            LoadHpHealthStand();
            LoadMpHealthStand();
        }

        #endregion

        #region Members

        private int[,] _criticalDist;
        private int[,] _criticalDistRate;
        private int[,] _criticalHit;
        private int[,] _criticalHitRate;
        private int[,] _distDef;
        private int[,] _distDodge;
        private int[,] _distRate;
        private int[,] _hitDef;
        private int[,] _hitDodge;
        private int[,] _hitRate;
        private int[,] _magicalDef;
        private int[,] _maxDist;
        private int[,] _maxHit;
        private int[,] _minDist;

        // difference between class
        private int[,] _minHit;

        // STAT DATA

        // same for all class

        #endregion

        #region Properties

        public double[] FirstJobXpData { get; private set; }

        public double[] HeroXpData { get; private set; }

        public int[,] HpData { get; private set; }

        public int[] HpHealth { get; private set; }

        public int[] HpHealthStand { get; private set; }

        public int[,] MpData { get; private set; }

        public int[] MpHealth { get; private set; }

        public int[] MpHealthStand { get; private set; }

        public double[] SecondJobXpData { get; private set; }

        public byte[] SpeedData { get; private set; }

        public double[] SpxpData { get; private set; }

        public double[] XpData { get; private set; }

        #endregion

        #region Methods

        public void AddSpecialistWingsBuff(ClientSession session)
        {
            if (!session.Character.UseSp || session.Character.SpInstance == null)
            {
                return;
            }

            RemoveSpecialistWingsBuff(session);
            switch (session.Character.SpInstance.Design)
            {
                case 6:
                    session.Character.AddBuff(new Buff.Buff(387, isPermaBuff: true));
                    break;
                case 7:
                    session.Character.AddBuff(new Buff.Buff(395, isPermaBuff: true));
                    break;
                case 8:
                    session.Character.AddBuff(new Buff.Buff(396, isPermaBuff: true));
                    break;
                case 9:
                    session.Character.AddBuff(new Buff.Buff(397, isPermaBuff: true));
                    break;
                case 10:
                    session.Character.AddBuff(new Buff.Buff(398, isPermaBuff: true));
                    break;
                case 11:
                    session.Character.AddBuff(new Buff.Buff(410, isPermaBuff: true));
                    break;
                case 12:
                    session.Character.AddBuff(new Buff.Buff(411, isPermaBuff: true));
                    break;
                case 13:
                    session.Character.AddBuff(new Buff.Buff(444, isPermaBuff: true));
                    break;
                case 14:
                    session.Character.AddBuff(new Buff.Buff(663, isPermaBuff: true));
                    break;
            }
        }

        public void RemoveSpecialistWingsBuff(ClientSession session)
        {
            session.Character.RemoveBuff(387, true);
            session.Character.RemoveBuff(395, true);
            session.Character.RemoveBuff(396, true);
            session.Character.RemoveBuff(397, true);
            session.Character.RemoveBuff(398, true);
            session.Character.RemoveBuff(410, true);
            session.Character.RemoveBuff(411, true);
            session.Character.RemoveBuff(444, true);
        }

        public static float ExperiencePenalty(byte playerLevel, byte monsterLevel)
        {
            int leveldifference = playerLevel - monsterLevel;
            float penalty;

            // penalty calculation
            switch (leveldifference)
            {
                case 6:
                    penalty = 0.9f;
                    break;

                case 7:
                    penalty = 0.7f;
                    break;

                case 8:
                    penalty = 0.5f;
                    break;

                case 9:
                    penalty = 0.3f;
                    break;

                default:
                    if (leveldifference > 9)
                    {
                        penalty = 0.1f;
                    }
                    else if (leveldifference > 18)
                    {
                        penalty = 0.05f;
                    }
                    else
                    {
                        penalty = 1f;
                    }

                    break;
            }

            return penalty;
        }

        public static float GoldPenalty(byte playerLevel, byte monsterLevel)
        {
            int leveldifference = playerLevel - monsterLevel;
            float penalty;

            // penalty calculation
            switch (leveldifference)
            {
                case 5:
                    penalty = 0.9f;
                    break;

                case 6:
                    penalty = 0.7f;
                    break;

                case 7:
                    penalty = 0.5f;
                    break;

                case 8:
                    penalty = 0.3f;
                    break;

                case 9:
                    penalty = 0.2f;
                    break;

                default:
                    if (leveldifference > 9 && leveldifference < 19)
                    {
                        penalty = 0.1f;
                    }
                    else if (leveldifference > 18 && leveldifference < 30)
                    {
                        penalty = 0.05f;
                    }
                    else if (leveldifference > 30)
                    {
                        penalty = 0f;
                    }
                    else
                    {
                        penalty = 1f;
                    }

                    break;
            }

            return penalty;
        }

        public static long LoadFairyXpData(long elementRate)
        {
            if (elementRate < 40)
            {
                return elementRate * elementRate + 50;
            }

            return elementRate * elementRate * 3 + 50;
        }

        public static int LoadFamilyXpData(byte familyLevel)
        {
            switch (familyLevel)
            {
                case 1:
                    return 100000;

                case 2:
                    return 250000;

                case 3:
                    return 370000;

                case 4:
                    return 560000;

                case 5:
                    return 840000;

                case 6:
                    return 1260000;

                case 7:
                    return 1900000;

                case 8:
                    return 2850000;

                case 9:
                    return 3570000;

                case 10:
                    return 3830000;

                case 11:
                    return 4150000;

                case 12:
                    return 4750000;

                case 13:
                    return 5500000;

                case 14:
                    return 6500000;

                case 15:
                    return 7000000;

                case 16:
                    return 8500000;

                case 17:
                    return 9500000;

                case 18:
                    return 10000000;

                case 19:
                    return 17000000;

                default:
                    return 999999999;
            }
        }

        public int MagicalDefence(ClassType @class, byte level) => _magicalDef[(byte)@class, level];

        public int MaxDistance(ClassType @class, byte level) => _maxDist[(byte)@class, level];

        public int MaxHit(ClassType @class, byte level) => _maxHit[(byte)@class, level];

        public int MinDistance(ClassType @class, byte level) => _minDist[(byte)@class, level];

        public int MinHit(ClassType @class, byte level) => _minHit[(int)@class, level];

        public int RarityPoint(short rarity, short lvl)
        {
            int p;
            switch (rarity)
            {
                case 0:
                    p = 0;
                    break;

                case 1:
                    p = 1;
                    break;

                case 2:
                    p = 2;
                    break;

                case 3:
                    p = 3;
                    break;

                case 4:
                    p = 4;
                    break;

                case 5:
                    p = 5;
                    break;

                case 6:
                    p = 7;
                    break;

                case 7:
                    p = 10;
                    break;

                case 8:
                    p = 15;
                    break;

                default:
                    p = rarity * 2;
                    break;
            }

            return p * (lvl / 5 + 1);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted",
            Justification = "Easier to read")]
        public int SlPoint(short spPoint, short mode)
        {
            try
            {
                int point = 0;
                switch (mode)
                {
                    case 0:
                        if (spPoint <= 10)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 28)
                        {
                            point = 10 + (spPoint - 10) / 2;
                        }
                        else if (spPoint <= 88)
                        {
                            point = 19 + (spPoint - 28) / 3;
                        }
                        else if (spPoint <= 168)
                        {
                            point = 39 + (spPoint - 88) / 4;
                        }
                        else if (spPoint <= 268)
                        {
                            point = 59 + (spPoint - 168) / 5;
                        }
                        else if (spPoint <= 334)
                        {
                            point = 79 + (spPoint - 268) / 6;
                        }
                        else if (spPoint <= 383)
                        {
                            point = 90 + (spPoint - 334) / 7;
                        }
                        else if (spPoint <= 391)
                        {
                            point = 97 + (spPoint - 383) / 8;
                        }
                        else if (spPoint <= 400)
                        {
                            point = 98 + (spPoint - 391) / 9;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 99 + (spPoint - 400) / 10;
                        }

                        break;

                    case 2:
                        if (spPoint <= 20)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 40)
                        {
                            point = 20 + (spPoint - 20) / 2;
                        }
                        else if (spPoint <= 70)
                        {
                            point = 30 + (spPoint - 40) / 3;
                        }
                        else if (spPoint <= 110)
                        {
                            point = 40 + (spPoint - 70) / 4;
                        }
                        else if (spPoint <= 210)
                        {
                            point = 50 + (spPoint - 110) / 5;
                        }
                        else if (spPoint <= 270)
                        {
                            point = 70 + (spPoint - 210) / 6;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 80 + (spPoint - 270) / 7;
                        }

                        break;

                    case 1:
                        if (spPoint <= 10)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 48)
                        {
                            point = 10 + (spPoint - 10) / 2;
                        }
                        else if (spPoint <= 81)
                        {
                            point = 29 + (spPoint - 48) / 3;
                        }
                        else if (spPoint <= 161)
                        {
                            point = 40 + (spPoint - 81) / 4;
                        }
                        else if (spPoint <= 236)
                        {
                            point = 60 + (spPoint - 161) / 5;
                        }
                        else if (spPoint <= 290)
                        {
                            point = 75 + (spPoint - 236) / 6;
                        }
                        else if (spPoint <= 360)
                        {
                            point = 84 + (spPoint - 290) / 7;
                        }
                        else if (spPoint <= 400)
                        {
                            point = 97 + (spPoint - 360) / 8;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 99 + (spPoint - 400) / 10;
                        }

                        break;

                    case 3:
                        if (spPoint <= 10)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 50)
                        {
                            point = 10 + (spPoint - 10) / 2;
                        }
                        else if (spPoint <= 110)
                        {
                            point = 30 + (spPoint - 50) / 3;
                        }
                        else if (spPoint <= 150)
                        {
                            point = 50 + (spPoint - 110) / 4;
                        }
                        else if (spPoint <= 200)
                        {
                            point = 60 + (spPoint - 150) / 5;
                        }
                        else if (spPoint <= 260)
                        {
                            point = 70 + (spPoint - 200) / 6;
                        }
                        else if (spPoint <= 330)
                        {
                            point = 80 + (spPoint - 260) / 7;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 90 + (spPoint - 330) / 8;
                        }

                        break;
                }

                return point;
            }
            catch
            {
                return 0;
            }
        }

        public int SpPoint(short spLevel, short upgrade)
        {
            int point = (spLevel - 20) * 3;
            if (spLevel <= 20)
            {
                point = 0;
            }

            switch (upgrade)
            {
                case 1:
                    point += 5;
                    break;

                case 2:
                    point += 10;
                    break;

                case 3:
                    point += 15;
                    break;

                case 4:
                    point += 20;
                    break;

                case 5:
                    point += 28;
                    break;

                case 6:
                    point += 36;
                    break;

                case 7:
                    point += 46;
                    break;

                case 8:
                    point += 56;
                    break;

                case 9:
                    point += 68;
                    break;

                case 10:
                    point += 80;
                    break;

                case 11:
                    point += 95;
                    break;

                case 12:
                    point += 110;
                    break;

                case 13:
                    point += 128;
                    break;

                case 14:
                    point += 148;
                    break;

                case 15:
                    point += 173;
                    break;
            }

            if (upgrade > 15)
            {
                point += 173 + 25 + 5 * (upgrade - 15);
            }

            return point;
        }

        internal int DarkResistance(ClassType @class, byte level) => 0;

        internal int Defence(ClassType @class, byte level) => _hitDef[(byte)@class, level];

        /// <summary>
        ///     Defence rate base stats for Character by Class & Level
        /// </summary>
        /// <param name="class"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal int DefenceRate(ClassType @class, byte level) => _hitDodge[(byte)@class, level];

        /// <summary>
        ///     Distance Defence base stats for Character by Class & Level
        /// </summary>
        /// <param name="class"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal int DistanceDefence(ClassType @class, byte level) => _distDef[(byte)@class, level];

        /// <summary>
        ///     Distance Defence Rate base stats for Character by Class & Level
        /// </summary>
        /// <param name="class"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal int DistanceDefenceRate(ClassType @class, byte level) => _distDodge[(byte)@class, level];

        /// <summary>
        ///     Distance Rate base stats for Character by Class & Level
        /// </summary>
        /// <param name="class"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal int DistanceRate(ClassType @class, byte level) => _distRate[(byte)@class, level];

        internal int DistCritical(ClassType @class, byte level) => _criticalDist[(byte)@class, level];

        internal int DistCriticalRate(ClassType @class, byte level) => _criticalDistRate[(byte)@class, level];

        internal int Element(ClassType @class, byte level) => 0;

        internal int ElementRate(ClassType @class, byte level) => 0;

        internal int FireResistance(ClassType @class, byte level) => 0;

        internal int HitCritical(ClassType @class, byte level) => _criticalHit[(byte)@class, level];

        internal int HitCriticalRate(ClassType @class, byte level) => _criticalHitRate[(byte)@class, level];

        internal int HitRate(ClassType @class, byte level) => _hitRate[(byte)@class, level];

        internal int LightResistance(ClassType @class, byte level) => 0;

        internal int WaterResistance(ClassType @class, byte level) => 0;

        private void LoadHeroXpData()
        {
            int index = 1;
            int increment = 118980;
            int increment2 = 9120;
            int increment3 = 360;

            HeroXpData = new double[256];
            HeroXpData[0] = 949560;
            for (int lvl = 1; lvl < HeroXpData.Length; lvl++)
            {
                HeroXpData[lvl] = HeroXpData[lvl - 1] + increment;
                increment2 += increment3;
                increment = increment + increment2;
                index++;
                if ((index % 10) == 0)
                {
                    if (index / 10 < 3)
                    {
                        increment3 -= index / 10 * 30;
                    }
                    else
                    {
                        increment3 -= 30;
                    }
                }
            }
        }

        private void LoadHpData()
        {
            HpData = new int[5, 256];

            // Adventurer HP
            for (int i = 1; i < HpData.GetLength(1); i++)
            {
                HpData[(int)ClassType.Adventurer, i] = (int)(1 / 2.0 * i * i + 31 / 2.0 * i + 205);
            }

            // Swordsman HP
            for (int i = 0; i < HpData.GetLength(1); i++)
            {
                int j = 16;
                int hp = 946;
                int inc = 85;
                while (j <= i)
                {
                    if ((j % 5) == 2)
                    {
                        hp += inc / 2;
                        inc += 2;
                    }
                    else
                    {
                        hp += inc;
                        inc += 4;
                    }

                    ++j;
                }

                HpData[(int)ClassType.Swordman, i] = hp;
            }

            // Magician HP
            for (int i = 0; i < HpData.GetLength(1); i++)
            {
                HpData[(int)ClassType.Magician, i] = (int)((i + 15) * (i + 15) + i + 15.0 - 465 + 550);
            }

            // Archer HP
            for (int i = 0; i < HpData.GetLength(1); i++)
            {
                int hp = 680;
                int inc = 35;
                int j = 16;
                while (j <= i)
                {
                    hp += inc;
                    ++inc;
                    if ((j % 10) == 1 || (j % 10) == 5 || (j % 10) == 8)
                    {
                        hp += inc;
                        ++inc;
                    }

                    ++j;
                }

                HpData[(int)ClassType.Archer, i] = hp;
            }

            // Martial artist HP
            for (int i = 0; i < HpData.GetLength(1); i++)
            {
                int j = 16;
                int hp = 946;
                int inc = 85;
                while (j <= i)
                {
                    if ((j % 5) == 2)
                    {
                        hp += inc / 2;
                        inc += 2;
                    }
                    else
                    {
                        hp += inc;
                        inc += 4;
                    }

                    ++j;
                }

                HpData[(int)ClassType.Wrestler, i] = hp;
            }
        }

        private void LoadHpHealth()
        {
            HpHealth = new int[5];
            HpHealth[(int)ClassType.Archer] = 60;
            HpHealth[(int)ClassType.Adventurer] = 30;
            HpHealth[(int)ClassType.Swordman] = 90;
            HpHealth[(int)ClassType.Magician] = 30;
            HpHealth[(int)ClassType.Wrestler] = 90;
        }

        private void LoadHpHealthStand()
        {
            HpHealthStand = new int[5];
            HpHealthStand[(int)ClassType.Archer] = 32;
            HpHealthStand[(int)ClassType.Adventurer] = 25;
            HpHealthStand[(int)ClassType.Swordman] = 26;
            HpHealthStand[(int)ClassType.Magician] = 20;
            HpHealthStand[(int)ClassType.Wrestler] = 26;
        }

        private void LoadJobXpData()
        {
            // Load JobData
            FirstJobXpData = new double[21];
            SecondJobXpData = new double[256];
            FirstJobXpData[0] = 2200;
            SecondJobXpData[0] = 17600;
            for (int i = 1; i < FirstJobXpData.Length; i++)
            {
                FirstJobXpData[i] = FirstJobXpData[i - 1] + 700;
            }

            for (int i = 1; i < SecondJobXpData.Length; i++)
            {
                int var2 = 400;
                if (i > 3)
                {
                    var2 = 4500;
                }

                if (i > 40)
                {
                    var2 = 15000;
                }

                SecondJobXpData[i] = SecondJobXpData[i - 1] + var2;
            }
        }

        private void LoadMpData()
        {
            MpData = new int[5, 257];

            // ADVENTURER MP
            MpData[(int)ClassType.Adventurer, 0] = 60;
            int baseAdventurer = 9;
            for (int i = 1; i < MpData.GetLength(1); i += 4)
            {
                MpData[(int)ClassType.Adventurer, i] = MpData[(int)ClassType.Adventurer, i - 1] + baseAdventurer;
                MpData[(int)ClassType.Adventurer, i + 1] = MpData[(int)ClassType.Adventurer, i] + baseAdventurer;
                MpData[(int)ClassType.Adventurer, i + 2] = MpData[(int)ClassType.Adventurer, i + 1] + baseAdventurer;
                baseAdventurer++;
                MpData[(int)ClassType.Adventurer, i + 3] = MpData[(int)ClassType.Adventurer, i + 2] + baseAdventurer;
                baseAdventurer++;
            }

            // SWORDSMAN MP
            for (int i = 1; i < MpData.GetLength(1) - 1; i++)
            {
                MpData[(int)ClassType.Swordman, i] = MpData[(int)ClassType.Adventurer, i];
            }

            // ARCHER MP
            for (int i = 0; i < MpData.GetLength(1) - 1; i++)
            {
                MpData[(int)ClassType.Archer, i] = MpData[(int)ClassType.Adventurer, i + 1];
            }

            // MAGICIAN MP
            for (int i = 0; i < MpData.GetLength(1) - 1; i++)
            {
                MpData[(int)ClassType.Magician, i] = 3 * MpData[(int)ClassType.Adventurer, i];
            }

            // WRESTLER CLASS
            for (int i = 1; i < MpData.GetLength(1) - 1; i++)
            {
                MpData[(int)ClassType.Wrestler, i] = MpData[(int)ClassType.Adventurer, i];
            }
        }

        private void LoadMpHealth()
        {
            MpHealth = new int[5];
            MpHealth[(int)ClassType.Adventurer] = 10;
            MpHealth[(int)ClassType.Swordman] = 30;
            MpHealth[(int)ClassType.Archer] = 50;
            MpHealth[(int)ClassType.Magician] = 80;
            MpHealth[(int)ClassType.Wrestler] = 30;
        }

        private void LoadMpHealthStand()
        {
            MpHealthStand = new int[5];
            MpHealthStand[(int)ClassType.Adventurer] = 5;
            MpHealthStand[(int)ClassType.Swordman] = 16;
            MpHealthStand[(int)ClassType.Archer] = 28;
            MpHealthStand[(int)ClassType.Magician] = 40;
            MpHealthStand[(int)ClassType.Wrestler] = 16;
        }

        private void LoadSpeedData()
        {
            SpeedData = new byte[5];
            SpeedData[(int)ClassType.Adventurer] = 11;
            SpeedData[(int)ClassType.Swordman] = 11;
            SpeedData[(int)ClassType.Archer] = 12;
            SpeedData[(int)ClassType.Magician] = 10;
            SpeedData[(int)ClassType.Wrestler] = 11;
        }

        private void LoadSpxpData()
        {
            // Load SpData
            SpxpData = new double[256];
            SpxpData[0] = 15000;
            SpxpData[19] = 218000;
            for (int i = 1; i < 19; i++)
            {
                SpxpData[i] = SpxpData[i - 1] + 10000;
            }

            for (int i = 20; i < SpxpData.Length; i++)
            {
                SpxpData[i] = SpxpData[i - 1] + 6 * (3 * i * (i + 1) + 1);
            }
        }

        // TODO: Change or Verify
        private void LoadStats()
        {
            _minHit = new int[5, 256];
            _maxHit = new int[5, 256];
            _hitRate = new int[5, 256];
            _criticalHitRate = new int[5, 256];
            _criticalHit = new int[5, 256];
            _minDist = new int[5, 256];
            _maxDist = new int[5, 256];
            _distRate = new int[5, 256];
            _criticalDistRate = new int[5, 256];
            _criticalDist = new int[5, 256];
            _hitDef = new int[5, 256];
            _hitDodge = new int[5, 256];
            _distDef = new int[5, 256];
            _distDodge = new int[5, 256];
            _magicalDef = new int[5, 256];

            for (int i = 0; i < 256; i++)
            {
                // ADVENTURER
                _minHit[(int)ClassType.Adventurer, i] = i + 9; // approx
                _maxHit[(int)ClassType.Adventurer, i] = i + 9; // approx
                _hitRate[(int)ClassType.Adventurer, i] = i + 9; // approx
                _criticalHitRate[(int)ClassType.Adventurer, i] = 0; // sure
                _criticalHit[(int)ClassType.Adventurer, i] = 0; // sure
                _minDist[(int)ClassType.Adventurer, i] = i + 9; // approx
                _maxDist[(int)ClassType.Adventurer, i] = i + 9; // approx
                _distRate[(int)ClassType.Adventurer, i] = (i + 9) * 2; // approx
                _criticalDistRate[(int)ClassType.Adventurer, i] = 0; // sure
                _criticalDist[(int)ClassType.Adventurer, i] = 0; // sure
                _hitDef[(int)ClassType.Adventurer, i] = i + 9 / 2; // approx
                _hitDodge[(int)ClassType.Adventurer, i] = i + 9; // approx
                _distDef[(int)ClassType.Adventurer, i] = (i + 9) / 2; // approx
                _distDodge[(int)ClassType.Adventurer, i] = i + 9; // approx
                _magicalDef[(int)ClassType.Adventurer, i] = (i + 9) / 2; // approx

                // SWORDMAN
                _criticalHitRate[(int)ClassType.Swordman, i] = 0; // approx
                _criticalHit[(int)ClassType.Swordman, i] = 0; // approx
                _criticalDist[(int)ClassType.Swordman, i] = 0; // approx
                _criticalDistRate[(int)ClassType.Swordman, i] = 0; // approx
                _minDist[(int)ClassType.Swordman, i] = i + 12; // approx
                _maxDist[(int)ClassType.Swordman, i] = i + 12; // approx
                _distRate[(int)ClassType.Swordman, i] = 2 * (i + 12); // approx
                _hitDodge[(int)ClassType.Swordman, i] = i + 12; // approx
                _distDodge[(int)ClassType.Swordman, i] = i + 12; // approx
                _magicalDef[(int)ClassType.Swordman, i] = (i + 9) / 2; // approx
                _hitRate[(int)ClassType.Swordman, i] = i + 27; // approx
                _hitDef[(int)ClassType.Swordman, i] = i + 2; // approx

                _minHit[(int)ClassType.Swordman, i] = 2 * i + 5; // approx Numbers n such that 10n+9 is prime.
                _maxHit[(int)ClassType.Swordman, i] = 2 * i + 5; // approx Numbers n such that 10n+9 is prime.
                _distDef[(int)ClassType.Swordman, i] = i; // approx

                // MAGICIAN
                _hitRate[(int)ClassType.Magician, i] = 0; // sure
                _criticalHitRate[(int)ClassType.Magician, i] = 0; // sure
                _criticalHit[(int)ClassType.Magician, i] = 0; // sure
                _criticalDistRate[(int)ClassType.Magician, i] = 0; // sure
                _criticalDist[(int)ClassType.Magician, i] = 0; // sure

                _minDist[(int)ClassType.Magician, i] = 14 + i; // approx
                _maxDist[(int)ClassType.Magician, i] = 14 + i; // approx
                _distRate[(int)ClassType.Magician, i] = (14 + i) * 2; // approx
                _hitDef[(int)ClassType.Magician, i] = (i + 11) / 2; // approx
                _magicalDef[(int)ClassType.Magician, i] = i + 4; // approx
                _hitDodge[(int)ClassType.Magician, i] = 24 + i; // approx
                _distDodge[(int)ClassType.Magician, i] = 14 + i; // approx

                _minHit[(int)ClassType.Magician, i] =
                    2 * i + 9; // approx Numbers n such that n^2 is of form x^ 2 + 40y ^ 2 with positive x,y.
                _maxHit[(int)ClassType.Magician, i] =
                    2 * i + 9; // approx Numbers n such that n^2 is of form x^2+40y^2 with positive x,y.
                _distDef[(int)ClassType.Magician, i] = 20 + i; // approx

                // ARCHER
                _criticalHitRate[(int)ClassType.Archer, i] = 0; // sure
                _criticalHit[(int)ClassType.Archer, i] = 0; // sure
                _criticalDistRate[(int)ClassType.Archer, i] = 0; // sure
                _criticalDist[(int)ClassType.Archer, i] = 0; // sure

                _minHit[(int)ClassType.Archer, i] = 9 + i * 3; // approx
                _maxHit[(int)ClassType.Archer, i] = 9 + i * 3; // approx
                int add = (i % 2) == 0 ? 2 : 4;
                _hitRate[(int)ClassType.Archer, 1] = 41;
                _hitRate[(int)ClassType.Archer, i] = _hitRate[(int)ClassType.Archer, i] + add; // approx
                _minDist[(int)ClassType.Archer, i] = 2 * i; // approx
                _maxDist[(int)ClassType.Archer, i] = 2 * i; // approx

                _distRate[(int)ClassType.Archer, i] = 20 + 2 * i; // approx
                _hitDef[(int)ClassType.Archer, i] = i; // approx
                _magicalDef[(int)ClassType.Archer, i] = i + 2; // approx
                _hitDodge[(int)ClassType.Archer, i] = 41 + i; // approx
                _distDodge[(int)ClassType.Archer, i] = i + 2; // approx
                _distDef[(int)ClassType.Archer, i] = i; // approx

                // Wrestler
                _criticalHitRate[(int)ClassType.Wrestler, i] = 0; // approx
                _criticalHit[(int)ClassType.Wrestler, i] = 0; // approx
                _criticalDist[(int)ClassType.Wrestler, i] = 0; // approx
                _criticalDistRate[(int)ClassType.Wrestler, i] = 0; // approx
                _minDist[(int)ClassType.Wrestler, i] = i + 12; // approx
                _maxDist[(int)ClassType.Wrestler, i] = i + 12; // approx
                _distRate[(int)ClassType.Wrestler, i] = 2 * (i + 12); // approx
                _hitDodge[(int)ClassType.Wrestler, i] = i + 12; // approx
                _distDodge[(int)ClassType.Wrestler, i] = i + 12; // approx
                _magicalDef[(int)ClassType.Wrestler, i] = (i + 9) / 2; // approx
                _hitRate[(int)ClassType.Wrestler, i] = i + 27; // approx
                _hitDef[(int)ClassType.Wrestler, i] = i + 2; // approx

                _minHit[(int)ClassType.Wrestler, i] = 2 * i + 5; // approx Numbers n such that 10n+9 is prime.
                _maxHit[(int)ClassType.Wrestler, i] = 2 * i + 5; // approx Numbers n such that 10n+9 is prime.
                _distDef[(int)ClassType.Wrestler, i] = i; // approx
            }
        }

        private void LoadXpData()
        {
            // Load XpData
            XpData = new double[256];
            double[] v = new double[256];
            double var = 1;
            v[0] = 540;
            v[1] = 960;
            XpData[0] = 300;
            for (int i = 2; i < v.Length; i++)
            {
                v[i] = v[i - 1] + 420 + 120 * (i - 1);
            }

            for (int i = 1; i < XpData.Length; i++)
            {
                if (i < 79)
                {
                    if (i == 14)
                    {
                        var = 6 / 3d;
                    }
                    else if (i == 39)
                    {
                        var = 19 / 3d;
                    }
                    else if (i == 59)
                    {
                        var = 70 / 3d;
                    }

                    XpData[i] = Convert.ToInt64(XpData[i - 1] + var * v[i - 1]);
                }

                if (i < 79)
                {
                    continue;
                }

                switch (i)
                {
                    case 79:
                        var = 5000;
                        break;
                    case 82:
                        var = 9000;
                        break;
                    case 84:
                        var = 13000;
                        break;
                }

                XpData[i] = Convert.ToInt64(XpData[i - 1] + var * (i + 2) * (i + 2));
            }
        }

        public AttackType GetClassAttackType(ClassType type)
        {
            switch (type)
            {
                case ClassType.Adventurer:
                case ClassType.Swordman:
                case ClassType.Wrestler:
                    return AttackType.Close;

                case ClassType.Archer:
                    return AttackType.Ranged;

                case ClassType.Magician:
                    return AttackType.Magical;
            }

            return AttackType.Close;
        }

        #endregion


        #region Singleton

        private static CharacterHelper _instance;

        public static CharacterHelper Instance => _instance ?? (_instance = new CharacterHelper());

        #endregion
    }
}