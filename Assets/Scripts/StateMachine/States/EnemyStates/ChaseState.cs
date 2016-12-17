
using Assets.Scripts.Enemies;
using System.Collections;
using UnityEngine;
using System;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    public class ChaseState : State
    {
        private static WanderState _instance;
        private Enemy _EnemyObject;
        private LevelManager _levelManager;
        private Transform mob;
        public Transform Target;

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
            _EnemyObject = entity.GetComponent<Enemy>();
            _levelManager = _EnemyObject.LevelManager;
            mob = entity.transform;
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            if (_EnemyObject.ChaseTarget == null) //We have nothing to chase so get out of here *for now*
                return;

            if (_pathFound) // we have a path, let's follow it
            {
                if (_EnemyObject.path.Length > 0)
                {
                    var currentWaypoint = _EnemyObject.path[_EnemyObject.targetIndex];
                    mob.position = Vector3.MoveTowards(mob.position, currentWaypoint, _EnemyObject.ChaseSpeed * Time.deltaTime);

                    if (mob.position == currentWaypoint)
                    {
                        _EnemyObject.targetIndex++;
                    }
                    if (_EnemyObject.targetIndex > _EnemyObject.path.Length - 1) //we've walked the whole path so we're done
                    {
                        _pathFound = false;
                        _EnemyObject.targetIndex = 0;
                        _EnemyObject.ChaseTarget = null;
                        _EnemyObject.isChasing = false;
                        //_EnemyObject.stateMachine.ChangeGlobalState(_EnemyObject.wanderState, null);
                    }
                }
                else
                {
                    _pathFound = false;
                }
            }
            else if (!_pathRequested)//We do not currently have a path and we are not waiting on one
            {
                PathRequestManager.RequestPath(entity.transform.position, _EnemyObject.ChaseTarget.transform.position, OnPathFound);
                _pathRequested = true;
            }
        }

        public override void Exit(GameObject entity, params GameObject[] args) { }
        //public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        //{
        //    Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
        //    randDirection += origin;
        //    NavMeshHit navHit;
        //    NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);
        //    return navHit.position;
        //}

        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                _EnemyObject.path = newPath;
                _pathFound = true;
                _pathRequested = false;
            }
            else
            {
                ResetEnemyPath();
            }
        }
        private void ResetEnemyPath()
        {
            _pathFound = false;
            _pathRequested = false;
            _EnemyObject.path = null;
        }
    }
}
