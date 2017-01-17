using Assets.Scripts.Enemies.Generation;
using Assets.Scripts.MapGeneration.Types;
using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEditor;
using MachineOfStates = Assets.Scripts.StateMachine.StateMachine;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        private List<Room> _rooms;
        private GameObject _playerObject;

        public MachineOfStates StateMachine;
        public MapGenerator MapGenerator;
        public EnemySpawner EnemySpawner;
        public GUIManager GUIManager;
        public Pathfinding PathFinding;
        public GameObject PlayerPrefab;
        public LayerMask WalkableLayer;

        public CombatController PlayerCombatController;

        public int EnemyScale = 2;

        public bool MapDebugMode = false;

        public bool GameReady = false;

        public void RestartNewLevel()
        {
            _rooms = new List<Room>();
            SetupLevel();
        }

        private void Awake()
        {
            GUIManager.LevelManager = this;
            PathFinding = GetComponent<Pathfinding>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (MapDebugMode && Input.GetKeyUp(KeyCode.F12))
            {
                RestartNewLevel();
            }
#endif
        }

        public void SetupLevel()
        {
            GameReady = false;

            while (_rooms == null || _rooms.Count < 2)
            {
                MapGenerator.GenerateMap();

                _rooms = MapGenerator.GetRooms();

                if (MapDebugMode) return;

                NavMeshBuilder.BuildNavMesh();
            }
            SetPlayer();

            EnemySpawner.Spawn(_rooms, MapGenerator.SquareSize, EnemyScale, MapGenerator.PlayerStartingY);

            GameReady = true;
        }

        private void SetPlayer()
        {
            var mainRoom = _rooms[0];
            var randomTileIndex = Random.Range(0, mainRoom.Tiles.Count);
            var roomPosition = mainRoom.Tiles[randomTileIndex];
            var playerPosition = new Vector3(roomPosition.X * MapGenerator.SquareSize, MapGenerator.PlayerStartingY - .5f, roomPosition.Y * MapGenerator.SquareSize);

            if (_playerObject == null)
            {
                PlayerPrefab.layer = LayerMask.NameToLayer("Players");
                _playerObject = (GameObject)Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                PlayerCombatController = _playerObject.GetComponent<CombatController>();
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