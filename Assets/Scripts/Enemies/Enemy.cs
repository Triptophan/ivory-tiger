using Assets.Scripts.Combat.Projectiles;
using Assets.Scripts.Enemies.AI;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States.EnemyStates;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Scripts.StateMachine.StateMachine))]
    public class Enemy : MonoBehaviour
    {
        public bool CanAttack = false;
        public StateMachine.StateMachine StateMachine;
        private GameObject _gameObject;
        private WanderState _wanderState;

        public float wanderRadius = 5f;
        public float wanderTimer = 5f;

        public int maxWanderMoves = 5;
        public bool isWandering = true;

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
            _wanderState = new Scripts.StateMachine.States.EnemyStates.WanderState();

            StateMachine = GetComponent<Scripts.StateMachine.StateMachine>();
            //Set initial state
            
            StateMachine.ChangeState(_wanderState, null);
        }

        public void Update()
        {
            //if(_chaseTarget.isChasing)
            //{
            //    _fieldOfView.TargetFound = false;
            //    _wander.isWandering = false;
            //    CanAttack = true;
            //}
            //else if (_fieldOfView.TargetFound)
            //{
            //    _fieldOfView.StopFOV();
            //    _chaseTarget.Chase(_fieldOfView.targetPosition);
            //    CanAttack = true;
            //}
            //else
            //{
            //    _wander.isWandering = true;
            //    _fieldOfView.StartFOV();
            //    CanAttack = false;
            //}
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