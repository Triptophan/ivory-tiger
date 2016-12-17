using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.StateMachine.States.GameFlowStates
{
    public class GameStartState : State
    {
        private GameManager _gameManager;
        private LevelManager _levelManager;
        private GameObject _loadingTextObject;

        private static GameStartState _instance;

        public static State Instance
        {
            get
            {
                if (_instance == null) _instance = new GameStartState();
                return _instance;
            }
        }

        public override void Enter(GameObject entity, params GameObject[] args)
        {
            _gameManager = entity.GetComponent<GameManager>();

            _levelManager = _gameManager.LevelManager;
        }

        public override void Execute(GameObject entity, params GameObject[] args)
        {
            throw new NotImplementedException();
        }

        public override void Exit(GameObject entity, params GameObject[] args)
        {
            throw new NotImplementedException();
        }

        private void ParseArguments(GameObject[] args)
        {
            foreach(GameObject go in args)
            {
                var levelMangager = go.GetComponent<LevelManager>();
                if(levelMangager != null)
                {
                    _levelManager = levelMangager;
                    continue;
                }

                _loadingTextObject = go;
            }

            if (_levelManager == null) throw new ArgumentException("LevelManager is required for GameStartState");
            if (_loadingTextObject == null) throw new ArgumentException("LoadingTextObject is required for GameStartState");
        }
    }
}
