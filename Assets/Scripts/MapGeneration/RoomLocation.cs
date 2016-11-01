using UnityEngine;

namespace Assets.Scripts
{
    public struct RoomLocation
    {
        public int X;
        public int Y;

        public RoomLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Print()
        {
            Debug.Log(string.Format("X: {0}, Y: {1}", X, Y));
        }
    }
}
