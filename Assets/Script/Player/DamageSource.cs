using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public DamageType damageType;
    public int DamageAmount { get; private set; }

    private void OnEnable()
    {
        UpdateDamageFromManager();
    }

    private void Start()
    {
        UpdateDamageFromManager();
    }

    public void UpdateDamageFromManager()
    {
        if (WeaponStatsManager.Instance != null)
        {
            DamageAmount = WeaponStatsManager.Instance.GetDamage(damageType);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(DamageAmount);
        }
        var boss = other.GetComponent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(DamageAmount);
        }
    }
}
