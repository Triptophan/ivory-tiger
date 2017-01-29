using UnityEngine;

public class GamePlayingState : State
{
    private KeyCode _menuKeyCode;

    private float _timer;
    private bool _canPause = false;

    private static GamePlayingState _instance;

    public static State Instance
    {
        get
        {
            if (_instance == null) _instance = new GamePlayingState();
            return _instance;
        }
    }

    public override void Enter(GameObject entity)
    {
        var gameManager = entity.GetComponent<GameManager>();

        gameManager.GameIsPaused = false;

        gameManager.GuiManager.CrosshairImageObject.SetActive(true);
        gameManager.GuiManager.InGameMenuObject.SetActive(false);

        gameManager.LevelManager.PlayerCombatController.CanFire = true; 

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

        if (_timer > 1f) _canPause = true;

        var gameManager = entity.GetComponent<GameManager>();
        var stateMachine = entity.GetComponent<StateMachine>();

        if (_canPause && !gameManager.GameIsPaused && Input.GetKeyUp(_menuKeyCode))
        {
            stateMachine.ChangeGlobalState(GamePausedState.Instance);
        }
    }

    public override void Exit(GameObject entity)
    {
        var gameManager = entity.GetComponent<GameManager>();

        gameManager.GuiManager.CrosshairImageObject.SetActive(true);

        //Deactivate player & enemies

        gameManager.LevelManager.PlayerCombatController.CanFire = false;

        _timer = 0f;
        _canPause = false;
    }
}