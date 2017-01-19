using UnityEngine;

public class GameManager : MonoBehaviour
{
    public StateMachine StateMachine;
    public LevelManager LevelManager;
    public GUIManager GuiManager;

    [HideInInspector]
    public bool GameIsPaused;

    // Use this for initialization
    private void Start()
    {
        GuiManager.OnExitGame = ExitGame;
        StateMachine.ChangeGlobalState(TitleState.Instance);
    }

    // Update is called once per frame
    private void Update()
    {
        StateMachine.Update();
    }

    public void StartGame()
    {
        StateMachine.ChangeGlobalState(GameSetupState.Instance);
    }

    public void ResumeGame()
    {
        StateMachine.ChangeGlobalState(GamePlayingState.Instance);
    }

    public void OnPlayerDeath()
    {
        StateMachine.ChangeGlobalState(GameRestartState.Instance);
    }

    public void RestartGame()
    {
        StateMachine.ChangeGlobalState(GameRestartState.Instance);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}