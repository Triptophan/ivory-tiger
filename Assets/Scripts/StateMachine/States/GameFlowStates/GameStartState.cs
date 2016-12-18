using UnityEngine;

namespace Assets.Scripts.StateMachine.States.GameFlowStates
{
    public class GameStartState : State
    {
        private static GameStartState _instance;

        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new GameStartState();
                return _instance;
            }
        }

        public override void Enter(GameObject entity)
        {
            var gameManager = entity.GetComponent<GameManager>();

            gameManager.LevelManager.SetupLevel();
        }

        public override void Execute(GameObject entity)
        {
        }

        public override void Exit(GameObject entity)
        {
        }
    }
}