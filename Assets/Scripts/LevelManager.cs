using UnityEngine;
using UnityEditor;
using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using Assets.Scripts.Enemies.Generation;
using Assets.Scripts.Enemies.AI;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        private List<Room> _rooms;
        private GameObject _playerGO;
        private Transform _playerTransform;

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
            //foreach (var candidate in EnemySpawner.EnemyPool)
            //{
            //    var navAgent = candidate.GetComponent<NavMeshAgent>();
            //    float distance = Vector3.Distance(candidate.transform.position, _playerTransform.position);

            //    navAgent.SetDestination(_playerTransform.position);
            //}
        }

        private void SetPlayer()
        {
            var mainRoom = _rooms[0];
            var roomPosition = mainRoom.GetRoomCenter(MapGenerator.Width, MapGenerator.Height);
            var playerPosition = new Vector3(roomPosition.x, MapGenerator.PlayerStartingY, roomPosition.z);
            Debug.Log("Player position: " + playerPosition);
            _playerGO = (GameObject)Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
            _playerTransform = _playerGO.transform;
        }

        private void SpawnEnemies()
        {
            Debug.Log(string.Format("Room count: {0}, EnemyScale: {1}, Total Enemies:{2}", _rooms.Count, EnemyScale, _rooms.Count * EnemyScale));
            EnemySpawner.GenerateEnemies(_rooms.Count * EnemyScale);
            
            foreach(var candidate in EnemySpawner.EnemyPool)
            {
                if (candidate.Active) continue;
                var randomRoomIndex = Random.Range(1, _rooms.Count);
                var room = _rooms[randomRoomIndex];
                var randomTileIndex = Random.Range(0, room.Tiles.Count);
                var tile = room.Tiles[randomTileIndex];
                candidate.transform.position = new Vector3(tile.X, MapGenerator.PlayerStartingY, tile.Y);
                candidate.Active = true;
                WanderBehavior wander = candidate.GetComponent<WanderBehavior>();
                wander.enabled = false;                
            }
        }
    }
}
