﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;

namespace WingsEmu.Pathfinder.PathFinder
{
    public static class JaggedArrayExtensions
    {
        #region Methods

        public static T[][] CreateJaggedArray<T>(int xLenght, int yLength)
        {
            T[][] array = new T[xLenght][];
            for (int i = 0; i < xLenght; i++)
            {
                array[i] = new T[yLength];
            }

            return array;
        }

        #endregion
    }

    internal class ArrayTraverse
    {
        #region Instantiation

        public ArrayTraverse(Array array)
        {
            _maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; ++i)
            {
                _maxLengths[i] = array.GetLength(i) - 1;
            }

            _position = new int[array.Rank];
        }

        #endregion

        #region Methods

        public bool Step()
        {
            for (int i = 0; i < _position.Length; ++i)
            {
                if (_position[i] < _maxLengths[i])
                {
                    _position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        _position[j] = 0;
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Members

        public readonly int[] _position;

        private readonly int[] _maxLengths;

        #endregion
    }

    public static class BestFirstSearch
    {
        #region Methods

        public static List<Node> Backtrace(Node end)
        {
            List<Node> path = new List<Node>();
            while (end.Parent != null)
            {
                end = end.Parent;
                path.Add(end);
            }

            path.Reverse();
            return path;
        }

        public static List<Node> FindPathJagged(GridPos start, GridPos end, GridPos[][] Grid)
        {
            int GridX = Grid.Length;
            int GridY = Grid[0].Length;
            if (GridX <= start.X || GridY <= start.Y || start.X < 0 || start.Y < 0)
            {
                return new List<Node>();
            }

            var node = new Node();

            Node[][] grid = JaggedArrayExtensions.CreateJaggedArray<Node>(GridX, GridY);
            if (grid[start.X][start.Y] == null)
            {
                grid[start.X][start.Y] = new Node(Grid[start.X][start.Y]);
            }

            Node startingNode = grid[start.X][start.Y];
            var path = new MinHeap();

            // push the start node into the open list
            path.Push(startingNode);
            startingNode.Opened = true;

            // while the open list is not empty
            while (path.Count > 0)
            {
                // pop the position of node which has the minimum `f` value.
                node = path.Pop();
                if (grid[node.X][node.Y] == null)
                {
                    grid[node.X][node.Y] = new Node(Grid[node.X][node.Y]);
                }

                grid[node.X][node.Y].Closed = true;

                //if reached the end position, construct the path and return it
                if (node.X == end.X && node.Y == end.Y)
                {
                    return Backtrace(node);
                }

                // get neigbours of the current node
                List<Node> neighbors = GetNeighborsJagged(grid, node, Grid);
                for (int i = 0, l = neighbors.Count; i < l; ++i)
                {
                    Node neighbor = neighbors[i];

                    if (neighbor.Closed)
                    {
                        continue;
                    }

                    // check if the neighbor has not been inspected yet, or can be reached with
                    // smaller cost from the current node
                    if (!neighbor.Opened)
                    {
                        if (neighbor.F == 0)
                        {
                            neighbor.F = Heuristic.Octile(Math.Abs(neighbor.X - end.X), Math.Abs(neighbor.Y - end.Y));
                        }

                        neighbor.Parent = node;

                        if (!neighbor.Opened)
                        {
                            path.Push(neighbor);
                            neighbor.Opened = true;
                        }
                        else
                        {
                            neighbor.Parent = node;
                        }
                    }
                }
            }

            return new List<Node>();
        }

        public static List<Node> GetNeighborsJagged(Node[][] Grid, Node node, GridPos[][] MapGrid)
        {
            int GridX = Grid.Length;
            int GridY = Grid[0].Length;
            short x = node.X, y = node.Y;
            List<Node> neighbors = new List<Node>();
            bool s0 = false, d0 = false, s1 = false, d1 = false, s2 = false, d2 = false, s3 = false, d3 = false;
            int IndexX;
            int IndexY;

            // ↑
            IndexX = x;
            IndexY = y - 1;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
                s0 = true;
            }

            // →
            IndexX = x + 1;
            IndexY = y;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
                s1 = true;
            }

            // ↓
            IndexX = x;
            IndexY = y + 1;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
                s2 = true;
            }

            // ←
            IndexX = x - 1;
            IndexY = y;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
                s3 = true;
            }

