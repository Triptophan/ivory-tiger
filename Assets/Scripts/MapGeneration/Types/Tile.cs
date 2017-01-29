using System.Diagnostics;

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