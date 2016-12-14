using Assets.Scripts;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    private bool _inGameMenuVisible = false;

    private KeyCode _menuKeyCode;

    public GameObject CrosshairImageObject;
    public GameObject InGameMenuObject;
    public GameObject GameOverScreen;
    public GameObject HealthBar;

    public CombatController PlayerCombatController;

    public LevelManager LevelManager;

    public Image PlayerHealthIndicator;

    #region Button Events

    public void ResumeGame()
    {
        _inGameMenuVisible = false;

        UpdateGUI();
    }

    public void RestartNewLevel()
    {
        ResumeGame();
        StartNewGame();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        GameOverScreen.SetActive(false);
        PlayerHealthIndicator.enabled = true;
        HealthBar.SetActive(true);
        LevelManager.RestartNewLevel();
        PlayerCombatController.ResetPlayer();
    }

    #endregion Button Events

    private void Awake()
    {
#if UNITY_EDITOR
        _menuKeyCode = KeyCode.BackQuote;
#else
        _menuKeyCode = KeyCode.Escape;
#endif
    }

    private void Update()
    {
        if (Input.GetKeyUp(_menuKeyCode))
        {
            _inGameMenuVisible = !_inGameMenuVisible;

            UpdateGUI();
        }

        Time.timeScale = _inGameMenuVisible ? 0f : 1f;

        TogglePlayer();

        if (PlayerCombatController)
        {
            PlayerHealthIndicator.fillAmount = PlayerCombatController.PlayerHealthIndicatorFillAmount;
            if (PlayerCombatController.IsDead) SetGameOver();
        }
    }

    private void UpdateGUI()
    {
        ToggleCrosshair();
        RenderInGameMenu();
    }

    private void ToggleCrosshair()
    {
        CrosshairImageObject.SetActive(!_inGameMenuVisible);
    }

    private void RenderInGameMenu()
    {
        Cursor.visible = _inGameMenuVisible;
        InGameMenuObject.SetActive(_inGameMenuVisible);
    }

    private void TogglePlayer()
    {
        if (!PlayerCombatController) return;

        PlayerCombatController.CanFire = !_inGameMenuVisible;
        PlayerCombatController.gameObject.SetActive(!_inGameMenuVisible);
    }

    private void SetGameOver()
    {
        GameOverScreen.SetActive(true);
        PlayerHealthIndicator.fillAmount = PlayerCombatController.PlayerHealthIndicatorFillAmount;PlayerHealthIndicator.enabled = false;
        HealthBar.SetActive(false);
    }
}