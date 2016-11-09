using Assets.Scripts.MapGeneration.Enumerations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGeneration.Types
{
    public class Room : IComparable<Room>
    {
        public List<Tile> Tiles;
        public List<Tile> EdgeTiles;
        public List<Room> ConnectedRooms;
        public int RoomSize;
        public bool IsAccessibleFromMainRoom;
        public bool IsMainRoom;

        public Room()
        {
        }

        public Room(List<Tile> roomTiles, TileType[,] map)
        {
            Tiles = roomTiles;
            RoomSize = Tiles.Count;
            ConnectedRooms = new List<Room>();
            EdgeTiles = new List<Tile>();

            foreach (var tile in Tiles)
            {
                MapEdgeTiles(tile, map);
            }
        }

        public Vector3 GetRoomCenter(int MapWidth, int MapHeight)
        {
            var centerTile = Tiles[Tiles.Count / 2];
            //return new Vector3(centerTile.X - MapWidth/2, 0, centerTile.Y - MapHeight/2);
            return new Vector3(centerTile.X, 0, centerTile.Y);
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

        private void MapEdgeTiles(Tile tile, TileType[,] map)
        {
            for (int x = tile.X - 1; x <= tile.X; x++)
                for (int y = tile.Y - 1; y <= tile.Y; y++)
                {
                    if (!IsInMap(map, x, y)) continue;

                    if (x == tile.X || y == tile.Y)
                    {
                        if (map[x, y] == TileType.Wall) EdgeTiles.Add(tile);
                    }
                }
        }

        private bool IsInMap(TileType[,] map, int x, int y)
        {
            return x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1);
        }
    }
}