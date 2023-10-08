using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    CarInputActions carInputActions;

    public bool isGamePaused = false;

    void Awake()
    {
        carInputActions = new CarInputActions();

        if (pauseMenu == null)
        {
            Debug.LogWarning("Pause Menu NOT Found");
            return;
        }

        pauseMenu.SetActive(false);
        carInputActions.PauseMenu.TogglePause.performed += (InputAction.CallbackContext context) =>
        {
            // Toggle Pause
            isGamePaused = !isGamePaused;

            if (isGamePaused) PauseGame();
            else ResumeGame();
        };
    }

    void OnEnable()
    {
        carInputActions.PauseMenu.Enable();
    }

    void OnDisable()
    {
        carInputActions.PauseMenu.Disable();
    }

    void PauseGame()
    {
        if (pauseMenu == null)
            return;

        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        if (pauseMenu == null)
            return;

        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();

#else
		Application.Quit();
#endif
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //! TO DO :
    public void OpenSettingsMenu()
    {

    }

    public void CloseSettingsMenu()
    {

    }

    public void OpenMainMenu()
    {
        Time.timeScale = 1;
        // Scene 0 : Main Menu
        // Scene 1 : Level 1
        SceneManager.LoadScene(0);
    }
}
