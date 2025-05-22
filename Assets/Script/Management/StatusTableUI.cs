using TMPro;
using UnityEngine;

public class StatusTableUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject statusTablePanel;

    public TMP_Text swordDamageText;
    public TMP_Text bowDamageText;
    public TMP_Text spellDamageText;

    public static bool isStatusTableOpen = false;

    private float previousTimeScale;

    void Start()
    {
        statusTablePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Status"))
        {
            ShowStatusTable();
        }
        if (Input.GetButtonUp("Status"))
        {
            HideStatusTable();
        }
    }

    void ShowStatusTable()
    {
        isStatusTableOpen = true;
        previousTimeScale = Time.timeScale; 

        Time.timeScale = 0f;
        statusTablePanel.SetActive(true);

        if (WeaponStatsManager.Instance != null)
        {
            swordDamageText.text = WeaponStatsManager.Instance.meleeDamage.ToString();
            bowDamageText.text = WeaponStatsManager.Instance.arrowDamage.ToString();
            spellDamageText.text = WeaponStatsManager.Instance.spellDamage.ToString();
        }
    }

    void HideStatusTable()
    {
        isStatusTableOpen = false;
        statusTablePanel.SetActive(false);

        if (!Pause.isGamePaused)
        {
            Time.timeScale = 1f;
        }
    }
}
