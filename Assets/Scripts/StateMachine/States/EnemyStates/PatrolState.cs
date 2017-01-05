
using Assets.Scripts.Enemies;
using System.Collections;
using UnityEngine;
using System;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
	public class PatrolState : State
	{
		private static PatrolState _instance;

		public static State Instance
		{
			get
			{
				if (_instance == null) _instance = new PatrolState();
				return _instance;
			}
		}

		public override void Enter(GameObject entity, params GameObject[] args) 
		{ 
			var enemy = entity.GetComponent<Enemy>();
            enemy.isPatrolling = true;

        }


		public override void Execute(GameObject entity, params GameObject[] args)
		{
			var enemy = entity.GetComponent<Enemy>();



			if(enemy.isTraveling) //We are traveling along the calculated path
				return;

            
			enemy.Patrol();


        }

		public override void Exit(GameObject entity, params GameObject[] args)
		{ 
			var enemy = entity.GetComponent<Enemy>();
			enemy.isPatrolling = false;
		}
	}
}