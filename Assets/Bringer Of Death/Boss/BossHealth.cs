using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 50;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private int currentHealth;

    private Flash flash;
    private BossPathfinding pathfinding;
    private Animator animator;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        pathfinding = GetComponent<BossPathfinding>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = startingHealth;
        BossHealthUIManager.Instance?.ShowBossHealth(startingHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (flash != null)
            StartCoroutine(flash.FlashRoutine());

        BossHealthUIManager.Instance?.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / startingHealth;
    }

    private void Die()
    {
        if (pathfinding != null)
            pathfinding.StopMoving();

        if (animator != null)
        {
            animator.SetTrigger("Death");
            StartCoroutine(WaitThenDestroy());
        }
        else
        {
            Destroy(gameObject, 1f);
        }

        if (deathVFXPrefab != null)
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        GameManager.Instance?.AddEnemyKill();
        BossHealthUIManager.Instance?.HideBossHealth();
    }

    private IEnumerator WaitThenDestroy()
    {
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Death") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );
        Destroy(gameObject);
    }
}
