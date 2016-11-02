using Assets.Scripts.MapGeneration.Enumerations;
using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    private TileType[,] _map;
    private System.Random _randomizer;
    private List<RoomLocation> _roomLocations;
    private List<List<Tile>> _roomRegions;

    public int Width = 80;
    public int Height = 60;
    public int BorderWidth = 5;
    public int MinimumRoomDimension = 4;
    public int MaximumRoomDimension = 10;
    public int PathWidth = 2;

    public int RoomCount = 20;

    public string Seed;
    public bool UseRandomSeed;

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) GenerateMap();
    }

    private void GenerateMap()
    {
        _map = new TileType[Width, Height];
        _roomLocations = new List<RoomLocation>();

        if (UseRandomSeed) Seed = Guid.NewGuid().ToString();

        _randomizer = new System.Random(Seed.GetHashCode());

        FillMap();

        ProcessMap();

        var borderedMap = BuildBorderedMap();

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(borderedMap, 1);
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
        for (int i = 0; i < RoomCount; i++)
        {
            int x = _randomizer.Next(MaximumRoomDimension / 2, Width - MaximumRoomDimension / 2);
            int y = _randomizer.Next(MaximumRoomDimension / 2, Height - MaximumRoomDimension / 2);
            _map[x, y] = (int)TileType.Empty;
            _roomLocations.Add(new RoomLocation(x, y));
        }

        foreach (var location in _roomLocations)
        {
            var sizeX = _randomizer.Next(MinimumRoomDimension, MaximumRoomDimension);
            var sizeY = _randomizer.Next(MinimumRoomDimension, MaximumRoomDimension);

            FillRoomSize(location, sizeX, sizeY);
        }
    }

    private void FillRoomSize(RoomLocation location, int sizeX, int sizeY)
    {
        var offsetX = sizeX % 2 == 0 ? 1 : 0;
        var offsetY = sizeY % 2 == 0 ? 1 : 0;
        var startX = location.X - sizeX / 2 + offsetX;
        var endX = location.X + sizeX / 2;
        var startY = location.Y - sizeY / 2 + offsetY;
        var endY = location.Y + sizeY / 2;

        //Debug.Log(string.Format("startX: {0}, endX: {1}, startY: {2}, endY: {3}", startX, endX, startY, endY));
        for (int x = startX; x <= endX; x++)
            for (int y = startY; y <= endY; y++)
            {
                if (!IsInMapBounds(x, y)) continue;
                _map[x, y] = (int)TileType.Empty;
            }
    }

    private TileType[,] BuildBorderedMap()
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

        return borderedMap;
    }

    #endregion Map Building

    #region Map Processing

    private void ProcessMap()
    {
        _roomRegions = GetRegions(TileType.Empty);

        List<Room> rooms = new List<Room>();
        foreach (var roomRegion in _roomRegions)
        {
            rooms.Add(new Room(roomRegion, _map));
        }

        rooms.Sort();
        rooms[0].IsMainRoom = true;
        rooms[0].IsAccessibleFromMainRoom = true;

        ConnectClosetRooms(rooms);
    }

    private List<List<Tile>> GetRegions(TileType tileType)
    {
        List<List<Tile>> regions = new List<List<Tile>>();
        int[,] mapFlags = new int[Width, Height];

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
        int[,] mapFlags = new int[Width, Height];
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

                if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
                {
                    CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                }
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
            //DrawCircle(c, PathWidth);
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
        if (IsInMapBounds(drawX, drawY)) _map[drawX, drawY] = TileType.Empty;
    }

    private void DrawCircle(Tile c, int r)
    {
        for (int x = -r; x <= r; x++)
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.X + x;
                    int drawY = c.Y + y;
                    if (IsInMapBounds(drawX, drawY)) _map[drawX, drawY] = TileType.Empty;
                }
            }
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

        for(int y = 1; y <= dy; y++)
        {
            line.Add(new Tile(to.X, from.Y + y * ySign));
        }

        return line;
    }

    private Vector3 TileToWorldPoint(Tile tile)
    {
        return new Vector3(-Width / 2 + tile.X, 2, -Height / 2 + tile.Y);
    }

    #endregion Map Processing

    #region Utility Functions

    private bool IsInMapBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private void PrintMap(TileType[,] map)
    {
        var output = new System.Text.StringBuilder();
        for (int y = 0; y < map.GetLength(1); y++)
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