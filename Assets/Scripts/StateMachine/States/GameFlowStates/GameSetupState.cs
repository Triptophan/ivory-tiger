using UnityEngine;

namespace Assets.Scripts.StateMachine.States.GameFlowStates
{
    public class GameSetupState : State
    {
        private KeyCode _menuKeyCode;

        private static GameSetupState _instance;

        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new GameSetupState();
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
            var gameManager = entity.GetComponent<GameManager>();
            var stateMachine = entity.GetComponent<StateMachine>();

            if (gameManager.LevelManager.GameReady) stateMachine.ChangeGlobalState(GamePlayingState.Instance);
        }

        public override void Exit(GameObject entity)
        {
            var gameManager = entity.GetComponent<GameManager>();

            gameManager.GuiManager.CrosshairImageObject.SetActive(true);
            gameManager.GuiManager.HealthBar.SetActive(true);
        }
    }
}