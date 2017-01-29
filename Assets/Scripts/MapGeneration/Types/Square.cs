using System.Diagnostics;
using System;
using Assets.Scripts.MapGeneration.Types;

[DebuggerDisplay("Type: {TileTypeName}, RoomIndex: {RoomIndex}")]
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

    public int RoomIndex = -1;

    protected string TileTypeName { get { return Enum.GetName(typeof(TileType), TileType); } }

    public Square(TileType tileType, Node topLeft, Node topRight, Node bottomLeft, Node bottomRight)
    {
        TileType = tileType;

        TopLeft = topLeft;
        TopRight = topRight;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
    }
}