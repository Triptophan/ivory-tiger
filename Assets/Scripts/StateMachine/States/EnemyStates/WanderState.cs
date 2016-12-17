
using Assets.Scripts.Enemies;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    [Serializable]
    public class WanderState : State
    {
        private static WanderState _instance;
        private Enemy _EnemyObject;
        private LevelManager _levelManager;
        private Transform mob;
        //Start Position of the mob.
        private Vector3 startPosition;

        private NavMeshAgent _agent;
        private float timer;
        private int numberOfMoves = 0;

        public Transform Target;
        float speed = 5;



        private bool _pathFound = false;
        private bool _pathRequested = false;

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
            _agent = entity.GetComponent<NavMeshAgent>();
            _EnemyObject = entity.GetComponent<Enemy>();
            _levelManager = _EnemyObject.LevelManager;
            

            timer = _EnemyObject.wanderTimer ;
            mob = entity.transform;
            startPosition = mob.position;

            _EnemyObject.isWandering = true;
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            //timer += Time.deltaTime;

            //if (timer >= _EnemyObject.wanderTimer && _agent.remainingDistance <= _agent.stoppingDistance)
            //{
            //    if (numberOfMoves == _EnemyObject.maxWanderMoves)
            //    {
            //        _agent.SetDestination(startPosition);
            //        numberOfMoves = 0;
            //        timer = 0;
            //    }
            //    else
            //    {

            //        //_agent.SetDestination(newPos);
            //        timer = 0;
            //        numberOfMoves++;
            //    }
            //}



        }



        public override void Exit(GameObject entity, params GameObject[] args) { _EnemyObject.isWandering = false; }


    }
}
