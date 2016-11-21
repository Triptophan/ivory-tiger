using Assets.Scripts.MapGeneration.Enumerations;

namespace Assets.Scripts.MapGeneration.Types
{
    public class Square
    {
        public Node TopLeft;
        public Node TopRight;
        public Node BottomLeft;
        public Node BottomRight;

        public bool TopEdgeActive;
        public bool LeftEdgeActive;
        public bool RightEdgeActive;
        public bool BottomEdgeActive;

        public TileType TileType;

        public Square(TileType tileType, Node topLeft, Node topRight, Node bottomLeft, Node bottomRight)
        {
            TileType = tileType;

            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
        }
    }
}