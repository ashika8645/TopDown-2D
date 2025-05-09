using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown, resolutionDropdown;
    public Slider volumeSlider;
    public bool inGame;

    void Start()
    {
        if (PlayerPrefs.GetInt("settingsSaved", 0) == 0)
        {
            PlayerPrefs.SetInt("Graphics", 0);
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.SetFloat("Volume", 1.0f);
            PlayerPrefs.SetInt("Vignette", 1);
            PlayerPrefs.SetInt("Chromatic", 1);
            PlayerPrefs.SetInt("Grain", 1);
        }

        if (PlayerPrefs.GetInt("Graphics", 2) == 2)
        {
            graphicsDropdown.value = 0;
            QualitySettings.SetQualityLevel(0);
        }
        else if (PlayerPrefs.GetInt("Graphics", 1) == 1)
        {
            graphicsDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);
        }
        else if (PlayerPrefs.GetInt("Graphics", 0) == 0)
        {
            graphicsDropdown.value = 2;
            QualitySettings.SetQualityLevel(2);
        }

        // Resolution
        if (PlayerPrefs.GetInt("Resolution", 2) == 2)
        {
            resolutionDropdown.value = 0;
            Screen.SetResolution(854, 480, true);
        }
        else if (PlayerPrefs.GetInt("Resolution", 1) == 1)
        {
            resolutionDropdown.value = 1;
            Screen.SetResolution(1280, 720, true);
        }
        else if (PlayerPrefs.GetInt("Resolution", 0) == 0)
        {
            resolutionDropdown.value = 2;
            Screen.SetResolution(1920, 1080, true);
        }

        // Volume
        volumeSlider.value = PlayerPrefs.GetFloat("mastervolume", 1.0f);
        AudioListener.volume = volumeSlider.value;

    }

    public void SetGraphics()
    {
        if (graphicsDropdown.value == 0)
        {
            PlayerPrefs.SetInt("Graphics", 2);
            PlayerPrefs.Save();
            QualitySettings.SetQualityLevel(0);
        }
        else if (graphicsDropdown.value == 1)
        {
            PlayerPrefs.SetInt("Graphics", 1);
            PlayerPrefs.Save();
            QualitySettings.SetQualityLevel(1);
        }
        else if (graphicsDropdown.value == 2)
        {
            PlayerPrefs.SetInt("Graphics", 0);
            PlayerPrefs.Save();
            QualitySettings.SetQualityLevel(2);
        }
    }

    public void SetResolution()
    {
        if (resolutionDropdown.value == 0)
        {
            PlayerPrefs.SetInt("Resolution", 2);
            PlayerPrefs.Save();
            Screen.SetResolution(854, 480, true);
        }
        else if (resolutionDropdown.value == 1)
        {
            PlayerPrefs.SetInt("Resolution", 1);
            PlayerPrefs.Save();
            Screen.SetResolution(1280, 720, true);
        }
        else if (resolutionDropdown.value == 2)
        {
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.Save();
            Screen.SetResolution(1920, 1080, true);
        }
    }

    public void SetVolume()
    {
        PlayerPrefs.SetFloat("mastervolume", volumeSlider.value);
        PlayerPrefs.Save();
        AudioListener.volume = volumeSlider.value;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("settingsSaved", 1);
        PlayerPrefs.Save();
    }
}
