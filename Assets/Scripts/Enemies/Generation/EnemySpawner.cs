using Assets.Scripts;
using Assets.Scripts.Enemies;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyFactory _enemyFactory;
    private Transform _transform;

        public List<Enemy> EnemyPool;
        public List<Enemy> DeadEnemyPool;
        public LevelManager LevelManager;

    public void Awake()
    {
        _transform = transform;
        _enemyFactory = GetComponent<EnemyFactory>();
        EnemyPool = new List<Enemy>();
        DeadEnemyPool = new List<Enemy>();
    }

    public void Spawn(List<Room> rooms, int roomScale, int enemyScale, float playerStartingY)
    {
        ResetPools();
        GenerateEnemies(rooms.Count * enemyScale);

        foreach (var candidate in EnemyPool)
        {
            var randomRoomIndex = Random.Range(1, rooms.Count - 1);
            var room = rooms[randomRoomIndex];

            var randomTileIndex = Random.Range(0, room.Tiles.Count - 1);
            var randomTile = room.Tiles[randomTileIndex];
            candidate.transform.position = new Vector3(randomTile.X * roomScale, playerStartingY, randomTile.Y * roomScale);

            candidate.Active = true;
        }
    }

    private void GenerateEnemies(int enemyPoolCapacity)
    {
        int enemiesToAddToPool = enemyPoolCapacity - EnemyPool.Count;

            for (int i = 0; i < enemiesToAddToPool; i++)
            {
                var enemy = _enemyFactory.Spawn();
                enemy.transform.parent = _transform;
                enemy.EnemyPool = EnemyPool;
                enemy.DeadEnemyPool = DeadEnemyPool;
                enemy.LevelManager = LevelManager;
                EnemyPool.Add(enemy);
            }
        }

    private void ResetPools()
    {
        foreach(var enemy in EnemyPool)
        {
            enemy.Active = false;
        }

        DeadEnemyPool.Clear();
    }
}