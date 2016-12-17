
using Assets.Scripts.Enemies;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    public class LookState : State
    {


        private static WanderState _instance;
        private Enemy _EnemyObject;
        private LevelManager _levelManager;
        private Transform mob;

        [HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();

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
            FindVisibleTargets();
        }

        public override void Exit(GameObject entity, params GameObject[] args) { }

        private void FindVisibleTargets()
        {
            visibleTargets.Clear();
            _EnemyObject.ChaseTarget = null;

            Collider[] targetsInviewRadius = Physics.OverlapSphere(mob.position, _EnemyObject.targetDetectionRadius, _EnemyObject.targetMask); //TargetMask should be looking for player
            //If we have no "targets", clean up and get outta here!
            if (targetsInviewRadius.Length == 0)
            {
                return;
            }

            for (int i = 0; i < targetsInviewRadius.Length; i++)
            {
                Transform target = targetsInviewRadius[i].transform;
                Vector3 dirToTarget = (target.position - mob.position).normalized;

                float dstToTarget = Vector3.Distance(mob.position, target.position);
                if (!Physics.Raycast(mob.position, dirToTarget, dstToTarget, _EnemyObject.obstacleMask))
                {
                    visibleTargets.Add(target);
                    var playerPosition = target.transform.position;
                    _EnemyObject.ChaseTarget = target;
                    break;
                }
            }

            //if we have found the player and we can see him, chase his ass
            if (_EnemyObject.ChaseTarget != null)
            {
                _EnemyObject.stateMachine.ChangeGlobalState(_EnemyObject.chaseState, null);
                _EnemyObject.stateMachine.ChangeState(_EnemyObject.idleState, null);
            }

        }
    }
}
