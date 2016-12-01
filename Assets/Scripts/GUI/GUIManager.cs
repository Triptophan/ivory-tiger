using Assets.Scripts.Player;
using Assets.Scripts;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    private bool _inGameMenuVisible = false;

    public GameObject CrosshairImageObject;
    public GameObject InGameMenuObject;

    public CombatController PlayerCombatController;

    public LevelManager LevelManager;

    #region Button Events
    public void ResumeGame()
    {
        _inGameMenuVisible = false;

        UpdateGUI();
    }

    public void RestartNewLevel()
    {
        LevelManager.RestartNewLevel();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            _inGameMenuVisible = !_inGameMenuVisible;

            UpdateGUI();
        }

        Time.timeScale = _inGameMenuVisible ? 0f : 1f;

        TogglePlayer();
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
        PlayerCombatController.CanFire = !_inGameMenuVisible;
        PlayerCombatController.gameObject.SetActive(!_inGameMenuVisible);
    }
}