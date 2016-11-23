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

        private void Start()
        {
            MapGenerator.GenerateMap();

            //NavMeshBuilder.BuildNavMesh();

            //_rooms = MapGenerator.GetRooms();

            //SetPlayer();

            //EnemySpawner.Spawn(_rooms, MapGenerator.SquareSize, EnemyScale, MapGenerator.PlayerStartingY);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.BackQuote))
            {
                MapGenerator.GenerateMap();
            }
        }

        private void SetPlayer()
        {
            var mainRoom = _rooms[0];
            var roomPosition = mainRoom.Center;
            var playerPosition = new Vector3(roomPosition.x * MapGenerator.SquareSize, MapGenerator.PlayerStartingY, roomPosition.y * MapGenerator.SquareSize);
            PlayerPrefab.layer = LayerMask.NameToLayer("Players");
            Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
        }
    }
}