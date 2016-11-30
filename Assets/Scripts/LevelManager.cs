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
        private GameObject _playerObject;

        public MapGenerator MapGenerator;
        public EnemySpawner EnemySpawner;
        public GameObject PlayerPrefab;

        public int EnemyScale = 2;

        public bool MapDebugMode = false;

        private void Start()
        {
            SetupLevel();
        }

        private void Update()
        {
            if (CanPlayerProceedLevels() || Input.GetKeyUp(KeyCode.BackQuote))
            {
                _rooms.Clear();
                SetupLevel();
            }
        }

        private void SetupLevel()
        {
            while (_rooms == null || _rooms.Count < 2)
            {
                MapGenerator.GenerateMap();

                if (MapDebugMode) return;

                NavMeshBuilder.BuildNavMesh();

                _rooms = MapGenerator.GetRooms();
            }
            SetPlayer();

            EnemySpawner.Spawn(_rooms, MapGenerator.SquareSize, EnemyScale, MapGenerator.PlayerStartingY);
        }

        private void SetPlayer()
        {
            var roomPosition = _rooms[0].Center;
            var playerPosition = new Vector3(roomPosition.x * MapGenerator.SquareSize, MapGenerator.PlayerStartingY, roomPosition.y * MapGenerator.SquareSize);

            if (_playerObject == null)
            {
                PlayerPrefab.layer = LayerMask.NameToLayer("Players");
                _playerObject = (GameObject)Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                return;
            }

            _playerObject.transform.position = playerPosition;
        }

        private bool CanPlayerProceedLevels()
        {
            return EnemySpawner.DeadEnemyPool.Count == EnemySpawner.EnemyPool.Count;
        }
    }
}