using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthUIManager : MonoBehaviour
{
    public static BossHealthUIManager Instance { get; private set; }

    private GameObject bossHealthPanel;
    private Slider healthSlider;
    private bool isReady = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(FindUIRoutine());
    }

    private IEnumerator FindUIRoutine()
    {
        GameObject canvas = null;

        while (canvas == null)
        {
            canvas = GameObject.FindWithTag("UICanvas");
            yield return null;
        }

        Transform bossPanel = canvas.transform.Find("BossHealth");

        if (bossPanel != null)
        {
            bossHealthPanel = bossPanel.gameObject;
            healthSlider = bossHealthPanel.GetComponentInChildren<Slider>(true);

            bossHealthPanel.SetActive(false);
            isReady = true;
        }
        else
        {
            Debug.LogError("BossHealth panel not found in UICanvas.");
        }
    }

    public void ShowBossHealth(int maxHealth)
    {
        if (!isReady || bossHealthPanel == null || healthSlider == null) return;

        bossHealthPanel.SetActive(true);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(int currentHealth)
    {
        if (!isReady || healthSlider == null) return;
        healthSlider.value = currentHealth;
    }

    public void HideBossHealth()
    {
        if (!isReady || bossHealthPanel == null) return;
        bossHealthPanel.SetActive(false);
    }
}
