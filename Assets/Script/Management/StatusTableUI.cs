using TMPro;
using UnityEngine;

public class StatusTableUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject statusTablePanel;

    public TMP_Text swordDamageText;
    public TMP_Text bowDamageText;
    public TMP_Text spellDamageText;

    private bool isTabHeld = false;

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
        isTabHeld = true;
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
        isTabHeld = false;
        Time.timeScale = 1f;
        statusTablePanel.SetActive(false);
    }
}
