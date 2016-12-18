using Assets.Scripts;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States.GameFlowStates;
using Assets.Scripts.StateMachine.States.MenuStates;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public StateMachine StateMachine;
    public LevelManager LevelManager;
    public GUIManager GuiManager;

    // Use this for initialization
    private void Start()
    {
        StateMachine.ChangeGlobalState(TitleState.Instance);
    }

    // Update is called once per frame
    private void Update()
    {
        StateMachine.Update();
    }

    private void StartGame()
    {
        StateMachine.ChangeGlobalState(GameStartState.Instance);
    }
}