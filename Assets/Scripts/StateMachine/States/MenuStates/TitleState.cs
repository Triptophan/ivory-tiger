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

        public override void Enter(GameObject entity, params GameObject[] args)
        {
            if (_titleMenuObject == null) _titleMenuObject = args[0];
            if (_levelManager == null) _levelManager = _titleMenuObject.GetComponent<LevelManager>();

            _titleMenuObject.SetActive(true);
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
        }

        public override void Exit(GameObject entity, params GameObject[] args)
        {
            if (_titleMenuObject != null) _titleMenuObject.SetActive(false);
        }
    }
}