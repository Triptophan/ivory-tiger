using UnityEngine;

namespace Assets.Scripts.StateMachine.States.MenuStates
{
    public class TitleState : State
    {
        private static TitleState _instance;

        private GameObject _titleMenuObject;

        private LevelManager _levelManager;

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
           
        }

        public override void Execute(GameObject entity)
        {
        }

        public override void Exit(GameObject entity)
        {
            if (_titleMenuObject != null) _titleMenuObject.SetActive(false);
        }
    }
}