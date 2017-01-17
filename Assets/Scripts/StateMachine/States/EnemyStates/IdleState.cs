using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.StateMachine.States.EnemyStates
{
    public class IdleState : State
    {
        private static IdleState _instance;
        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new IdleState();
                return _instance;
            }
        }
        public override void Enter(GameObject entity)
        {
            
        }

        public override void Execute(GameObject entity)
        {
            
        }

        public override void Exit(GameObject entity)
        {
            
        }
    }
}
