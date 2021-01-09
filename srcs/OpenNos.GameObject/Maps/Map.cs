﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.DAL;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Networking;
using WingsEmu.DTOs;
using WingsEmu.DTOs.Interfaces;
using WingsEmu.Packets.Enums;
using WingsEmu.Pathfinder.PathFinder;

namespace OpenNos.GameObject.Maps
{
    public class Map : IMapDTO
    {
        #region Members

        private readonly Random _random;

        #endregion

        #region Instantiation

        public Map(short mapId, byte[] data, IEnumerable<MapTypeDTO> mapTypesMa, IEnumerable<RespawnMapTypeDTO> defaultRespawn)
        {
            _random = new Random();
            MapId = mapId;
            Data = data;
            LoadZone();
            MapTypes = new List<MapTypeDTO>(mapTypesMa);
            if (MapTypes.Count == 0)
            {
                return;
            }

            if (MapTypes[0].RespawnMapTypeId == null)
            {
                return;
            }

            long? respawnMapTypeId = MapTypes[0].RespawnMapTypeId;
            long? returnMapTypeId = MapTypes[0].ReturnMapTypeId;
            if (respawnMapTypeId != null)
            {
                DefaultRespawn = defaultRespawn.FirstOrDefault(s => s.RespawnMapTypeId == respawnMapTypeId);
            }

            if (returnMapTypeId != null)
            {
                DefaultReturn = defaultRespawn.FirstOrDefault(s => s.RespawnMapTypeId == returnMapTypeId);
            }
        }

        #endregion

        #region Properties

        public byte[] Data { get; set; }

        public RespawnMapTypeDTO DefaultRespawn { get; }

        public RespawnMapTypeDTO DefaultReturn { get; }

        public GridPos[][] Grid { get; private set; }

        private ConcurrentBag<MapCell> Cells { get; set; }

        public short MapId { get; set; }

        public List<MapTypeDTO> MapTypes { get; }

        public int Music { get; set; }

        /// <summary>
        ///     This list ONLY for READ access to MapMonster, you CANNOT MODIFY them here. Use
        ///     Add/RemoveMonster instead.
        /// </summary>
        public string Name { get; set; }

        public bool ShopAllowed { get; set; }

        private int XLength { get; set; }

        private int YLength { get; set; }

        #endregion

        #region Methods

        public static int GetDistance(Character.Character character1, Character.Character character2) => GetDistance(new MapCell { X = character1.PositionX, Y = character1.PositionY },
            new MapCell { X = character2.PositionX, Y = character2.PositionY });

        public static int GetDistance(MapCell p, MapCell q) => (int)Heuristic.Octile(Math.Abs(p.X - q.X), Math.Abs(p.Y - q.Y));

        public IEnumerable<ToSummon> GenerateSummons(short vnum, short amount, bool move,
            ConcurrentBag<EventContainer> deathEvents, bool isBonusOrProtected = false, bool isHostile = true,
            bool isBossOrMate = false)
        {
            ConcurrentBag<ToSummon> summonParameters = new ConcurrentBag<ToSummon>();
            for (int i = 0; i < amount; i++)
            {
                MapCell cell = GetRandomPosition();
                summonParameters.Add(new ToSummon(vnum, cell, null, move, isBonusOrProtected: isBonusOrProtected,
                    isHostile: isHostile, isBossOrMate: isBossOrMate) { DeathEvents = deathEvents });
            }

            return summonParameters;
        }

        public bool GetDefinedPosition(int x, int y)
        {
            var cell = new MapCell { X = (short)x, Y = (short)y };
            if (IsBlockedZone(x, y))
            {
                return false;
            }

            return true;
        }

        public MapCell GetLastGoodPosition(short x, short y, MoveType type, ClientSession session)
        {
            var cell = new MapCell { X = x, Y = y };

            switch (type)
            {
                case MoveType.Right:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.X -= 1;
                    }

                    return cell;
                case MoveType.Left:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.X += 1;
                    }

                    return cell;
                case MoveType.Up:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.Y += 1;
                    }

