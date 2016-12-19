
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
            var enemy = entity.GetComponent<Enemy>();
            enemy.isWandering = true;
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            var enemy = entity.GetComponent<Enemy>();
            var mob = entity.transform;
            enemy.isWandering = true;
        }

        public override void Exit(GameObject entity, params GameObject[] args)
        {
            var enemy = entity.GetComponent<Enemy>();
            enemy.isWandering = false;
        }


    }
}
