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
        [HideInInspector]
        public StateMachine.StateMachine stateMachine;
        [HideInInspector]
        public LevelManager LevelManager;
        private GameObject _gameObject;

        //States
        public WanderState wanderState;
        public LookState lookState;
        public ChaseState chaseState;
        public IdleState idleState;

        //Wander Properties
        [Header("Wander State")]
        public float wanderRadius = 5f;
        public float wanderTimer = 5f;
        public int maxWanderMoves = 5;
        public bool isWandering = true;

        //Look Properties
        [Header("Look State")]
        public LayerMask obstacleMask;
        public float targetDetectionRadius = 10f;
        public LayerMask targetMask;

        //Chase Properties
        [Header("Chase State")]
        public int ChaseSpeed = 5;
        public Transform ChaseTarget;
        [HideInInspector]
        public bool isChasing = false;

        [HideInInspector]
        public Vector3[] path;
        [HideInInspector]
        public int targetIndex;


        public bool Active
        {
            get { return isActiveAndEnabled; }
            set
            {
                if (_gameObject == null) _gameObject = gameObject;
                _gameObject.SetActive(value);
            }
        }

        [HideInInspector]
        public List<Enemy> EnemyPool;
        [HideInInspector]
        public List<Enemy> DeadEnemyPool;

        public void Awake()
        {
            _gameObject = gameObject;
            _gameObject.tag = "Enemy";
            _gameObject.layer = LayerMask.NameToLayer("Enemies");

            //Get yo states!
            wanderState = new WanderState();
            lookState = new LookState();
            chaseState = new ChaseState();
            idleState = new IdleState();

            //Instantiate the state machine
            stateMachine = GetComponent<StateMachine.StateMachine>();
            
            //Set initial state
            stateMachine.ChangeGlobalState(wanderState, null);
            stateMachine.ChangeState(lookState, null);
        }
        public void Update()
        {

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

        //public void OnDrawGizmos()
        //{
        //    if (path != null)
        //    {
        //        for (int i = targetIndex; i < path.Length; i++)
        //        {
        //            Gizmos.color = Color.black;
        //            Gizmos.DrawCube(path[i], Vector3.one);

        //            if (i == targetIndex)
        //            {
        //                Gizmos.DrawLine(transform.position, path[i]);
        //            }
        //            else
        //            {
        //                Gizmos.DrawLine(path[i - 1], path[i]);
        //            }

        //        }
        //    }
        //}



    }
}