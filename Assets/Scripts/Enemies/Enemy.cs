using Assets.Scripts.Combat.Projectiles;
using Assets.Scripts.Enemies.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(WanderBehavior))]
    [RequireComponent(typeof(FieldOfView))]
    [RequireComponent(typeof(ChaseTarget))]
    public class Enemy : MonoBehaviour
    {
        public bool CanAttack = false;
        private WanderBehavior _wander;
        private FieldOfView _fieldOfView;
        private GameObject _gameObject;
        private ChaseTarget _chaseTarget;

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
            _chaseTarget = GetComponent<ChaseTarget>();
        }

        public void Update()
        {
            if(_chaseTarget.isChasing)
            {
                _fieldOfView.TargetFound = false;
                _wander.isWandering = false;
                CanAttack = true;
            }
            else if (_fieldOfView.TargetFound)
            {
                _fieldOfView.StopFOV();
                _chaseTarget.Chase(_fieldOfView.targetPosition);
                CanAttack = true;
            }
            else
            {
                _wander.isWandering = true;
                _fieldOfView.StartFOV();
                CanAttack = false;
            }
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