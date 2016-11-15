using Assets.Scripts.Enemies.AI;
using Assets.Scripts.Enemies.Generation;
using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        private List<Room> _rooms;

        public MapGenerator MapGenerator;
        public EnemySpawner EnemySpawner;
        public GameObject PlayerPrefab;

        public int EnemyScale = 2;

        public bool RenderHallways = true;

        private void Start()
        {
            MapGenerator.GenerateMap(RenderHallways);

            NavMeshBuilder.BuildNavMesh();

            _rooms = MapGenerator.GetRooms();

            SetPlayer();

            SpawnEnemies();
        }

        private void Update()
        {
        }

        private void SetPlayer()
        {
            var mainRoom = _rooms[0];
            var roomPosition = mainRoom.GetRoomCenter(MapGenerator.Width, MapGenerator.Height);
            var playerPosition = new Vector3(roomPosition.x, MapGenerator.PlayerStartingY, roomPosition.z);
            PlayerPrefab.layer = LayerMask.NameToLayer("Players");     
            Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
        }

        private void SpawnEnemies()
        {
            EnemySpawner.GenerateEnemies(_rooms.Count * EnemyScale);

            foreach (var candidate in EnemySpawner.EnemyPool)
            {
                if (candidate.Active) continue;
                var randomRoomIndex = Random.Range(1, _rooms.Count-1);
                var room = _rooms[randomRoomIndex];
               
                var randomTileIndex = Random.Range(0, room.Tiles.Count-1);
                var tile = room.Tiles[randomTileIndex];
                candidate.transform.position = new Vector3(tile.X, MapGenerator.PlayerStartingY, tile.Y);
                
                candidate.Active = true;
                //WanderBehavior wander = candidate.GetComponent<WanderBehavior>();
                //wander.enabled = false;
            }
        }
    }
}