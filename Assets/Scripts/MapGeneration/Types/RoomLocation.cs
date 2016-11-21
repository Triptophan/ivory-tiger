using System.Diagnostics;

namespace Assets.Scripts.MapGeneration.Types
{
    [DebuggerDisplay("Location: ({X}, {Y}), Size: ({Width}, {Height})")]
    public struct RoomLocation
    {
        public int X;
        public int Y;

        public int Width;
        public int Height;

        public RoomLocation(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}