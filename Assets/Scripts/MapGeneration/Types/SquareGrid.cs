using Assets.Scripts.MapGeneration.Enumerations;
using UnityEngine;

namespace Assets.Scripts.MapGeneration.Types
{
    public class SquareGrid
    {
        public Square[,] Squares;

        public SquareGrid(TileType[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0) + 1;
            int nodeCountY = map.GetLength(1) + 1;
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            Vector3 position;
            int y;
            for (int x = 0; x < nodeCountX; x++)
            {
                for (y = 0; y < nodeCountY; y++)
                {
                    var isWall = x >= map.GetLength(0) || y >= map.GetLength(1) ? true : map[x, y] == TileType.Wall;
                    position = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(isWall, position, squareSize);
                }
            }

            Squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
                for (y = 0; y < nodeCountY - 1; y++)
                {
                    Squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
        }
    }
}