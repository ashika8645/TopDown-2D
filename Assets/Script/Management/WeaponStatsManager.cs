using UnityEngine;

public class WeaponStatsManager : MonoBehaviour
{
    public static WeaponStatsManager Instance { get; private set; }

    public int meleeDamage = 10;
    public int arrowDamage = 6;
    public int spellDamage = 8;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetDamage(DamageType type)
    {
        return type switch
        {
            DamageType.Melee => meleeDamage,
            DamageType.Ranged => arrowDamage,
            DamageType.Magic => spellDamage,
            _ => 1
        };
    }

    public void IncreaseDamage(DamageType type, int amount)
    {
        switch (type)
        {
            case DamageType.Melee:
                meleeDamage += amount;
                break;
            case DamageType.Ranged:
                arrowDamage += amount;
                break;
            case DamageType.Magic:
                spellDamage += amount;
                break;
        }

        UpdateAllDamageSources(type);
    }

    private void UpdateAllDamageSources(DamageType type)
    {
        DamageSource[] allSources = FindObjectsOfType<DamageSource>();
        foreach (DamageSource source in allSources)
        {
            if (source.damageType == type)
            {
                source.UpdateDamageFromManager();
            }
        }
    }
}
