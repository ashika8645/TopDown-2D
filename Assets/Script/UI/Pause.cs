using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu, settingsMenu;
    public string sceneName;
    public bool toggle;
    public static bool isGamePaused = false;

    private void Start()
    {
        Canvas canvas = pauseMenu.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.scaleFactor = 0.75f;
            canvas.referencePixelsPerUnit = 16;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Escape") || Input.GetKeyDown(KeyCode.JoystickButton7))
        {

            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }
            else
            {
                toggle = !toggle;
                if (toggle)
                {
                    pauseMenu.SetActive(true);
                    AudioListener.pause = true;
                    Time.timeScale = 0;
                    isGamePaused = true;
                }
                else
                {
                    pauseMenu.SetActive(false);
                    AudioListener.pause = false;
                    Time.timeScale = 1;
                    isGamePaused = false;
                }

            }
        }
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            pauseMenu.SetActive(false);
        }
    }

    public void toSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void backToPause()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void resumeGame()
    {
        toggle = false;
        pauseMenu.SetActive(false);
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void quitToMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = true;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            Destroy(player);

        GameObject uiCanvas = GameObject.FindWithTag("UICanvas");
        if (uiCanvas != null)
            Destroy(uiCanvas);

        PlayerSpawn.ResetSpawnState();

        SceneManager.LoadScene(sceneName);
    }


    public void quitToDesktop()
    {
        Time.timeScale = 1;
        AudioListener.pause = true;
        Debug.Log("Quit to desktop");
        Application.Quit();
    }
}
