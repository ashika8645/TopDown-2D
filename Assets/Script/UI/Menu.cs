using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class Menu : MonoBehaviour
{
    public GameObject loadingScreen, menuObj, settingsObj;
    public List<string> sceneName;
    public Button continueButton;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        continueButton.interactable = PlayerPrefs.GetInt("level") != 0;
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("level") == 0)
        {
            continueButton.interactable = false;
        }
        else
        {
        }
    }

    public void playGame()
    {
        loadingScreen.SetActive(true);
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName[0]);
    }
    public void MenuGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetStats();
            Destroy(GameManager.Instance.gameObject);
        }

        if (AudioScript.Instance != null)
        {
            DestroyImmediate(AudioScript.Instance.gameObject); 
        }

        PlayerSpawn.ResetSpawnState();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) Destroy(player);

        GameObject canvas = GameObject.Find("UICanvas");
        if (canvas != null) Destroy(canvas);

        GameObject cam = GameObject.Find("Camera");
        if (cam != null) Destroy(cam);

        SceneManager.LoadScene("Menu");
    }



    public void continueGame()
    {
        loadingScreen.SetActive(true);

        int level = PlayerPrefs.GetInt("level", 0);

        if (level >= 1 && level <= sceneName.Count)
        {
            SceneManager.LoadScene(sceneName[level - 1]);
        }
    }

    public void SettingsMenu()
    {
        menuObj.SetActive(false);
        settingsObj.SetActive(true);
    }

    public void quitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void backToMenu()
    {
        settingsObj.SetActive(false);
        menuObj.SetActive(true);
    }
}
