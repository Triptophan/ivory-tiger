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

        public void Awake()
        {
            _transform = transform;
            _enemyFactory = GetComponent<EnemyFactory>();
            EnemyPool = new List<Enemy>();
            DeadEnemyPool = new List<Enemy>();
        }

        public void Spawn(List<Room> rooms, int roomScale, int enemyScale, float playerStartingY)
        {
            ResetDeadEnemies();
            GenerateEnemies(rooms.Count * enemyScale);

            foreach (var candidate in EnemyPool)
            {
                if (candidate.Active) continue;
                var randomRoomIndex = Random.Range(1, rooms.Count - 1);
                var room = rooms[randomRoomIndex];

                var randomTileIndex = Random.Range(0, room.Tiles.Count - 1);
                var tile = room.Tiles[randomTileIndex];
                candidate.transform.position = new Vector3(tile.X * roomScale, playerStartingY, tile.Y * roomScale);

                candidate.Active = true;
            }
        }

        private void GenerateEnemies(int enemyPoolCapacity)
        {
            int enemiesToAddToPool = enemiesToAddToPool = enemyPoolCapacity - EnemyPool.Count;

            for (int i = 0; i < enemiesToAddToPool; i++)
            {
                var enemy = _enemyFactory.Spawn();
                enemy.transform.parent = _transform;
                enemy.EnemyPool = EnemyPool;
                enemy.DeadEnemyPool = DeadEnemyPool;

                EnemyPool.Add(enemy);
            }
        }

        private void ResetDeadEnemies()
        {
            DeadEnemyPool.Clear();
        }
    }
}