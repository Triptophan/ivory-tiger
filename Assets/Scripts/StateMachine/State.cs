using UnityEngine;

namespace Assets.Scripts.StateMachine
{
    public abstract class State
    {
        public abstract void Enter(GameObject entity, params GameObject[] args);

        public abstract void Execute(GameObject entity, params GameObject[] args);

        public abstract void Exit(GameObject entity, params GameObject[] args);
    }
}