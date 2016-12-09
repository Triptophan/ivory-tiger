using UnityEngine;

namespace Assets.Scripts.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        private State _previousState;
        private State _previousGlobalState;

        private State _currentState;
        private State _currentGlobalState;

        private GameObject _gameObject;

        public void Awake()
        {
            _gameObject = gameObject;
        }

        public void Update()
        {
            if (_currentGlobalState != null) _currentGlobalState.Execute(_gameObject);
            if (_currentState != null) _currentState.Execute(_gameObject);
        }

        public void ChangeState(State newState, params GameObject[] args)
        {
            if (newState == null) return;

            _previousState = _currentState;

            if (_currentState != null) _currentState.Exit(_gameObject, args);

            _currentState = newState;
            _currentState.Enter(_gameObject, args);
        }

        public void ChangeGlobalState(State newState, params GameObject[] args)
        {
            if (newState == null) return;

            _previousGlobalState = _currentGlobalState;

            if (_currentGlobalState != null) _currentGlobalState.Exit(_gameObject, args);

            _currentGlobalState = newState;
            _currentGlobalState.Enter(_gameObject, args);
        }

        public void RevertToPreviousState(params GameObject[] args)
        {
            if (_previousState != null) ChangeState(_previousState, args);
        }

        public void RevertToPreviousGlobalState(params GameObject[] args)
        {
            if (_previousGlobalState != null) ChangeGlobalState(_previousGlobalState, args);
        }

        public bool IsInState(State state)
        {
            return _currentState.GetType() == typeof(State);
        }

        public bool IsInGlobalState(State state)
        {
            return _currentGlobalState.GetType() == typeof(State);
        }
    }
}