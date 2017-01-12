using Assets.Scripts.MapGeneration.Enumerations;
using Assets.Scripts.MapGeneration.Types;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private TileType[,] _map;
    private System.Random _randomizer;
    private List<List<Tile>> _roomRegions;
    private List<Room> _rooms;

    public int Width = 128;
    public int Height = 128;
    public int BorderWidth = 2;
    public int MinimumRoomDimension = 4;
    public int MaximumRoomDimension = 32;
    public int PathWidth = 2;
    public int WallHeight = 4;
    public int SquareSize = 1;
    public int MinRoomCount = 2;
    public int MaxRoomCount = 10;
    public bool RenderPassageways = true;
    public string Seed;
    public bool UseRandomSeed;

    [HideInInspector]
    public bool MapGenerated = false;

    [HideInInspector]
    public float PlayerStartingY;

    public void GenerateMap()
    {
        _map = new TileType[Width, Height];

        if (UseRandomSeed) Seed = Guid.NewGuid().ToString();

        _randomizer = new System.Random(Seed.GetHashCode());

        FillMap();

        ProcessMap();

        PrintMap(_map, "Bordered Map");
        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(_map, _rooms, WallHeight, SquareSize);
        PlayerStartingY = WallHeight * -SquareSize + 1;
        MapGenerated = true;
    }

    #region Map Building

    private void FillMap()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                _map[x, y] = TileType.Wall;
            }

        BuildRooms();
    }

    private void BuildRooms()
    {
        var roomCount = _randomizer.Next(MinRoomCount, MaxRoomCount);
        List<RoomLocation> roomLocations = new List<RoomLocation>();
        for (int i = 0; i < roomCount; i++)
        {
            int x = _randomizer.Next(0, Width - MaximumRoomDimension / 2);
            int y = _randomizer.Next(0, Height - MaximumRoomDimension / 2);
            if (_map[x, y] == TileType.Room)
            {
                i--;
                continue;
            }

            _map[x, y] = TileType.Room;

            var sizeX = _randomizer.Next(MinimumRoomDimension, MaximumRoomDimension);
            var sizeY = _randomizer.Next(MinimumRoomDimension, MaximumRoomDimension);

            roomLocations.Add(new RoomLocation(x, y, sizeX, sizeY));
        }

        PrintMap(_map, "Post Room Location: Room Count = " + roomCount);

        foreach (var location in roomLocations)
        {
            FillRoomSize(location);
        }

        PrintMap(_map, "Post Room Fill");
    }

    private void FillRoomSize(RoomLocation location)
    {
        var offsetX = location.Width % 2 == 0 ? 1 : 0;
        var offsetY = location.Height % 2 == 0 ? 1 : 0;
        var startX = location.X - location.Width / 2 + offsetX;
        var endX = location.X + location.Width / 2;
        var startY = location.Y - location.Height / 2 + offsetY;
        var endY = location.Y + location.Height / 2;

        for (int x = startX; x <= endX; x++)
            for (int y = startY; y <= endY; y++)
            {
                if (!IsInMapBounds(x, y)) continue;
                _map[x, y] = TileType.Room;
            }
    }

    private void BuildBorderedMap()
    {
        TileType[,] borderedMap = new TileType[Width + BorderWidth * 2, Height + BorderWidth * 2];
        for (int x = 0; x < borderedMap.GetLength(0); x++)
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= BorderWidth && x < Width + BorderWidth && y >= BorderWidth && y < Height + BorderWidth)
                {
                    borderedMap[x, y] = _map[x - BorderWidth, y - BorderWidth];
                }
                else
                {
                    borderedMap[x, y] = TileType.Wall;
                }
            }

        _map = borderedMap;
    }

    #endregion Map Building

    #region Map Processing

    private void ProcessMap()
    {
        _roomRegions = GetRegions(TileType.Room);

        _rooms = new List<Room>();
        foreach (var roomRegion in _roomRegions)
        {
            _rooms.Add(new Room(SquareSize, roomRegion, _map));
        }

        _rooms.Sort();
        _rooms[0].IsMainRoom = true;
        _rooms[0].IsAccessibleFromMainRoom = true;

        if (RenderPassageways)
        {
            ConnectClosetRooms(_rooms);
        }

        PrintMap(_map, "After Passageways");

        BuildBorderedMap();

        PrintMap(_map, "After bordering");

        var wallRegions = GetRegions(TileType.Wall);
        foreach (var wallRegion in wallRegions)
        {
            RemoveExtraWallTiles(wallRegion);
        }

        PrintMap(_map, "After wall culling");
    }

    private List<List<Tile>> GetRegions(TileType tileType)
    {
        List<List<Tile>> regions = new List<List<Tile>>();
        int[,] mapFlags = new int[_map.GetLength(0), _map.GetLength(1)];

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (mapFlags[x, y] == 0 && _map[x, y] == tileType)
                {
                    List<Tile> newRoom = GetRegionTiles(x, y);
                    regions.Add(newRoom);
                    foreach (var tile in newRoom)
                    {
                        mapFlags[tile.X, tile.Y] = 1;
                    }
                }
            }

        return regions;
    }

    private List<Tile> GetRegionTiles(int startX, int startY)
    {
        List<Tile> tiles = new List<Tile>();
        int[,] mapFlags = new int[_map.GetLength(0), _map.GetLength(1)];
        TileType tileType = _map[startX, startY];

        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(new Tile(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Tile tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.X - 1; x <= tile.X + 1; x++)
                for (int y = tile.Y - 1; y <= tile.Y + 1; y++)
                {
                    if (IsInMapBounds(x, y) && (y == tile.Y || x == tile.X))
                    {
                        if (mapFlags[x, y] == 0 && _map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Tile(x, y));
                        }
                    }
                }
        }

        return tiles;
    }

    private void ConnectClosetRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (var room in allRooms)
            {
                if (room.IsAccessibleFromMainRoom) roomListB.Add(room);
                else roomListA.Add(room);
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Tile bestTileA = new Tile();
        Tile bestTileB = new Tile();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (var roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.ConnectedRooms.Count > 0) continue;
            }

            foreach (var roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB)) continue;

                if (roomA.IsConnected(roomB))
                {
                    possibleConnectionFound = false;
                    break;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.EdgeTiles.Count; tileIndexA++)
                    for (int tileIndexB = 0; tileIndexB < roomB.EdgeTiles.Count; tileIndexB++)
                    {
                        Tile tileA = roomA.EdgeTiles[tileIndexA];
                        Tile tileB = roomB.EdgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.X - tileB.X, 2) + Mathf.Pow(tileA.Y - tileB.Y, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosetRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosetRooms(allRooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Tile tileA, Tile tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Tile> line = GetPassagePath(tileA, tileB);
        foreach (Tile c in line)
        {
            DrawPath(c, PathWidth);
        }
    }

    private void DrawPath(Tile c, int width)
    {
        var halfWidth = width / 2;
        if (halfWidth > 0)
        {
            for (int x = -halfWidth; x < halfWidth; x++)
            {
                for (int y = -halfWidth; y < halfWidth; y++)
                {
                    DrawPathTile(c, x, y);
                }
            }
        }
        else
        {
            DrawPathTile(c, 0, 0);
        }
    }

    private void DrawPathTile(Tile c, int x, int y)
    {
        int drawX = c.X + x;
        int drawY = c.Y + y;
        if (IsInMapBounds(drawX, drawY)) _map[drawX, drawY] = TileType.Room;
    }

    private List<Tile> GetPassagePath(Tile from, Tile to)
    {
        List<Tile> line = new List<Tile>();

        int dx = Math.Abs(to.X - from.X);
        int dy = Math.Abs(to.Y - from.Y);

        int xSign = Math.Sign(to.X - from.X);
        int ySign = Math.Sign(to.Y - from.Y);

        for (int x = 1; x <= dx; x++)
        {
            line.Add(new Tile(from.X + x * xSign, from.Y));
        }

        for (int y = 1; y <= dy; y++)
        {
            line.Add(new Tile(to.X, from.Y + y * ySign));
        }

        return line;
    }

    private Vector3 TileToWorldPoint(Tile tile)
    {
        return new Vector3(-Width / 2 + tile.X, 2, -Height / 2 + tile.Y);
    }

    public List<Room> GetRooms()
    {
        return _rooms;
    }

    private void RemoveExtraWallTiles(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            MarkExtraWallTile(tile);
        }
    }

    private void MarkExtraWallTile(Tile tile)
    {
        bool isEligible = true;
        for (int x = tile.X - 1; x <= tile.X + 1; x++)
            for (int y = tile.Y - 1; y <= tile.Y + 1; y++)
            {
                if (!IsInMapBounds(x, y)) continue;

                isEligible &= _map[x, y] == TileType.Wall || _map[x, y] == TileType.Nothing;
            }

        if (isEligible)
        {
            _map[tile.X, tile.Y] = TileType.Nothing;
        }
    }

    #endregion Map Processing

    #region Utility Functions

    private bool IsInMapBounds(int x, int y)
    {
        return x >= 0 && x < _map.GetLength(0) && y >= 0 && y < _map.GetLength(1);
    }

    private void PrintMap(TileType[,] map, string message = "")
    {
        var output = new StringBuilder(message + "\n");
        for (int y = map.GetLength(1) - 1; y >= 0; y--)
        {
            var temp = "";
            for (int x = 0; x < map.GetLength(0); x++)
            {
                temp += ((int)map[x, y]).ToString() + " ";
            }
            output.AppendLine(temp);
        }

        Debug.Log(output.ToString());
    }

    #endregion Utility Functions
}