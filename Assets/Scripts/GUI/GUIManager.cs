using Assets.Scripts;
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

    public void ExitGame()
    {
        OnExitGame();
    }

    #endregion Button Events

    private void Update()
    {
        if (PlayerCombatController && PlayerCombatController.PlayerHealthChanged == null)
        {
            PlayerCombatController.PlayerHealthChanged = UpdatePlayerHealth;
        }

        Time.timeScale = _inGameMenuVisible ? 0f : 1f;
    }

    public void ToggleGameOver(bool enabled)
    {
        GameOverScreen.SetActive(enabled);
        PlayerHealthIndicator.enabled = !enabled;
        HealthBar.SetActive(!enabled);
        if (!enabled) UpdatePlayerHealth();
    }

    private void UpdatePlayerHealth()
    {
        PlayerHealthIndicator.fillAmount = PlayerCombatController.PlayerHealthIndicatorFillAmount;
    }
}