                    return cell;
                case MoveType.Down:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.Y -= 1;
                    }

                    return cell;
                case MoveType.DiagUpRight:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.Y += 1;
                        cell.X -= 1;
                    }

                    return cell;
                case MoveType.DiagUpLeft:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.Y += 1;
                        cell.X += 1;
                    }

                    return cell;
                case MoveType.DiagDownLeft:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.Y -= 1;
                        cell.X += 1;
                    }

                    return cell;
                case MoveType.DiagDownRight:
                    while (!GetDefinedPosition(cell.X, cell.Y))
                    {
                        cell.Y -= 1;
                        cell.X -= 1;
                    }

                    return cell;
            }

            return cell;
        }

        public MapCell GetRandomPosition()
        {
            if (Cells != null)
            {
                return Cells.OrderBy(s => _random.Next(int.MaxValue)).FirstOrDefault();
            }

            Cells = new ConcurrentBag<MapCell>();
            Parallel.For(0, YLength, y => Parallel.For(0, XLength, x =>
            {
                if (!IsBlockedZone(x, y))
                {
                    Cells.Add(new MapCell { X = (short)x, Y = (short)y });
                }
            }));
            return Cells.OrderBy(s => _random.Next(int.MaxValue)).FirstOrDefault();
        }

        public MapCell GetRandomPositionInRadius(byte radius, short xOrigin, short yOrigin)
        {
            ConcurrentBag<MapCell> mapCells = new ConcurrentBag<MapCell>();
            Parallel.For(yOrigin - radius, yOrigin + radius, y => Parallel.For(xOrigin - radius, xOrigin + radius, x =>
            {
                if (!IsBlockedZone(x, y))
                {
                    mapCells.Add(new MapCell { X = (short)x, Y = (short)y });
                }
            }));
            return mapCells.OrderBy(s => _random.Next(int.MaxValue)).FirstOrDefault();
        }

        public bool IsBlockedZone(int x, int y)
        {
            try
            {
                if (Grid == null)
                {
                    return false;
                }

                return !Grid[x][y].IsWalkable();
            }
            catch
            {
                return true;
            }
        }

        internal bool GetFreePosition(ref short firstX, ref short firstY, byte xpoint, byte ypoint)
        {
            short minX = (short)(-xpoint + firstX);
            short maxX = (short)(xpoint + firstX);

            short minY = (short)(-ypoint + firstY);
            short maxY = (short)(ypoint + firstY);

            List<MapCell> cells = new List<MapCell>();
            for (short y = minY; y <= maxY; y++)
            {
                for (short x = minX; x <= maxX; x++)
                {
                    if (x != firstX || y != firstY)
                    {
                        cells.Add(new MapCell { X = x, Y = y });
                    }
                }
            }

            foreach (MapCell cell in cells.OrderBy(s => _random.Next(int.MaxValue)))
            {
                if (IsBlockedZone(firstX, firstY, cell.X, cell.Y))
                {
                    continue;
                }

                firstX = cell.X;
                firstY = cell.Y;
                return true;
            }

            return false;
        }

        private bool IsBlockedZone(int firstX, int firstY, int mapX, int mapY)
        {
            for (int i = 1; i <= Math.Abs(mapX - firstX); i++)
            {
                if (IsBlockedZone(firstX + Math.Sign(mapX - firstX) * i, firstY))
                {
                    return true;
                }
            }

            for (int i = 1; i <= Math.Abs(mapY - firstY); i++)
            {
                if (IsBlockedZone(firstX, firstY + Math.Sign(mapY - firstY) * i))
                {
                    return true;
                }
            }

            return false;
        }

        private void LoadZone()
        {
            // TODO: Optimize
            using (Stream stream = new MemoryStream(Data))
            {
                const int numBytesToRead = 1;
                const int numBytesRead = 0;
                byte[] bytes = new byte[numBytesToRead];

                byte[] xlength = new byte[2];
                byte[] ylength = new byte[2];
                stream.Read(bytes, numBytesRead, numBytesToRead);
                xlength[0] = bytes[0];
                stream.Read(bytes, numBytesRead, numBytesToRead);
                xlength[1] = bytes[0];
                stream.Read(bytes, numBytesRead, numBytesToRead);
                ylength[0] = bytes[0];
                stream.Read(bytes, numBytesRead, numBytesToRead);
                ylength[1] = bytes[0];
                YLength = BitConverter.ToInt16(ylength, 0);
                XLength = BitConverter.ToInt16(xlength, 0);

                Grid = JaggedArrayExtensions.CreateJaggedArray<GridPos>(XLength, YLength);
                for (short i = 0; i < YLength; ++i)
                {
                    for (short t = 0; t < XLength; ++t)
                    {
                        stream.Read(bytes, numBytesRead, numBytesToRead);
                        Grid[t][i] = new GridPos
                        {
                            Value = bytes[0],
                            X = t,
                            Y = i
                        };
                    }
                }
            }
        }

        #endregion
    }
}