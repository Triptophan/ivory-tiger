using System.Diagnostics;

namespace Assets.Scripts.MapGeneration.Types
{
    [DebuggerDisplay("({X},{Y})")]
    public struct Tile
    {
        public int X;
        public int Y;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}