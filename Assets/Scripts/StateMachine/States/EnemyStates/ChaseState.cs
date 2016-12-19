
using Assets.Scripts.Enemies;
using System.Collections;
using UnityEngine;
using System;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    public class ChaseState : State
    {
        private static ChaseState _instance;

        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new ChaseState();
                return _instance;
            }
        }

        public override void Enter(GameObject entity, params GameObject[] args)
        {
            var enemy = entity.GetComponent<Enemy>();
            enemy.isChasing = true;
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            var enemy = entity.GetComponent<Enemy>();
            var mob = entity.transform;

            if (enemy.ChaseTarget == null) //We have nothing to chase so get out of here *for now*
                return;

            if (enemy.PathFound) // we have a path, let's follow it
            {
                if (enemy.Path.Length > 0)
                {
                    var currentWaypoint = enemy.Path[enemy.TargetIndex];
                    mob.position = Vector3.MoveTowards(mob.position, currentWaypoint, enemy.ChaseSpeed * Time.deltaTime);

                    if (mob.position == currentWaypoint)
                    {
                        enemy.TargetIndex++;
                    }
                    if (enemy.TargetIndex > enemy.Path.Length - 1) //we've walked the whole path so we're done
                    {
                        enemy.stateMachine.ChangeState(LookState.Instance, null);
                        enemy.stateMachine.ChangeGlobalState(WanderState.Instance, null);
                    }
                }
                else
                {
                    enemy.PathFound = false;
                }
            }
            else if (!enemy.PathRequested)//We do not currently have a path and we are not waiting on one
            {
                PathRequestManager.RequestPath(entity.transform.position, enemy.ChaseTarget.transform.position, enemy.OnPathFound);
                enemy.PathRequested = true;
            }
        }

        public override void Exit(GameObject entity, params GameObject[] args)
        {
            var enemy = entity.GetComponent<Enemy>();
            enemy.PathFound = false;
            enemy.TargetIndex = 0;
            enemy.ChaseTarget = null;
            enemy.isChasing = false;
        }
    }
}
