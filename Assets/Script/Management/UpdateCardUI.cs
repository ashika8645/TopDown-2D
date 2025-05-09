using UnityEngine;
using UnityEngine.UI;

public class UpdateCardUI : MonoBehaviour
{
    [Header("Card Buttons")]
    [SerializeField] private Button healthCardButton;
    [SerializeField] private Button swordCardButton;
    [SerializeField] private Button spellCardButton;

    private const int UPGRADE_COST = 15;

    private void Start()
    {
        healthCardButton.onClick.AddListener(OnHealthCardClicked);
        swordCardButton.onClick.AddListener(OnSwordCardClicked);
        spellCardButton.onClick.AddListener(OnSpellCardClicked);
    }

    private void OnHealthCardClicked()
    {
        if (!CanAffordUpgrade()) return;

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.IncreaseMaxHealth(1);
            SpendCoins(UPGRADE_COST);
        }
    }

    private void OnSwordCardClicked()
    {
        if (!CanAffordUpgrade()) return;

        WeaponStatsManager.Instance.IncreaseDamage(DamageType.Melee, 5);
        WeaponStatsManager.Instance.IncreaseDamage(DamageType.Ranged, 5);
        SpendCoins(UPGRADE_COST);
    }

    private void OnSpellCardClicked()
    {
        if (!CanAffordUpgrade()) return;

        WeaponStatsManager.Instance.IncreaseDamage(DamageType.Magic, 5);
        SpendCoins(UPGRADE_COST);
    }

    private bool CanAffordUpgrade()
    {
        return EconomyManager.Instance.GetCurrentGold() >= UPGRADE_COST;
    }

    private void SpendCoins(int amount)
    {
        EconomyManager.Instance.UpdateCurrentGold(-amount);
    }
}
