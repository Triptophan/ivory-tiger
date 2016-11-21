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