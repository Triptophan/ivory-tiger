using Assets.Scripts.MapGeneration.Types;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies.Generation
{
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
            //GenerateEnemies(rooms.Count * enemyScale);
            GenerateEnemies(1);

            foreach (var candidate in EnemyPool)
            {
                var randomRoomIndex = Random.Range(1, rooms.Count - 1);
                var room = rooms[randomRoomIndex];

                var randomTileX = (int)Random.Range(room.MinimumBounds.x + 1, room.MaximumBounds.x - 1);
                var randomTileY = (int)Random.Range(room.MinimumBounds.y + 1, room.MaximumBounds.y - 1);
                candidate.transform.position = new Vector3(randomTileX * roomScale, playerStartingY, randomTileY * roomScale);

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
}