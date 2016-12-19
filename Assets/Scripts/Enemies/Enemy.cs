using Assets.Scripts.Combat.Projectiles;
using Assets.Scripts.Enemies.AI;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States.EnemyStates;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
		public int TurnSpeed = 10;
        public Transform ChaseTarget;
        //[HideInInspector]
        public bool isTraveling = false;
        //[HideInInspector]
        public Vector3[] Path;
        //[HideInInspector]
        public int TargetIndex;

		[HideInInspector]
		private List<Transform> visibleTargets = new List<Transform>();

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


		public void Chase()
		{
			if(!isTraveling && ChaseTarget != null)
				PathRequestManager.RequestPath(transform.position, ChaseTarget.position, OnPathFound);
		}

		public void GoHome()
		{
			if(!isTraveling)
				PathRequestManager.RequestPath(transform.position, StartPosition, OnPathFound);
		}


		IEnumerator FollowPath()
		{
			Vector3 currentWaypoint = Path[0];
			while(true)
			{
				if(transform.position == currentWaypoint)
				{
					TargetIndex++;
					if(TargetIndex >= Path.Length)
					{
						isTraveling = false;
						yield break;
					}
					else
					{
						isTraveling = true;
						currentWaypoint = Path[TargetIndex];
					}
				}

				float chaseStep = ChaseSpeed * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, chaseStep);

				float turnStep = TurnSpeed * Time.deltaTime;
				Vector3 targetDir = ChaseTarget.position - transform.position;
				Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, turnStep, 0.0F);
				transform.rotation = Quaternion.LookRotation(newDir);

				yield return null;
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

        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
			if (pathSuccessful)
			{
				Path = newPath;
				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}
            else
            {
				Path = null;
				StopCoroutine("FollowPath");
            }
        }

		public void FindVisibleTargets()
		{

			visibleTargets.Clear();
			Transform tempTarget = null;

			Collider[] targetsInviewRadius = Physics.OverlapSphere(transform.position, targetDetectionRadius, targetMask); //TargetMask should be looking for player
			//If we have no "targets", clean up and get outta here!
			if (targetsInviewRadius.Length != 0)
			{
				for (int i = 0; i < targetsInviewRadius.Length; i++)
				{
					Transform target = targetsInviewRadius[i].transform;
					Vector3 dirToTarget = (target.position - transform.position).normalized;

					float dstToTarget = Vector3.Distance(transform.position, target.position);
					if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
					{
						visibleTargets.Add(target);
						var playerPosition = target.transform.position;
						tempTarget = target;
						break;
					}
				}
			}
			ChaseTarget = tempTarget;
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