using UnityEngine;

public class GameRestartState : State
{
    private KeyCode _menuKeyCode;

    private static GameRestartState _instance;

    public static State Instance
    {
        get
        {
            if (_instance == null) _instance = new GameRestartState();
            return _instance;
        }
    }

    public override void Enter(GameObject entity)
    {
        var gameManager = entity.GetComponent<GameManager>();

        gameManager.LevelManager.RestartNewLevel();
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