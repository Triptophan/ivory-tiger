using Assets.Scripts;
using Assets.Scripts.GUI;
using Assets.Scripts.Player;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    private bool _inGameMenuVisible = false;
    
    public GameObject CrosshairImageObject;
    public GameObject InGameMenuObject;
    public GameObject GameOverScreen;
    public GameObject HealthBar;
    public GameObject TitleMenu;

    public CombatController PlayerCombatController;

    public LevelManager LevelManager;
    public MouseManager MouseManager;

    public Image PlayerHealthIndicator;

    public Action OnExitGame { get; set; }

    #region Button Events
    
    public void RestartNewLevel()
    {
        StartNewGame();
    }

    public void ExitGame()
    {
        OnExitGame();
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

    private void Update()
    {

        Time.timeScale = _inGameMenuVisible ? 0f : 1f;

        TogglePlayer();

        if (PlayerCombatController)
        {
            PlayerHealthIndicator.fillAmount = PlayerCombatController.PlayerHealthIndicatorFillAmount;
            if (PlayerCombatController.IsDead) SetGameOver();
        }
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