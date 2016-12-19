
using Assets.Scripts.Enemies;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    public class LookState : State
    {
        private static LookState _instance;

        [HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();

        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new LookState();
                return _instance;
            }
        }

        public override void Enter(GameObject entity, params GameObject[] args)
        {
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            var enemy = entity.GetComponent<Enemy>();
            FindVisibleTargets(enemy);
        }

        public override void Exit(GameObject entity, params GameObject[] args) { }

        private void FindVisibleTargets(Enemy enemy)
        {
            var mob = enemy.transform;
            visibleTargets.Clear();
            enemy.ChaseTarget = null;

            Collider[] targetsInviewRadius = Physics.OverlapSphere(mob.position, enemy.targetDetectionRadius, enemy.targetMask); //TargetMask should be looking for player
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
                if (!Physics.Raycast(mob.position, dirToTarget, dstToTarget, enemy.obstacleMask))
                {
                    visibleTargets.Add(target);
                    var playerPosition = target.transform.position;
                    enemy.ChaseTarget = target;
                    break;
                }
            }

            //if we have found the player and we can see him, chase his ass
            if (enemy.ChaseTarget != null)
            {
                enemy.stateMachine.ChangeGlobalState(ChaseState.Instance, null);
                enemy.stateMachine.ChangeState(IdleState.Instance, null);
            }

        }
    }
}
