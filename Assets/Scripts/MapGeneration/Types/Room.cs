﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[DebuggerDisplay("Room Size: {RoomSize}, Width: {Width}, Height: {Height}, Center: {Center}")]
public class Room : IComparable<Room>
{
    public List<Tile> Tiles;
    public List<Tile> EdgeTiles;

    public List<Room> ConnectedRooms;

    public int RoomSize;
    public int Width;
    public int Height;
    public int TileSize;

    public Vector2 Center;
    public Vector2 MinimumBounds;
    public Vector2 MaximumBounds;

    public bool IsAccessibleFromMainRoom;
    public bool IsMainRoom;

    public Room()
    {
    }

    public Room(int tileSize, List<Tile> roomTiles, TileType[,] map)
    {
        TileSize = tileSize;
        Tiles = roomTiles;
        RoomSize = Tiles.Count;
        ConnectedRooms = new List<Room>();
        EdgeTiles = new List<Tile>();

        ParseDimensionInfo();

        foreach (var tile in Tiles)
        {
            MapEdgeTiles(tile, map);
        }
    }

    public void SetAccessibleFromMainRoom()
    {
        if (!IsAccessibleFromMainRoom)
        {
            IsAccessibleFromMainRoom = true;
            foreach (Room connectedRoom in ConnectedRooms)
            {
                connectedRoom.SetAccessibleFromMainRoom();
            }
        }
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA.IsAccessibleFromMainRoom) roomB.SetAccessibleFromMainRoom();
        else if (roomB.IsAccessibleFromMainRoom) roomA.SetAccessibleFromMainRoom();

        roomA.ConnectedRooms.Add(roomB);
        roomB.ConnectedRooms.Add(roomA);
    }

    public bool IsConnected(Room otherRoom)
    {
        return ConnectedRooms.Contains(otherRoom);
    }

    public int CompareTo(Room otherRoom)
    {
        return otherRoom.RoomSize.CompareTo(RoomSize);
    }

    private void ParseDimensionInfo()
    {
        var minX = Tiles.Min(t => t.X);
        var maxX = Tiles.Max(t => t.X);
        var minY = Tiles.Min(t => t.Y);
        var maxY = Tiles.Max(t => t.Y);

        Width = maxX - minX;
        Height = maxY - minY;

        Center = new Vector2(minX + Width / 2, minY + Height / 2);
        MinimumBounds = new Vector2(minX, minY);
        MaximumBounds = new Vector2(maxX, maxY);
    }

    private void MapEdgeTiles(Tile tile, TileType[,] map)
    {
        for (int x = tile.X - 1; x <= tile.X + 1; x++)
            for (int y = tile.Y - 1; y <= tile.Y + 1; y++)
            {
                if (IsNonCheckedNonCurrentInMapTile(map, tile, x, y)) continue;

                if (IsEligibleTile(map, tile, x, y))
                {
                    EdgeTiles.Add(tile);
                }
            }
    }

    private bool IsNonCheckedNonCurrentInMapTile(TileType[,] map, Tile tile, int x, int y)
    {
        return EdgeTiles.Contains(tile) || (x == tile.X && y == tile.Y) || !IsInMap(map, x, y);
    }

    private bool IsEligibleTile(TileType[,] map, Tile tile, int x, int y)
    {
        return (x == tile.X || y == tile.Y) && map[x, y] == TileType.Wall;
    }

    private bool IsInMap(TileType[,] map, int x, int y)
    {
        return x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1);
    }
}