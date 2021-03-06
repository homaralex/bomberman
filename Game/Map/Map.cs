﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Bomberman.Game.Algorithms;
using Bomberman.Game.Movable;
using Bomberman.Game.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.Game.Map
{
    partial class Map : IDrawable, IToInfo
    {
        public const short MAP_WIDTH = 15;
        public const short MAP_HEIGHT = 9;

        private MapElement[,] _mapElements;
        public Tuple<int, int> startPosition { get; private set; }
        public MapElement GetStartSquare() { return _mapElements[startPosition.Item1, startPosition.Item2]; }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var mapObject in _mapElements)
            {
                mapObject.Draw(spriteBatch);
            }
        }
        public void LoadContent(ContentManager content)
        {
            foreach (var mapObject in _mapElements)
            {
                mapObject.LoadContent(content);
            }
        }

        public MapElement GetSquare(int x, int y)
        {
            if (x >= 0 && x <= _mapElements.GetUpperBound(0) && y >= 0 && y <= _mapElements.GetUpperBound(1))
            {
                return _mapElements[x, y];
            }
            else
            {
                return null;
                //throw new BombermanException("Square " + x + ", " + y + "does not exist.");
            }
        }
        public List<MapElement> GetUnoccupiedSquares()
        {
            List<MapElement> freeSquares = new List<MapElement>();

            foreach (var square in _mapElements)
            {
                if (square.IsWalkingTerrain && !square.IsOccupied)
                {
                    freeSquares.Add(square);
                }
            }

            return freeSquares;
        }
        public int DestroySquare(int x, int y)
        {
            int value = 0;
            MapElement square = this.GetSquare(x, y);
            if (square != null)
            {
                if (square.CanBeAffected)
                {
                    value += square.DestroyOccupiables();
                }
                if (square.CanBeDestroyed)
                {
                    value += square.Destroy();
                    this._mapElements[x, y] = new Ground(x, y);
                    if (square.Collectable != null)
                        this._mapElements[x, y].AddCollectable(square.Collectable);
                }
            }
            return value;
        }
        public int DestroySquare(MapElement square)
        {
            return DestroySquare(square.X, square.Y);
        }
        public MapElement OccupySquare(DestroyableElement element)
        {
            var square = GetSquare(element.X, element.Y);
            square.Occupy(element);

            return square;
        }
        public void OccupySquare(int x, int y, DestroyableElement element)
        {
            var square = GetSquare(x, y);
            square.Occupy(element);
        }
        public void LeaveSquare(DestroyableElement element)
        {
            var square = GetSquare(element.X, element.Y);
            square.Leave(element);
        }

        public static double GetSquaresDistance(int x1, int y1, int x2, int y2)
        {
            double distance = 0;
            double xDistance, yDistance;

            xDistance = Math.Abs(x1 - x2);
            yDistance = Math.Abs(y1 - y2);
            distance = Math.Max(xDistance, yDistance);

            return distance;
        }
        public static double GetSquaresDistance(MapElement square1, MapElement square2)
        {
            return GetSquaresDistance(square1.X, square1.Y, square2.X, square2.Y);
        }
        public List<int>[] ToGraph(MovableElement movable)
        {
            int width = MAP_WIDTH;
            int height = MAP_HEIGHT;
            int totalVertices = width * height;

            List<int>[] vertices = new List<int>[totalVertices];
            for (int x = 0; x < width; ++x)
            {
                int rowIndex = x * height;
                for (int y = 0; y < height; ++y)
                {
                    List<int> adjacentVertices = new List<int>();
                    if (IsMoveLegal(movable, x + 1, y))
                    {
                        int verticeIndex = (x + 1) * height + y;
                        adjacentVertices.Add(verticeIndex);
                    }
                    if (IsMoveLegal(movable, x - 1, y))
                    {
                        int verticeIndex = (x - 1) * height + y;
                        adjacentVertices.Add(verticeIndex);
                    }
                    if (IsMoveLegal(movable, x, y + 1))
                    {
                        int verticeIndex = x * height + y + 1;
                        adjacentVertices.Add(verticeIndex);
                    }
                    if (IsMoveLegal(movable, x, y - 1))
                    {
                        int verticeIndex = x * height + y - 1;
                        adjacentVertices.Add(verticeIndex);
                    }

                    vertices[y + rowIndex] = adjacentVertices;
                }
            }
            return vertices;
        }
        private bool IsMoveLegal(MovableElement movable, int x, int y)
        {
            MapElement square = GetSquare(x, y);
            if (square == null)
                return false;
            if (square == Adventurer.GetOccupiedSquare())
                return true;

            if (square.IsOccupied)
                return false;
            if (!square.IsFlyingTerrain)
                return false;
            if (!square.IsWalkingTerrain)
                return movable.CanFly;
            else
                return true;
        }
        public int GetSquareIndex(MapElement square)
        {
            return GetSquareIndex(square.X, square.Y);
        }
        public int GetSquareIndex(int x, int y)
        {
            int index = x * MAP_HEIGHT + y;
            return index;
        }
        public Moves IndexToMove(int index, int x, int y)
        {
            if (index == -1)
            {
                return Moves.None;
            }
            int toX = index / MAP_HEIGHT;
            int toY = index % MAP_HEIGHT;

            if (toX == x + 1)
            {
                return Moves.Right;
            }
            if (toX == x - 1)
            {
                return Moves.Left;
            }
            if (toY == y + 1)
            {
                return Moves.Down;
            }
            if (toY == y - 1)
            {
                return Moves.Up;
            }
            return Moves.None;
        }

        private Map(MapElement[,] elements, int x, int y)
        {
            _mapElements = elements;
            startPosition = new Tuple<int, int>(x, y);
        }

        public System.Xml.Serialization.IXmlSerializable ToInfo()
        {
            int width = _mapElements.GetUpperBound(0) + 1;
            int height = _mapElements.GetUpperBound(1) + 1;
            var squareInfos = new MapElementInfo[width, height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    squareInfos[x, y] = (MapElementInfo)_mapElements[x, y].ToInfo();
                }
            }

            var info = new MapInfo(squareInfos);
            info.StartPosition = this.startPosition;
            info.Width = width;
            info.Height = height;

            return info;
        }

        public void Construct(System.Xml.Serialization.IXmlSerializable info)
        {
            MapInfo mapInfo = (MapInfo)info;
            this.startPosition = mapInfo.StartPosition;
            this._mapElements = new MapElement[mapInfo.Width, mapInfo.Height];

            for (int x = 0; x < mapInfo.Width; ++x)
            {
                for (int y = 0; y < mapInfo.Height; ++y)
                {
                    var squareInfo = mapInfo.Squares[x, y];
                    this._mapElements[x, y] = MapFactory.ConstructSquare(squareInfo);
                }
            }
        }

        public Map(IXmlSerializable info)
        {
            Construct(info);
        }
        private Map() { }
    }
}
