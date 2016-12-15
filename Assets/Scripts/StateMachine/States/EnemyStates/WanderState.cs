
using Assets.Scripts.Enemies;
using UnityEngine;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    public class WanderState : State
    {
        private static WanderState _instance;

        private Enemy _EnemyObject;

        private LevelManager _levelManager;



        private Transform mob;

        //Start Position of the mob.
        private Vector3 startPosition;

        private NavMeshAgent agent;
        private float timer;
        private int numberOfMoves = 0;

        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new WanderState();
                return _instance;
            }
        }

        public override void Enter(GameObject entity, params GameObject[] args)
        {
            agent = entity.GetComponent<NavMeshAgent>();
            _EnemyObject = entity.GetComponent<Enemy>();
            timer = _EnemyObject.wanderTimer ;
            mob = entity.transform;
            startPosition = mob.position;
            _EnemyObject.isWandering = true;
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            timer += Time.deltaTime;

            if (timer >= _EnemyObject.wanderTimer && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (numberOfMoves == _EnemyObject.maxWanderMoves)
                {
                    agent.SetDestination(startPosition);
                    numberOfMoves = 0;
                    timer = 0;
                }
                else
                {
                    Vector3 newPos = RandomNavSphere(entity.transform.position, _EnemyObject.wanderRadius, -1);
                    agent.SetDestination(newPos);
                    timer = 0;
                    numberOfMoves++;
                }
            }
        }

        public override void Exit(GameObject entity, params GameObject[] args) { _EnemyObject.isWandering = false; }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;
            randDirection += origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
            return navHit.position;
        }
    }
}
