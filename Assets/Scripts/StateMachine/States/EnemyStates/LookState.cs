
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

        public override void Enter(GameObject entity, params GameObject[] args)
        {
			System.Diagnostics.Debug.WriteLine("Entered Look State");
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            var enemy = entity.GetComponent<Enemy>();
            enemy.FindVisibleTargets();


			//if we have found the player and we can see him, chase his ass
			if (enemy.ChaseTarget != null)
			{
				enemy.stateMachine.ChangeGlobalState(ChaseState.Instance, null);
				//enemy.stateMachine.ChangeState(LookState.Instance, null);
			}
			else
			{
				enemy.stateMachine.ChangeGlobalState(IdleState.Instance, null);
			}
        }

		public override void Exit(GameObject entity, params GameObject[] args) { System.Diagnostics.Debug.WriteLine("Exited Look State");}


    }
}
