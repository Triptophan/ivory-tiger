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

        public void Awake()
        {
            _transform = transform;
            _enemyFactory = GetComponent<EnemyFactory>();
        }

        public void Spawn(List<Room> rooms, int roomScale, int enemyScale, float playerStartingY)
        {
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

        public void GenerateEnemies(int enemyPoolCapacity)
        {
            for (int i = 0; i < enemyPoolCapacity; i++)
            {
                var enemy = _enemyFactory.Spawn();
                enemy.transform.parent = _transform;
                enemy.Active = false;

                EnemyPool.Add(enemy);
            }
        }
    }
}