using Assets.Scripts.Combat.Projectiles;
using Assets.Scripts.Enemies.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(WanderBehavior))]
    [RequireComponent(typeof(FieldOfView))]
    public class Enemy : MonoBehaviour
    {
        public bool CanAttack = false;
        private WanderBehavior _wander;
        private FieldOfView _fieldOfView;
        private GameObject _gameObject;

        public bool Active
        {
            get { return isActiveAndEnabled; }
            set
            {
                if (_gameObject == null) _gameObject = gameObject;
                _gameObject.SetActive(value);
            }
        }

        public List<Enemy> EnemyPool;
        public List<Enemy> DeadEnemyPool;

        public void Awake()
        {
            _gameObject = gameObject;
            _gameObject.tag = "Enemy";
            _gameObject.layer = LayerMask.NameToLayer("Enemies");
            _wander = GetComponent<WanderBehavior>();
            _fieldOfView = GetComponent<FieldOfView>();
        }

        public void Update()
        {
            //If we're chasing, disable the wander behavior
            _wander.isWandering = !_fieldOfView.isChasing;
            //If we are chasing, we can attack
            CanAttack = _fieldOfView.isChasing;
        }

        public void OnCollisionEnter(Collision collision)
        {
            var collisionObject = collision.gameObject;
            if (collisionObject.tag == "Walls" || collisionObject.tag == "Enemy") return;

            var rockBehavior = collisionObject.GetComponent<Rock>();
            if (rockBehavior != null)
            {
                Destroy(collisionObject);
                _gameObject.SetActive(false);
                DeadEnemyPool.Add(this);
            }
        }
    }
}