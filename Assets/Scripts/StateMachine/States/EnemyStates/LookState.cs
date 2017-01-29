
using Assets.Scripts.Enemies;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    public class LookState : State
    {
        private static LookState _instance;



        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new LookState();
                return _instance;
            }
        }

        public override void Enter(GameObject entity)
        {
			System.Diagnostics.Debug.WriteLine("Entered Look State");
        }

        public override void Execute(GameObject entity)
        {
            var enemy = entity.GetComponent<Enemy>();
            enemy.FindVisibleTargets();


			//if we have found the player and we can see him, chase his ass
			if (enemy.ChaseTarget != null && !enemy.isChasing)
			{
				enemy.stateMachine.ChangeGlobalState(ChaseState.Instance);
				//enemy.stateMachine.ChangeState(LookState.Instance, null);
			}
			else
			{
                if (!enemy.isPatrolling)
				    enemy.stateMachine.ChangeGlobalState(PatrolState.Instance);
			}
        }

		public override void Exit(GameObject entity) { System.Diagnostics.Debug.WriteLine("Exited Look State");}


    }
}
