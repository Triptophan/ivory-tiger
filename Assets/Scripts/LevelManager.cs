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
        [HideInInspector]
        public GameObject PlayerObject;
        private CombatController _playerCombatController;

        public MachineOfStates StateMachine;
        public MapGenerator MapGenerator;
        public EnemySpawner EnemySpawner;
        public GUIManager GUIManager;
        public Pathfinding PathFinding;
        public GameObject PlayerPrefab;
        public LayerMask WalkableLayer;

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
            PathFinding = GetComponent<Pathfinding>();
        }

        private void Start()
        {
            //SetupLevel();
        }

        private void Update()
        {
            if (!MapDebugMode && (CanPlayerProceedLevels() || Input.GetKeyUp(KeyCode.F12)))
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
            var playerPosition = new Vector3(roomPosition.X * MapGenerator.SquareSize, MapGenerator.PlayerStartingY - .5f, roomPosition.Y * MapGenerator.SquareSize);

            if (PlayerObject == null)
            {
                PlayerPrefab.layer = LayerMask.NameToLayer("Players");
                PlayerObject = (GameObject)Instantiate(PlayerPrefab, playerPosition, Quaternion.identity);
                GUIManager.PlayerCombatController = PlayerObject.GetComponent<CombatController>();
                return;
            }

            var playerTransform = PlayerObject.transform;
            playerTransform.position = playerPosition;
            playerTransform.LookAt(mainRoom.Center);

        }

        private bool CanPlayerProceedLevels()
        {
            return EnemySpawner.DeadEnemyPool.Count == EnemySpawner.EnemyPool.Count;
        }
    }
}