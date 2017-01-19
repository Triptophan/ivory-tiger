using UnityEngine;

public class GamePausedState : State
{
    private KeyCode _menuKeyCode;

    private float _timer;
    private bool _canUnPause = false;

    private static GamePausedState _instance;

    public static State Instance
    {
        get
        {
            if (_instance == null) _instance = new GamePausedState();
            return _instance;
        }
    }

    public override void Enter(GameObject entity)
    {
        var gameManager = entity.GetComponent<GameManager>();

        gameManager.GameIsPaused = true;

        gameManager.GuiManager.CrosshairImageObject.SetActive(false);
        gameManager.GuiManager.InGameMenuObject.SetActive(true);

        gameManager.GuiManager.MouseManager.ToggleMouse();

#if UNITY_EDITOR
        _menuKeyCode = KeyCode.BackQuote;
#else
        _menuKeyCode = KeyCode.Escape;
#endif

        _timer = 0f;
    }

    public override void Execute(GameObject entity)
    {
        _timer += Time.deltaTime;

        if (_timer > 1f) _canUnPause = true;

        var gameManager = entity.GetComponent<GameManager>();
        var stateMachine = entity.GetComponent<StateMachine>();

        if (_canUnPause && gameManager.GameIsPaused && Input.GetKeyUp(_menuKeyCode))
        {
            stateMachine.ChangeGlobalState(GamePlayingState.Instance);
        }
    }

    public override void Exit(GameObject entity)
    {
        var gameManager = entity.GetComponent<GameManager>();

        gameManager.GuiManager.CrosshairImageObject.SetActive(true);
        gameManager.GuiManager.InGameMenuObject.SetActive(false);

        gameManager.GuiManager.MouseManager.ToggleMouse();

        _timer = 0f;
        _canUnPause = false;
    }
}