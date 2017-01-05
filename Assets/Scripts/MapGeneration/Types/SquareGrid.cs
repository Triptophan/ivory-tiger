using Assets.Scripts.MapGeneration.Enumerations;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.MapGeneration.Types
{
    public class SquareGrid
    {
        public Square[,] Squares;

        public SquareGrid(TileType[,] map, List<Room> rooms, float squareSize)
        {
            int nodeCountX = map.GetLength(0) + 1;
            int nodeCountY = map.GetLength(1) + 1;
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            Node[,] nodes = new Node[nodeCountX, nodeCountY];

            GenerateNodes(map, squareSize, mapWidth, mapHeight, nodes);

            GenerateSquares(map, nodeCountX, nodeCountY, nodes, rooms);

            MarkEdges();
        }

        private static void GenerateNodes(TileType[,] map, float squareSize, float mapWidth, float mapHeight, Node[,] nodes)
        {
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int nx = x; nx <= x + 1; nx++)
                        for (int ny = y; ny <= y + 1; ny++)
                        {
                            if (map[x, y] == TileType.Nothing) continue;
                            Vector3 position = new Vector3(-mapWidth / 2 + nx * squareSize + squareSize / 2, 0, -mapHeight / 2 + ny * squareSize + squareSize / 2);
                            nodes[nx, ny] = new Node(position);
                        }
                }
        }

        private void GenerateSquares(TileType[,] map, int nodeCountX, int nodeCountY, Node[,] nodes, List<Room> rooms)
        {
            Squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    var topLeft = nodes[x, y + 1];
                    var topRight = nodes[x + 1, y + 1];
                    var bottomLeft = nodes[x, y];
                    var bottomRight = nodes[x + 1, y];

                    if (IsANotSquare(topLeft, topRight, bottomLeft, bottomRight)) continue;

                    Squares[x, y] = new Square(map[x, y], topLeft, topRight, bottomLeft, bottomRight);
                }

            var roomIndex = 0;
            foreach(var room in rooms)
            {
                foreach(var tile in room.Tiles)
                {
                    Squares[tile.X+1, tile.Y+1].RoomIndex = roomIndex;
                }
                roomIndex++;
            }
        }

        private bool IsANotSquare(Node topLeft, Node topRight, Node bottomLeft, Node bottomRight)
        {
            return topLeft == null || topRight == null || bottomLeft == null || bottomRight == null;
        }

        private void MarkEdges()
        {
            for (int x = 0; x < Squares.GetLength(0); x++)
                for (int y = 0; y < Squares.GetLength(1); y++)
                {
                    var square = Squares[x, y];

                    if (IsTileEdge(square)) continue;

                    square.LeftEdgeActive = x == 0 || IsTileEdge(Squares[x - 1, y]);
                    square.RightEdgeActive = x == Squares.GetLength(0) - 1 || IsTileEdge(Squares[x + 1, y]);
                    square.BottomEdgeActive = y == 0 || IsTileEdge(Squares[x, y - 1]);
                    square.TopEdgeActive = y == Squares.GetLength(1) - 1 || IsTileEdge(Squares[x, y + 1]);
                }
        }

        private bool IsTileEdge(Square square)
        {
            return square == null || square.TileType != TileType.Wall;
        }
    }
}