            d0 = s3 || s0;
            d1 = s0 || s1;
            d2 = s1 || s2;
            d3 = s2 || s3;

            // ↖
            IndexX = x - 1;
            IndexY = y - 1;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && d0 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
            }

            // ↗
            IndexX = x + 1;
            IndexY = y - 1;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && d1 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
            }

            // ↘
            IndexX = x + 1;
            IndexY = y + 1;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && d2 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
            }

            // ↙
            IndexX = x - 1;
            IndexY = y + 1;
            if (GridX > IndexX && GridY > IndexY && IndexX >= 0 && IndexY >= 0 && d3 && MapGrid[IndexX][IndexY].IsWalkable())
            {
                if (Grid[IndexX][IndexY] == null)
                {
                    Grid[IndexX][IndexY] = new Node(MapGrid[IndexX][IndexY]);
                }

                neighbors.Add(Grid[IndexX][IndexY]);
            }

            return neighbors;
        }

        public static Node[][] LoadBrushFireJagged(GridPos user, GridPos[][] Grid, short MaxDistance = 22)
        {
            int GridX = Grid.Length;
            int GridY = Grid[0].Length;
            Node[][] grid = JaggedArrayExtensions.CreateJaggedArray<Node>(GridX, GridY);

            var node = new Node();
            if (grid[user.X][user.Y] == null)
            {
                grid[user.X][user.Y] = new Node(Grid[user.X][user.Y]);
            }

            Node Start = grid[user.X][user.Y];
            var path = new MinHeap();

            // push the start node into the open list
            path.Push(Start);
            Start.Opened = true;

            // while the open list is not empty
            while (path.Count > 0)
            {
                // pop the position of node which has the minimum `f` value.
                node = path.Pop();
                if (grid[node.X][node.Y] == null)
                {
                    grid[node.X][node.Y] = new Node(Grid[node.X][node.Y]);
                }

                grid[node.X][node.Y].Closed = true;

                // get neighbors of the current node
                List<Node> neighbors = GetNeighborsJagged(grid, node, Grid);

                for (int i = 0, l = neighbors.Count; i < l; ++i)
                {
                    Node neighbor = neighbors[i];

                    if (neighbor.Closed)
                    {
                        continue;
                    }

                    // check if the neighbor has not been inspected yet, or can be reached with
                    // smaller cost from the current node
                    if (!neighbor.Opened)
                    {
                        if (neighbor.F == 0)
                        {
                            double distance = Heuristic.Octile(Math.Abs(neighbor.X - node.X), Math.Abs(neighbor.Y - node.Y)) + node.F;
                            if (distance > MaxDistance)
                            {
                                neighbor.Value = 1;
                                continue;
                            }

                            neighbor.F = distance;
                            grid[neighbor.X][neighbor.Y].F = neighbor.F;
                        }

                        neighbor.Parent = node;

                        if (!neighbor.Opened)
                        {
                            path.Push(neighbor);
                            neighbor.Opened = true;
                        }
                        else
                        {
                            neighbor.Parent = node;
                        }
                    }
                }
            }

            return grid;
        }

        public static List<Node> TracePathJagged(Node node, Node[][] Grid, GridPos[][] MapGrid)
        {
            List<Node> list = new List<Node>();
            if (MapGrid == null || Grid == null || node.X >= Grid.Length || node.Y >= Grid[0].Length || node.X < 0 || node.Y < 0 || Grid[node.X][node.Y] == null)
            {
                node.F = 100;
                list.Add(node);
                return list;
            }

            Node currentnode = Grid[node.X][node.Y];
            while (currentnode.F != 1 && currentnode.F != 0)
            {
                Node newnode = GetNeighborsJagged(Grid, currentnode, MapGrid)?.OrderBy(s => s.F).FirstOrDefault();
                lock(list)
                {
                    if (newnode == null)
                    {
                        continue;
                    }

                    list.Add(newnode);
                    currentnode = newnode;
                }
            }

            return list;
        }

        #endregion
    }
}