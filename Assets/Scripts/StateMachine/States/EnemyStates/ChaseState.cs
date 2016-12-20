
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

//			if (enemy.ChaseTarget == null || (enemy.Path != null && enemy.Path.Length > 0 && enemy.TargetIndex > enemy.Path.Length -1)) //We have nothing to chase so get out of here *for now*
//			{
//				enemy.stateMachine.ChangeState(LookState.Instance, null);
//				enemy.stateMachine.ChangeGlobalState(WanderState.Instance, null);
//				return;
//			}

			if(enemy.isTraveling ) //We are traveling along the calculated path
				return;


			enemy.Chase();

		}

		public override void Exit(GameObject entity, params GameObject[] args)
		{ 
			var enemy = entity.GetComponent<Enemy>();
			enemy.isChasing = false;
		}
    }
}
