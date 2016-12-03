using Assets.Scripts.Enemies.Generation;
using Assets.Scripts.MapGeneration.Types;
using Assets.Scripts.Player;
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
        public GUIManager GUIManager;

        public GameObject PlayerPrefab;

        public int EnemyScale = 2;

        public bool MapDebugMode = false;

        public void RestartNewLevel()
        {
            _rooms = new List<Room>();
            SetupLevel();
        }

        private void Awake()
        {
            GUIManager.LevelManager = this;
        }

        private void Start()
        {
            SetupLevel();
        }

        private void Update()
        {
            if (!MapDebugMode && (CanPlayerProceedLevels() || Input.GetKeyUp(KeyCode.BackQuote)))
            {
                RestartNewLevel();
            }
        }

        private void SetupLevel()
        {
            while (_rooms == null || _rooms.Count < 2)
            {
                MapGenerator.GenerateMap();

                _rooms = MapGenerator.GetRooms();

                if (MapDebugMode) return;

                NavMeshBuilder.BuildNavMesh();
            }
            SetPlayer();

            EnemySpawner.Spawn(_rooms, MapGenerator.SquareSize, EnemyScale, MapGenerator.PlayerStartingY);
        }

        private void SetPlayer()
        {
            var mainRoom = _rooms[0];
            var randomTileIndex = Random.Range(0, mainRoom.Tiles.Count);
            var roomPosition = mainRoom.Tiles[randomTileIndex];
            var playerPosition = new Vector3(roomPosition.X * MapGenerator.SquareSize, MapGenerator.PlayerStartingY, roomPosition.Y * MapGenerator.SquareSize);

            if (_playerObject == null)
            {
                PlayerPrefab.layer = LayerMask.NameToLayer("Players");
                _playerObject = (GameObject)Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                GUIManager.PlayerCombatController = _playerObject.GetComponent<CombatController>();
                return;
            }

            var playerTransform = _playerObject.transform;
            playerTransform.position = playerPosition;
            playerTransform.LookAt(mainRoom.Center);

        }

        private bool CanPlayerProceedLevels()
        {
            return EnemySpawner.DeadEnemyPool.Count == EnemySpawner.EnemyPool.Count;
        }
    }
}