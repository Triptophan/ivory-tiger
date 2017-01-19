using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<Room> _rooms;
    private GameObject _playerObject;

    public StateMachine StateMachine;
    public MapGenerator MapGenerator;
    public EnemySpawner EnemySpawner;
    public GUIManager GUIManager;

    public GameObject PlayerPrefab;

    public GameObject LevelExit;

    public CombatController PlayerCombatController;

    public int EnemyScale = 2;

    public bool MapDebugMode = false;

    [HideInInspector]
    public bool GameReady = false;

    public void RestartNewLevel()
    {
        _rooms = new List<Room>();
        SetupLevel();
    }

    private void Awake()
    {
        GUIManager.LevelManager = this;
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
        }

        if (MapDebugMode) return;

        NavMeshBuilder.BuildNavMesh();
        
        SetPlayer();

        EnemySpawner.Spawn(_rooms, MapGenerator.SquareSize, EnemyScale, MapGenerator.PlayerStartingY);

        GameReady = true;

        PlaceLevelExit();
    }

    private void SetPlayer()
    {
        var mainRoom = _rooms[0];
        var randomTileIndex = Random.Range(0, mainRoom.Tiles.Count);
        var roomPosition = mainRoom.Tiles[randomTileIndex];
        var playerPosition = new Vector3(Scale(roomPosition.X) + Scale(.5f),
                                            MapGenerator.PlayerStartingY - .5f,
                                            Scale(roomPosition.Y) + Scale(.5f));

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

    private void PlaceLevelExit()
    {
        var roomIndex = Random.Range(1, _rooms.Count - 1);

        var room = _rooms[roomIndex];

        var tileIndex = Random.Range(0, room.Tiles.Count);

        var tile = room.Tiles[tileIndex];

        LevelExit.transform.position = new Vector3(Scale(tile.X) + Scale(.5f), 
                                                    LevelExit.transform.position.y,
                                                    Scale(tile.Y) + Scale(.5f));
    }

    private float Scale(float value)
    {
        return MapGenerator.SquareSize * value;
    }

    private float Scale(int value)
    {
        return MapGenerator.SquareSize * value;
    }
}