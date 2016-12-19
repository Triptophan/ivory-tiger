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
        
        [HideInInspector]
        public StateMachine.StateMachine stateMachine;
        [HideInInspector]
        public LevelManager LevelManager;
        private GameObject _gameObject;


        //Atack Properties
        [HideInInspector]
        public bool CanAttack = false;

        //Wander Properties
        [Header("Wander State")]
        public float WanderRadius = 5f;
        public float WanderTimer = 5f;
        public int MaximumWanderMoves = 5;
        public float WanderSpeed = 5;
        [HideInInspector]
        public bool isWandering = true;
        [HideInInspector]
        public Vector3 StartPosition;


        //Look Properties
        [Header("Look State")]
        public LayerMask obstacleMask;
        public float targetDetectionRadius = 10f;
        public LayerMask targetMask;

        //Chase Properties
        [Header("Chase State")]
        public int ChaseSpeed = 5;
        public Transform ChaseTarget;
        //[HideInInspector]
        public bool isChasing = false;
        //[HideInInspector]
        public bool PathFound = false;
        //[HideInInspector]
        public bool PathRequested = false;
        //[HideInInspector]
        public Vector3[] Path;
        //[HideInInspector]
        public int TargetIndex;


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
            StartPosition = transform.position;

            //Instantiate the state machine
            stateMachine = GetComponent<StateMachine.StateMachine>();
            
            //Set initial state
            stateMachine.ChangeGlobalState(WanderState.Instance, null);
            stateMachine.ChangeState(LookState.Instance, null);
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

        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                Path = newPath;
                PathFound = true;
                PathRequested = false;
            }
            else
            {
                ResetEnemyPath();
            }
        }

        private void ResetEnemyPath()
        {
            PathFound = false;
            PathRequested = false;
            Path = null;
        }
        public void OnDrawGizmos()
        {
            if (Path != null)
            {
                for (int i = TargetIndex; i < Path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(Path[i], Vector3.one);

                    if (i == TargetIndex)
                    {
                        Gizmos.DrawLine(transform.position, Path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(Path[i - 1], Path[i]);
                    }

                }
            }
        }



    }
}