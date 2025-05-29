using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] GameObject guideScreen;

    void Start()
    {
        guideScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButton("Guide") || Input.GetKey(KeyCode.JoystickButton6))
        {
            if (guideScreen != null)
            {
                Pause.isGamePaused = true;
                Time.timeScale = 0;
                guideScreen.SetActive(true);
            }
        }
        else
        {
            Pause.isGamePaused = false;
            Time.timeScale = 1;
            guideScreen.SetActive(false);
        }
    }
}
