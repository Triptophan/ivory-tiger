using UnityEngine;

namespace Assets.Scripts.StateMachine.States.MenuStates
{
    public class TitleState : State
    {
        private static TitleState _instance;

        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new TitleState();
                return _instance;
            }
        }

        public override void Enter(GameObject entity)
        {
            var gameManager = entity.GetComponent<GameManager>();

            gameManager.GuiManager.TitleMenu.SetActive(true);
        }

        public override void Execute(GameObject entity)
        {
        }

        public override void Exit(GameObject entity)
        {
            var gameManager = entity.GetComponent<GameManager>();

            gameManager.GuiManager.TitleMenu.SetActive(false);
        }
    }
}