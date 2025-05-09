using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 5f;
    [SerializeField] private float damageRecoveryTime = 1f;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private Animator myAnimator;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        myAnimator = GetComponent<Animator>();
        Instance = this;

        if (healthSlider == null)
        {
            var go = GameObject.Find("Heart Container/Heart Slider");
            if (go != null)
                healthSlider = go.GetComponent<Slider>();
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy && canTakeDamage)
        {
            TakeDamage(1);
            knockback.GetKnockedBack(other.gameObject.transform, knockBackThrustAmount);
            StartCoroutine(flash.FlashRoutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root == transform || other.CompareTag("WeaponCollider")) return;

        if (!canTakeDamage) return;

        if (other.TryGetComponent<Projectile>(out var projectile))
        {
            TakeDamage(1);
            knockback.GetKnockedBack(other.transform, knockBackThrustAmount);
            StartCoroutine(flash.FlashRoutine());
            return;
        }

        if (other.TryGetComponent<GrapeLandSplatter>(out var grapeLandSplatter))
        {
            TakeDamage(1);
            knockback.GetKnockedBack(other.transform, knockBackThrustAmount);
            StartCoroutine(flash.FlashRoutine());
            return;
        }

        if (other.TryGetComponent<BulletProjectile>(out var bulletProjectile))
        {
            TakeDamage(1);
            knockback.GetKnockedBack(other.transform, knockBackThrustAmount);
            StartCoroutine(flash.FlashRoutine());
            return;
        }

        if (other.TryGetComponent<AOEEffect>(out var aoeEffect))
        {
            TakeDamage(1);
            knockback.GetKnockedBack(other.transform, knockBackThrustAmount);
            StartCoroutine(flash.FlashRoutine());
        }
    }


    public void HealPlayer()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += 1;
            UpdateHealthUI();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (!canTakeDamage) return;

        canTakeDamage = false;
        currentHealth -= damageAmount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(HandleDeath());
        }
        else
        {
            StartCoroutine(DamageRecoveryRoutine());
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private IEnumerator HandleDeath()
    {
        GetComponent<Player>().enabled = false;
        canTakeDamage = false;

        myAnimator.SetTrigger("Death");
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 0f;
        Destroy(gameObject); 

        GameObject uiCanvas = GameObject.FindWithTag("UICanvas");
        if (uiCanvas != null)
        {
            Destroy(uiCanvas);
        }
        SceneManager.LoadScene("GameOver");
    }
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}
